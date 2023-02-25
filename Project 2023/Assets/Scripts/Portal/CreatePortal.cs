using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class CreatePortal : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 _hitPoint;
    public GameObject prefab;
    public Material Orangematerial;
    public Material Bluematerial;
    public Material PortalMaterial;
    private PairPortal[]  _portals;
    private PairPortal OrangePortal;
    private PairPortal BluePortal;
    Camera _cam;

    int turn = 0;
    void Awake () {
        //instantiate portal to empty list
        _portals = FindObjectsOfType<PairPortal>();
        _cam = GetComponent<Camera>();
    }

    void Update () {
        if (InputManager.GetButtonDown("Portal")) {
            SpawnPortal ();
        }
    }

    void SpawnPortal () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitfirst;
        if (Physics.Raycast(ray, out hitfirst) )
        {
            _portals = FindObjectsOfType<PairPortal>();
            if(_portals.Length == 0)
            {
                GameObject p = Instantiate(prefab, hitfirst.point, Quaternion.LookRotation(hitfirst.normal));
                p.transform.position += 3 *p.transform.forward;
                p.GetComponentInChildren<Camera>().enabled = false;
                //change Sceen 's material 第一個portal為橘色
                p.GetComponentInChildren<MeshRenderer>().material = Orangematerial;
                _portals = FindObjectsOfType<PairPortal>();
                _portals[0].setPortalId (0) ;
                OrangePortal = _portals[0];
                turn ++;
                
            }
            else if(_portals.Length == 1)
            {
                GameObject p = Instantiate(prefab, hitfirst.point, Quaternion.LookRotation(hitfirst.normal));
                p.transform.position += 3 *p.transform.forward;
                p.GetComponentInChildren<Camera>().enabled = false;
                //change Sceen 's material 第二個portal為藍色
                p.GetComponentInChildren<MeshRenderer>().material = Bluematerial;
                _portals = FindObjectsOfType<PairPortal>();
                //Add component to portal at simpleportal
                //set private variable in portal
                _portals[0].setPrivateVariable(_portals[1]);
                _portals[1].setPrivateVariable(_portals[0]);
                _portals[0].GetComponentInChildren<MeshRenderer>().material = PortalMaterial;
                _portals[1].GetComponentInChildren<MeshRenderer>().material = PortalMaterial;
                _portals[0].GetComponentInChildren<Camera>().enabled = true;
                _portals[1].GetComponentInChildren<Camera>().enabled = true;
                if(_portals[0].getPortalId() == 0){
                    _portals[1].setPortalId(1);
                    OrangePortal = _portals[0];
                    BluePortal = _portals[1];
                }
                else{
                    _portals[1].setPortalId(0);
                    OrangePortal = _portals[1];
                    BluePortal = _portals[0];
                }
                turn --;
            }
            else
            {
                //if turn is enum portal type 1 orange
                if(turn == 0){
                    _portals[1].transform.position = hitfirst.point;
                    Debug.Log("portal 1"+_portals[1].transform.position);
                    _portals[1].transform.position += 3 *hitfirst.normal;
                    Debug.Log("portal 1"+_portals[1].transform.position);
                    _portals[1].transform.rotation = Quaternion.LookRotation(hitfirst.normal);
                    turn ++;
                }
                else
                {
                    _portals[0].transform.position = hitfirst.point;
                    Debug.Log("portal 0"+_portals[0].transform.position);
                    _portals[0].transform.position += 3 *hitfirst.normal;
                    Debug.Log("portal 0"+_portals[0].transform.position);
                    _portals[0].transform.rotation = Quaternion.LookRotation(hitfirst.normal);

                    turn --;
                }
            }
        }
    }
}
