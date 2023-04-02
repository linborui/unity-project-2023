using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public PortalForPair[] Portals { private set; get; }

    private void Awake()
    {
        Portals = GetComponentsInChildren<PortalForPair>();

        if(Portals.Length != 2)
        {
            Debug.LogError("PortalPair children must contain exactly two Portal components in total.");
        }
    }
}
