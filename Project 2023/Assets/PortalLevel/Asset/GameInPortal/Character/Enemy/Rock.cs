using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates {HitPlayer, HitEnemy, HitNothing}
    private Rigidbody rb;
    public RockStates rockStates;
    [Header("Basic Settings")]
    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget(){
        if(target == null)
        target = FindObjectOfType<PlayerMovement>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            var otherStates = other.gameObject.GetComponent<CharacterStates>();
            otherStates.TakeDamage(damage,otherStates);
            Destroy(gameObject);
        }
        switch(rockStates)
        {
            case RockStates.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if(other.gameObject.CompareTag("Enemy"))
                {
                    var otherStates = other.gameObject.GetComponent<CharacterStates>();
                    otherStates.TakeDamage(damage,otherStates);
                }
                break;
        }
    }
}
