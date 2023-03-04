using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Vector3 initialVelocity = new Vector3(0, 0, 1);
    Vector3 PortalVelocity,Portal2Velocity;
    public Transform linked;
    // Start is called before the first frame update
    void Start()
    {
        PortalVelocity = transform.InverseTransformVector(initialVelocity);
        Portal2Velocity = linked.TransformVector(PortalVelocity);
        Debug.Log(PortalVelocity);
        Debug.Log(Portal2Velocity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
