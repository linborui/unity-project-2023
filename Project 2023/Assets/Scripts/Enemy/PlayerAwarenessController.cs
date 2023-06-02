using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    //whether the enemy is aware of the player
    public bool AwareOfPlayer {get; private set;}
    //It is useful to know the direction of the player
    public Vector3 DirectionToPlayer {get; private set;}
    //Add serialized field to make it visible in the inspector
    [SerializeField]
    private float _playerAwarenessDistance;
    private Transform _player;
    private void Awake()
    {
        //Find the player
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        //check if the player is in range
        //get the vector between the player and the enemy position
        Vector3 enemyToPlayerVector = _player.position - transform.position;
        //In calculating a direction, we don't need the magnitude of the vector.
        //We just want the direction on its own
        DirectionToPlayer = enemyToPlayerVector.normalized;

        //check if the player is in range
        if(enemyToPlayerVector.magnitude <= _playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
