using UnityEngine;
using Combat.Enemies;
using Entity_Components;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

namespace MLAgentsDemo
{
    [AddComponentMenu("ML-Agents/Slime Agent")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlimeAgent : Agent
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindPlayer = true;
        [SerializeField] private bool autoCreateTrainingTarget = true;
        [SerializeField] private bool trainingMode = false;
        [SerializeField] private bool randomizeOnEpisode = true;
        [SerializeField] private bool randomizeTargetOnEpisode = true;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float arenaLimit = 4f;
        [SerializeField] private float reachDistance = 0.5f;
        [SerializeField] private bool useMoverIfAvailable = true;

        [Header("Auto Setup")]
        [SerializeField] private bool autoConfigureBehavior = true;
        [SerializeField] private string behaviorName = "SlimeBehavior";
        [SerializeField] private int decisionPeriod = 5;
        [SerializeField] private bool takeActionsBetweenDecisions = true;
        [SerializeField] private bool disableClassicAI = true;
        [SerializeField] private bool disableIfNoModel = true;
        [SerializeField] private bool fallbackToClassicAI = true;

        private Rigidbody2D rb;
        private Vector3 arenaCenter;
        private Vector2 desiredMove;
        private Vector2 lastPos;
        private Mover mover;
        private bool agentInitialized;
        private bool agentActive;
        private bool loggedActive;

        protected override void Awake()
        {
            var behavior = GetComponent<BehaviorParameters>();

            if (!trainingMode && disableIfNoModel && (behavior == null || behavior.Model == null))
            {
                if (fallbackToClassicAI)
                {
                    var classic = GetComponent<Enemy_WanderAttacker>();
                    if (classic != null) classic.enabled = true;
                }
                agentActive = false;
                enabled = false;
                return;
            }

            if (autoConfigureBehavior)
            {
                if (behavior == null) behavior = gameObject.AddComponent<BehaviorParameters>();
                behavior.BehaviorName = behaviorName;
                behavior.BehaviorType = trainingMode ? BehaviorType.Default : BehaviorType.InferenceOnly;
                behavior.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(2);
                behavior.BrainParameters.VectorObservationSize = 4;

                var decision = GetComponent<DecisionRequester>();
                if (decision == null) decision = gameObject.AddComponent<DecisionRequester>();
                decision.DecisionPeriod = decisionPeriod;
                decision.TakeActionsBetweenDecisions = takeActionsBetweenDecisions;
            }

            base.Awake();
            agentInitialized = true;
            agentActive = true;
            rb = GetComponent<Rigidbody2D>();
            arenaCenter = transform.position;
            lastPos = rb.position;
            mover = GetComponent<Mover>();

            if (disableClassicAI)
            {
                var classic = GetComponent<Enemy_WanderAttacker>();
                if (classic != null) classic.enabled = false;
            }

            if (!loggedActive)
            {
                loggedActive = true;
                Debug.Log($"SlimeAgent active. trainingMode={trainingMode}, behavior={behaviorName}");
            }
        }

        protected override void OnDisable()
        {
            if (!agentInitialized || !agentActive)
                return;

            base.OnDisable();
        }

        public override void Initialize()
        {
            ResolveTarget();
        }

        public override void OnEpisodeBegin()
        {
            ResolveTarget();

            if (!trainingMode || !randomizeOnEpisode)
                return;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            transform.position = arenaCenter + new Vector3(
                Random.Range(-arenaLimit, arenaLimit),
                Random.Range(-arenaLimit, arenaLimit),
                0f
            );

            if (target != null && randomizeTargetOnEpisode && autoCreateTrainingTarget && target.name == "SlimeTrainingTarget")
            {
                target.position = arenaCenter + new Vector3(
                    Random.Range(-arenaLimit, arenaLimit),
                    Random.Range(-arenaLimit, arenaLimit),
                    0f
                );
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (target == null)
            {
                sensor.AddObservation(Vector2.zero);
            }
            else
            {
                Vector2 toTarget = (Vector2)(target.position - transform.position);
                sensor.AddObservation(toTarget / arenaLimit);
            }

            Vector2 velocity = (rb.position - lastPos) / Mathf.Max(0.001f, UnityEngine.Time.fixedDeltaTime);
            sensor.AddObservation(velocity / Mathf.Max(0.001f, moveSpeed));
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Vector2 move = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
            move = Vector2.ClampMagnitude(move, 1f);
            desiredMove = move;

            if (useMoverIfAvailable && mover != null)
            {
                mover.Move(move);
            }
            else
            {
                rb.linearVelocity = move * moveSpeed;
            }

            AddReward(-0.001f);

            if (target != null)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                AddReward(Mathf.Clamp01(1f - dist / arenaLimit) * 0.001f);

                if (dist < reachDistance)
                {
                    AddReward(1f);
                    EndEpisode();
                }
            }
        }

        private void FixedUpdate()
        {
            lastPos = rb.position;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.ContinuousActions;
            actions[0] = Input.GetAxisRaw("Horizontal");
            actions[1] = Input.GetAxisRaw("Vertical");
        }

        private void ResolveTarget()
        {
            if (target != null) return;

            if (autoFindPlayer)
            {
                var player = GameObject.FindWithTag("Player");
                if (player == null) player = GameObject.Find("Player");
                if (player != null)
                {
                    target = player.transform;
                    return;
                }
            }

            if (!trainingMode || !autoCreateTrainingTarget) return;

            var go = new GameObject("SlimeTrainingTarget");
            go.transform.position = arenaCenter;
            target = go.transform;
        }
    }
}
