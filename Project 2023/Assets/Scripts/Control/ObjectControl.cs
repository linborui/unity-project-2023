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
    public int pathPartition;
    public float pathUnit;
    public float moveSpeed;
    public float throwSpeed;
    public float throwAngle;
    public float collectRadius;

    static public GameObject controledObject;
    int objectState; // 0-nothing  1-get object  2-can move  3-moving  4-on hand
    float controlTime;

    Vector3 destination;
    DStarLite3d dStarLiteRoute;
    Queue<Vector3> objectPath;

    List<GameObject> collectedItems;

    GameObject timeManager;
    int pastlayer;
    int presentlayer;

    void Start()
    {
        controledObject = null;
        objectState = 0;
        objectPath = null;
        collectedItems = new List<GameObject>();
        timeManager = GameObject.FindGameObjectWithTag("TimeManager");
        pastlayer = LayerMask.NameToLayer("Past");
        presentlayer = LayerMask.NameToLayer("Present");
    }

    void Update()
    {
        MoveObject();
        GetInput();
    }

    void FixedUpdate()
    {
        OnHand();
    }

    void GetInput()
    {
        if (InputManager.GetButtonDown("Skill") && Time.time > controlTime + controlCD)
        {
            GetObject();
            FindPath();
            if (objectState == 2)
            {
                controledObject.GetComponent<Rigidbody>().useGravity = false;
                controledObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                objectState = 3;
                controlTime = Time.time;
            }
            else if (objectState == 4)
            {
                ThrowObject();
                controlTime = Time.time;
            }
            Debug.Log("State: " + objectState);
        }
    }

    void GetObject()
    {
        if (objectState >= 3)
            return;

        int ignoreLayer = 0;
        int pastBool = timeManager.GetComponent<TimeShiftingController>().PastBool;
        if (pastBool == 0 || pastBool == 3)
            ignoreLayer |= 1 << pastlayer;
        else
            ignoreLayer |= 1 << presentlayer;
        ignoreLayer = ~ignoreLayer;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, controlDistance, ignoreLayer, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Moveable"))
            {
                controledObject = hitObject;
                objectState = 1;
            }
            else if (hitObject.CompareTag("Item"))
            {
                collectedItems.Add(hitObject);
                Collider[] colliders = Physics.OverlapSphere(hitObject.transform.position, collectRadius, ignoreLayer);
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Item") && !hitObject.Equals(collider.gameObject))
                    {
                        collectedItems.Add(collider.gameObject);
                    }
                }
            }
            else
            {
                controledObject = null;
                objectState = 0;
            }
        }
        else
        {
            controledObject = null;
            objectState = 0;
        }
    }

    void FindPath()
    {
        if (objectState != 1)
            return;

        destination = cam.transform.position + cam.transform.rotation * controlPosition;
        var diff = destination - controledObject.transform.position;
        dStarLiteRoute = new DStarLite3d(controledObject, pathUnit, pathPartition, diff.magnitude * 1.2f);
        objectPath = dStarLiteRoute.FindPath(controledObject.GetComponent<Renderer>().bounds.center, destination);
        if (objectPath != null)
            objectState = 2;
    }

    void MoveObject()
    {
        if (timeManager.GetComponent<DesaturateController>().TimeIsStopped)
            return;

        float deltaTime = (Time.deltaTime < 0.1f) ? Time.deltaTime : 0.1f;
        destination = cam.transform.position + cam.transform.rotation * controlPosition;
        Vector3 diff;
        float speed;
        Queue<GameObject> removedItems = new Queue<GameObject>();
        foreach (GameObject item in collectedItems)
        {
            diff = destination - item.transform.position;
            speed = moveSpeed * deltaTime * Mathf.Max(diff.magnitude, 1.1f);

            if (diff.magnitude > speed)
            {
                item.transform.position += diff.normalized * speed;
            }
            else
            {
                removedItems.Enqueue(item);
            }
        }
        while(removedItems.Count > 0)
        {
            GameObject item = removedItems.Dequeue();
            collectedItems.Remove(item);
            Debug.Log("Collect " + item);
            Destroy(item);
        }

        if (objectState != 3)
            return;

        diff = destination - controledObject.transform.position;
        speed = moveSpeed * deltaTime * Mathf.Max(diff.magnitude, 1.1f);
        Vector3 currPos = controledObject.transform.position;
        while (speed > 0)
        {
            if (objectPath.Count == 0)
            {
                diff = destination - currPos;
                if (diff.magnitude > speed)
                {
                    currPos += diff.normalized * speed;
                    controledObject.transform.position = currPos;
                    break;
                }
                controledObject.transform.position = destination;
                objectState = 4;
                break;
            }
            diff = objectPath.Peek() - currPos;
            if (diff.magnitude > (destination - currPos).magnitude)
            {
                objectPath.Clear();
                continue;
            }
            if (diff.magnitude > speed)
            {
                currPos += diff.normalized * speed;
                controledObject.transform.position = currPos;
                break;
            }
            speed -= diff.magnitude;
            currPos = objectPath.Dequeue();
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
            Throw = Quaternion.AngleAxis(-throwAngle, cam.transform.right) * v;
        }
        controledObject.GetComponent<Collider>().enabled = true;
        controledObject.GetComponent<Rigidbody>().useGravity = true;
        controledObject.GetComponent<Rigidbody>().velocity += Throw * throwSpeed;
        controledObject = null;
        objectState = 0;
    }
}
