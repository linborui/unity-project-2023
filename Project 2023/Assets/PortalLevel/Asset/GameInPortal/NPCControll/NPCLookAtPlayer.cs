using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));

    }
}
