using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
[RequireComponent(typeof(PortalCamera))]
public class PortalPlacement : MonoBehaviour
{
    [SerializeField]
    private PortalPair portals;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]


    private PlayerCam cameraMove;

    private void Awake()
    {
        cameraMove = GetComponent<PlayerCam>();
    }

    private void Update()
    {
        if(InputManager.GetButtonDown("PortalIn"))
        {
            FirePortal(0, transform.position, transform.forward, 200.0f);
        }
        else if (InputManager.GetButtonDown("PortalOut"))
        {
            FirePortal(1, transform.position, transform.forward, 200.0f);
        }
    }

    private void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance);

        if(hit.collider != null)
        {
            if (hit.collider.tag == "Portal")
            {
                var inPortal = hit.collider.GetComponent<PortalForPair>();

                if(inPortal == null)
                {
                    return;
                }

                var outPortal = inPortal.OtherPortal;
                Vector3 relativePos = inPortal.transform.InverseTransformPoint(hit.point + dir);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                pos = outPortal.transform.TransformPoint(relativePos);
                Vector3 relativeDir = inPortal.transform.InverseTransformDirection(dir);
                relativeDir = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeDir;
                dir = outPortal.transform.TransformDirection(relativeDir);
                distance -= Vector3.Distance(pos, hit.point);
                FirePortal(portalID, pos, dir, distance);
                return;
            }

            // Orient the portal according to camera look direction and surface direction.
            var cameraRotation = cameraMove.TargetRotation;
            var portalRight = cameraRotation * Vector3.right;
            
            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);
            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);
            bool wasPlaced = portals.Portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation);
        }
    }
}
