using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    public Camera cam;
    public Vector3 controlPosition;
    public float controlCD;
    public float controlDistance;
    public float moveSpeed;
    public float throwSpeed;
    public float throwAngle;
    public int numPoints;

    static public GameObject controledObject;
    int objectState; // 0-nothing  1-get object  2-can move  3-moving  4-on hand
    float controlTime;

    Vector3 ColliderSize;
    Vector3[] objectRoute;

    void Start()
    {
        controledObject = null;
        objectState = 0;
        objectRoute = new Vector3[numPoints];
    }

    void Update()
    {
        GetInput();
        MoveObject();
        OnHand();
    }

    void GetInput()
    {
        if (InputManager.GetButtonDown("Skill") && Time.time > controlTime + controlCD)
        {
            GetObject();
            CheckRoute();
            if (objectState == 2)
            {
                ColliderSize = controledObject.GetComponent<Collider>().bounds.size;
                controledObject.GetComponent<Rigidbody>().useGravity = false;
                controledObject.GetComponent<Collider>().enabled = false;
                controledObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //Debug.Log("Size: " + ColliderSize.ToString());
                objectState = 3;
                controlTime = Time.time;
            }
            else if (objectState == 4)
            {
                ThrowObject();
                controlTime = Time.time;
            }
            //Debug.Log("State: " + objectState);
        }
    }

    void GetObject()
    {
        if (objectState >= 3)
            return;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, controlDistance, -1, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Moveable") && controledObject != hitObject)
            {
                controledObject = hitObject;
                objectState = 1;
            }
        }
        else
        {
            controledObject = null;
            objectState = 0;
            ColliderSize = Vector3.zero;
        }
    }

    void CheckRoute()
    {
        if (objectState != 1 && (objectState != 2 || ColliderSize == controledObject.GetComponent<Collider>().bounds.size))
            return;

        objectRoute = new Vector3[numPoints];
        ColliderSize = controledObject.GetComponent<Collider>().bounds.size;
        objectState = 2;
    }

    void MoveObject()
    {
        if (objectState != 3)
            return;

        Vector3 pos = cam.transform.position + cam.transform.rotation * controlPosition;
        if (controledObject.transform.position == pos)
            return;

        Vector3 diff = pos - controledObject.transform.position;
        float speed = moveSpeed * Time.deltaTime * Mathf.Max(diff.magnitude, 1.1f);
        if(diff.magnitude <= speed)
        {
            controledObject.transform.position = pos;
            objectState = 4;
        }
        else
        {
            controledObject.transform.position += diff.normalized * speed;
        }
        controledObject.transform.rotation = Quaternion.Slerp(controledObject.transform.rotation, cam.transform.rotation, 3f * Time.deltaTime);
    }

    void OnHand()
    {
        if (objectState != 4)
            return;

        controledObject.transform.position = cam.transform.position + cam.transform.rotation * controlPosition;
        controledObject.transform.rotation = cam.transform.rotation;
    }

    void ThrowObject()
    {
        Vector3 Throw  = cam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, controlDistance, -1, QueryTriggerInteraction.Ignore))
        {
            var v = (hit.point - controledObject.transform.position).normalized;
            Throw = new Vector3(v.x, v.y, v.z);
            Throw = Quaternion.AngleAxis(-throwAngle, cam.transform.right) * Throw;
        }
        controledObject.GetComponent<Collider>().enabled = true;
        controledObject.GetComponent<Rigidbody>().useGravity = true;
        controledObject.GetComponent<Rigidbody>().velocity += Throw * throwSpeed;
        controledObject = null;
        objectState = 0;
    }

    Vector3[] GetRoute()
    {
        return null;
    }
}
