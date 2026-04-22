#if UNITY_ML_AGENTS
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace MLAgentsDemo
{
    [AddComponentMenu("ML-Agents/Simple Move Agent 2D")]
    public class SimpleMoveAgent2D : Agent
    {
        public Rigidbody2D body;
        public Transform target;
        public float moveSpeed = 2f;
        public float areaRadius = 3f;

        public override void Initialize()
        {
            if (body == null) body = GetComponent<Rigidbody2D>();
        }

        public override void OnEpisodeBegin()
        {
            var agentPos = Random.insideUnitCircle * areaRadius;
            transform.position = new Vector3(agentPos.x, agentPos.y, 0f);

            if (target != null)
            {
                var targetPos = Random.insideUnitCircle * areaRadius;
                target.position = new Vector3(targetPos.x, targetPos.y, 0f);
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.position.x);
            sensor.AddObservation(transform.position.y);
            if (target != null)
            {
                sensor.AddObservation(target.position.x);
                sensor.AddObservation(target.position.y);
            }
            else
            {
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
            var velocity = new Vector2(moveX, moveY) * moveSpeed;
            body.velocity = velocity;

            if (target != null)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                AddReward(-0.001f);
                if (dist < 0.5f)
                {
                    AddReward(1f);
                    EndEpisode();
                }
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.ContinuousActions;
            actions[0] = Input.GetAxisRaw("Horizontal");
            actions[1] = Input.GetAxisRaw("Vertical");
        }
    }
#endif
