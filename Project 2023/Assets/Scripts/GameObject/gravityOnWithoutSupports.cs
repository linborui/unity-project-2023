using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityOnWithoutSupports : MonoBehaviour
{
    public GameObject[] supports;
    bool isGravityOn = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isGravityOn) return;

        bool isAllOn = true;
        foreach (GameObject support in supports)
        {
            if (!support.GetComponent<gravityOnCollision>().isGravityOn)
            {
                isAllOn = false;
                break;
            }
        }
        if (isAllOn)
        {
            isGravityOn = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
