using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Verzerite_agent : Training_scripts
{
    public player_weapon Player_weapon;
    public Agent_weapon Agent_weapon;
    
    public override void Initialize()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in objectsWithTag)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                Player = player;
            }
        }
        Player_status = Player.GetComponentInChildren<Player_interface>();

        Player_weapon = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<player_weapon>();
        if(isAgent) Agent_weapon = Player.GetComponentInChildren<Agent_weapon>();
    }
    public override void OnEpisodeBegin()
    {
        Ai_scripts.Reset();
        if(isAgent) Player.GetComponent<Player_agent>().Reset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {        
        sensor.AddObservation(transform.position); //3 value
        sensor.AddObservation(transform.eulerAngles.y);
        sensor.AddObservation(Ai_scripts.forward.distance);
        sensor.AddObservation(Ai_scripts.left.distance);
        sensor.AddObservation(Ai_scripts.right.distance);
        sensor.AddObservation(Ai_scripts.back.distance);
        sensor.AddObservation(Ai_scripts.acting);
        sensor.AddObservation(Ai_scripts.iFrame);
        sensor.AddObservation(Ai_scripts.percent);
        sensor.AddObservation(Ai_scripts.HP);
        sensor.AddObservation(Ai_scripts.Stamina);
        //13 values now
        //sensor.AddObservation(Player.transform.position); //3 value
        if(isAgent) sensor.AddObservation(Agent_weapon.sweaping);
        else sensor.AddObservation(Player_weapon.sweaping);
        sensor.AddObservation(Player_status.HP);
        sensor.AddObservation(Player_status.Stamina);
        sensor.AddObservation(Player_status.iFrame);
        sensor.AddObservation(Player_status.toxicFrame);
        sensor.AddObservation(desicion_Delay);
        sensor.AddObservation(distance);
        //20 values now
    }
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        desicion_Delay = Mathf.Max(desicion_Delay - Time.deltaTime, 0);

        Vector2 dis = new Vector2(Player.transform.position.x - transform.position.x, Player.transform.position.z - transform.position.z);
        distance =  Mathf.Sqrt(dis.x * dis.x + dis.y * dis.y);
        
        if(desicion_Delay == 0){
            Ai_scripts.desx = vectorAction.ContinuousActions[0] >= 0 ? Mathf.Max(vectorAction.ContinuousActions[0], 0.25f) : Mathf.Min(vectorAction.ContinuousActions[0], -0.25f);
            if(distance < 8f) Ai_scripts.desy = vectorAction.ContinuousActions[1] >= 0 ? Mathf.Max(vectorAction.ContinuousActions[1], 0.25f) : Mathf.Min(vectorAction.ContinuousActions[1], -0.25f);
            else Ai_scripts.desy = 1;
            desicion_Delay = Random.Range(0.15f, 0.5f);;
        }

        if(Ai_scripts.iFrame == 0){
            int action = Mathf.Clamp((int)Mathf.Round(vectorAction.ContinuousActions[2] * 20), 0, 13);

            if(distance > 8f) {
                action = 0;
                AddReward(-distance);
            }

            Ai_scripts.state = action;

            if(distance > 4 && distance <= 7.5 && action == 5)
                AddReward(50);
            if(distance <= 6.5 && action == 4)
                AddReward(45);
            if(distance <= 6 && (action == 2 || action == 6))
                AddReward(40);
            if(distance <= 4 && (action == 3 || action == 7 || action == 11))
                AddReward(40);
            if(distance <= 3 && (action == 10 || action == 13))
                AddReward(30);
            if(distance <= 2.5 && (action == 9 || action == 12))
                AddReward(40);
            if(distance <= 2 && action == 8)
                AddReward(40);

            if(action == 1){
                float rad = vectorAction.ContinuousActions[3] * Mathf.PI;
                Ai_scripts.dash_x = Mathf.Sin(rad);
                Ai_scripts.dash_y = Mathf.Cos(rad);
                if(Ai_scripts.dash_y < 0 && distance < 2) Ai_scripts.dash_y = - Ai_scripts.dash_y;

                if(isAgent && Agent_weapon.sweaping && distance < 3)
                    AddReward(50);
            }
        } 
    }
}