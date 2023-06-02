using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Training_scripts : Agent
{
    public bool Reset = false;
    public bool isAgent = false;
    public AI Ai_scripts;
    public GameObject Player;
    public Player_interface Player_status;
    protected int LastAction = 0;
    protected float desicion_Delay;
    protected float distance;
    protected bool plus = false;

    public void SetPoint(int point)
    {
        AddReward(point);

        if(Reset) {
            Reset = false;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }
}