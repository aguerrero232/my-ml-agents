using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent {

    public float forceMultiplier = 10;
    public Transform Target;
    Rigidbody rBody;

    
    // Start is called before the first frame update
    void Start() {
        // getting sphere agent
        rBody = GetComponent<Rigidbody>();
    }

    // Initialization and Resetting the Agent
    public override void OnEpisodeBegin() {
        // if agent fell, its momentum is 0
        if (this.transform.localPosition.y < 0) {
            this.rBody.angularVelocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, .5f, 0);
        }

        // taget did not fall and is moving to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }


    // Observing the Environment
    public override void CollectObservations(VectorSensor sensor) {
        // Target and Agent Positions on the floor plane
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent Velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }


    // Taking Actions and Assigning Rewards
    public override void OnActionReceived(ActionBuffers actions) {
        // 
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Agent Reached the target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Agent fell off the platform
        else if (this.transform.localPosition.y < 0) {
            EndEpisode();
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
