using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _rotationSpeed;
    private Rigidbody _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector3 _targetDirection;
    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
        
    }
    private void UpdateTargetDirection()
    {
        if(_playerAwarenessController.AwareOfPlayer)
        {
            //check if the player is in range
            //get the vector between the player and the enemy position
            // Vector3 enemyToPlayerVector = _player.position - transform.position;
            //In calculating a direction, we don't need the magnitude of the vector.
            //We just want the direction on its own
            // DirectionToPlayer = enemyToPlayerVector.normalized;
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
        else
        {
            _targetDirection = Vector3.zero;
        }
    }
    private void RotateTowardsTarget()
    {
        
        if(_targetDirection == Vector3.zero)
        {
            return;
        }
        //If there is a target direction,we will calculate the target rotation
        //We will use Quaternion LookRotation in the same way as we did when rotating the Player.
        //For the forward direction, we'll supply the current forward direction

        
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        
        _rigidbody.MoveRotation(rotation);
    }
    private void SetVelocity()
    {
        if(_targetDirection == Vector3.zero)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        else
        {
            _rigidbody.velocity = transform.forward * _speed;
        }
    }
}
