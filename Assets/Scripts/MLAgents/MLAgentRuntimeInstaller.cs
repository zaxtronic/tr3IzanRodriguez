#if UNITY_ML_AGENTS
using UnityEngine;
using Unity.MLAgents.Policies;

namespace MLAgentsDemo
{
    public static class MLAgentRuntimeInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (GameObject.Find("MLAgentRuntime") != null) return;

            var root = new GameObject("MLAgentRuntime");
            Object.DontDestroyOnLoad(root);

            var target = new GameObject("MLTarget");
            target.transform.SetParent(root.transform);
            target.transform.position = new Vector3(2f, 2f, 0f);
            var targetSprite = target.AddComponent<SpriteRenderer>();
            targetSprite.color = new Color(1f, 0.8f, 0.2f, 1f);

            var agent = new GameObject("MLAgent2D");
            agent.transform.SetParent(root.transform);
            agent.transform.position = Vector3.zero;

            var rb = agent.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = 1.5f;

            var col = agent.AddComponent<CircleCollider2D>();
            col.isTrigger = false;

            var sprite = agent.AddComponent<SpriteRenderer>();
            sprite.color = new Color(0.2f, 0.8f, 1f, 1f);

            var behavior = agent.AddComponent<BehaviorParameters>();
            behavior.BehaviorName = "SimpleMoveAgent2D";
            behavior.BrainParameters.VectorObservationSize = 4;
            behavior.BrainParameters.NumStackedVectorObservations = 1;
            behavior.BrainParameters.ActionSpec = Unity.MLAgents.Actuators.ActionSpec.MakeContinuous(2);

            var decision = agent.AddComponent<DecisionRequester>();
            decision.DecisionPeriod = 5;
            decision.TakeActionsBetweenDecisions = true;

            var simpleAgent = agent.AddComponent<SimpleMoveAgent2D>();
            simpleAgent.body = rb;
            simpleAgent.target = target.transform;
        }
    }
#endif
