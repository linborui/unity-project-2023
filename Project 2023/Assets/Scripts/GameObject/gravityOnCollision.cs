using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class gravityOnCollision : MonoBehaviour
{
    public bool isGravityOn = false;
    public void OnCollisionEnter(Collision collision)
    {
        if (isGravityOn) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Weapon"))
        {
            isGravityOn = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isGravityOn) return;

        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Weapon"))
        {
            isGravityOn = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
