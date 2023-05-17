using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [SerializeField]
    private WayPointPath wayPointPath;
    [SerializeField]
    private DesaturateController dc;

    [SerializeField]
    private float speed;

    private int targetWaypointIndex;

    private Transform previousWayPoint;
    private Transform targetWayPoint;


    private float timeToWayPoint;
    private float elapsedTime;

    private void Start()
    {
        TargetNextPoint();
    }

    private void FixedUpdate()
    {
        if (dc.TimeIsStopped) return;
        else {
            elapsedTime += Time.deltaTime;

        float elapsedPercentage = elapsedTime / timeToWayPoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);

        transform.position = Vector3.Lerp(previousWayPoint.position, targetWayPoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(previousWayPoint.rotation, targetWayPoint.rotation, elapsedPercentage);

        if (elapsedPercentage >= 1) {
            TargetNextPoint();
        }
        }

    }

    private void TargetNextPoint() {
        previousWayPoint = wayPointPath.GetWayPoint(targetWaypointIndex);
        targetWaypointIndex = wayPointPath.GetNextWayPointIndex(targetWaypointIndex);
        targetWayPoint = wayPointPath.GetWayPoint(targetWaypointIndex);

        elapsedTime = 0;

        float DisToWayPoint = Vector3.Distance(previousWayPoint.position, targetWayPoint.position);
        timeToWayPoint = DisToWayPoint / (speed*10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && other.gameObject.tag== "Player") {
            other.transform.parent.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3 && other.gameObject.tag == "Player") {
            other.transform.parent.SetParent(null);
        }
    }
}
