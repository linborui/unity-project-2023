using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyDelayOnCollision : MonoBehaviour
{
    public float delayTime;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Invoke("Destroy", delayTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Invoke("Destroy", delayTime);
        }
    }
    
}
