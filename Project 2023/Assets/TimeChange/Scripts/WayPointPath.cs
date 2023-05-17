using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
public Transform GetWayPoint(int waypointIndex) {
        return transform.GetChild(waypointIndex);
    }
public int GetNextWayPointIndex(int currentpointIndex) {
        int nextWayPointIndex = currentpointIndex + 1;
        if (nextWayPointIndex == transform.childCount) {
            nextWayPointIndex = 0;
        }
        return nextWayPointIndex;
    }
}
