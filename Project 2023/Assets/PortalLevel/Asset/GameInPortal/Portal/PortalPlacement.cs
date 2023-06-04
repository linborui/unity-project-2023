using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(PortalCamera))]
public class PortalPlacement : MonoBehaviour
{
    [SerializeField]
    private PortalPair portals;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private PlayerCam cameraMove;
    //TODO: Move this to a prefab
    public TextMeshProUGUI PortalIn;
    public TextMeshProUGUI PortalOut;
    [HideInInspector]
    public int portalInNum = 3; 
    [HideInInspector]
    public int portalOutNum = 3;
    bool wasPlaced = false;

    private void Awake()
    {
        cameraMove = GetComponent<PlayerCam>();
    }
    void Start(){
        PortalIn.text = portalInNum.ToString();
        PortalOut.text = portalOutNum.ToString();
    }
    private void Update()
    {
        if(InputManager.GetButtonDown("PortalIn") && portalInNum > 0)
        {
            FirePortal(0, transform.position, transform.forward, 500.0f);
            if(wasPlaced){
                portalInNum -= 1;
                wasPlaced=false;
            }
        }
        else if (InputManager.GetButtonDown("PortalOut") && portalOutNum > 0)
        {
            FirePortal(1, transform.position, transform.forward, 500.0f);
            if(wasPlaced){
                portalOutNum -= 1;
                wasPlaced=false;
            }
            else{
                Debug.Log("Can't place portal");
            }
        }
        PortalIn.text = portalInNum.ToString();
        PortalOut.text = portalOutNum.ToString();
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
            var traveller = hit.collider.GetComponent<PortalContainer> ();
            if (traveller) {
                hit.collider.GetComponent<PortalContainer>().AppendGameObject = portals.Portals[portalID].transform.gameObject;
            }

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
            wasPlaced = portals.Portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation);
        }
    }
}
