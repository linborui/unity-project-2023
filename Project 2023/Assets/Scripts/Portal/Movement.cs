using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : PortalTraveller
{
    
    // Start is called before the first frame update
    public override void Teleport (Transform fromPortal, Transform toPortal){
        Matrix4x4 m = toPortal.localToWorldMatrix * fromPortal.worldToLocalMatrix * transform.localToWorldMatrix;
        transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);
        Physics.SyncTransforms ();    
        GetComponent<Rigidbody>().velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (GetComponent<Rigidbody>().velocity ));
    }
}
