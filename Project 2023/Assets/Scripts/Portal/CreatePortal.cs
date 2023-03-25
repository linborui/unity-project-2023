using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class CreatePortal : MonoBehaviour
{
    // Start is called before the first frame update
    //Add on the camera 
    
    Vector3 _hitPoint;
    public GameObject prefab;
    public GameObject CubePrefab;
    public Material Orangematerial;
    public Material Bluematerial;
    public Material PortalMaterial;
    private PairPortal[]  _portals;
    private PairPortal OrangePortal;
    private PairPortal BluePortal;
    public GameObject pmodel; 
    GameObject p;
    Camera _cam;
    float mouseX, mouseY,lastmouseX,lastmouseY;
    float xSensitivity = 20f;
    float ySensitivity = 20f;
    int turn = 0;
    GameObject timeManager;
    int pastlayer;
    int presentlayer;
    void Start(){
        p = Instantiate(pmodel, new Vector3(0,0,0), Quaternion.identity);
        p.SetActive(false);
        timeManager = GameObject.FindGameObjectWithTag("TimeManager");
        pastlayer = LayerMask.NameToLayer("Past");
        presentlayer = LayerMask.NameToLayer("Present");
    }
    void Awake () {
        //instantiate portal to empty list
        _portals = FindObjectsOfType<PairPortal>();
        _cam = GetComponent<Camera>();
    }

    void Update () {
        if (InputManager.GetButtonDown("Portal")){
            p.SetActive(true);
        }
        if (InputManager.GetButton("Portal") /*&& mouseX != lastmouseX && mouseY != lastmouseY*/){
            PositionShow();
            lastmouseX = mouseX;
            lastmouseY = mouseY;
        }
        if (InputManager.GetButtonUp("Portal")) {
            p.SetActive(false);
            SpawnPortal ();
        }
        if(InputManager.GetButtonDown("Cube")){
            SpawnCube();
        }
    }
    void PositionShow(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitfirst= new RaycastHit();
        mouseX = InputManager.GetAxisRaw("Mouse X");
        mouseY = InputManager.GetAxisRaw("Mouse Y");
        int ignoreLayer = 0;
        int pastBool = timeManager.GetComponent<TimeShiftingController>().PastBool;
        if (pastBool == 0 || pastBool == 3)
            ignoreLayer |= 1 << pastlayer;
        else
            ignoreLayer |= 1 << presentlayer;
        ignoreLayer = ~ignoreLayer;
        int layermask = 1<<6;
        if (Physics.Raycast(ray, out hitfirst,1000f,~ layermask, QueryTriggerInteraction.Ignore))
        {
            lastmouseX = mouseX;
            lastmouseY = mouseY;
            Debug.Log("hit的到的物體"+hitfirst.collider.gameObject.name);
            Debug.Log("hitfirst.point"+hitfirst.point);
    
            p.transform.position = hitfirst.point +  1 *p.transform.forward;
            p.transform.rotation = Quaternion.LookRotation(hitfirst.normal);
            if(turn == 0){
                p.GetComponentInChildren<MeshRenderer>().material = Orangematerial;
            }
            else{
                p.GetComponentInChildren<MeshRenderer>().material = Bluematerial;
            }
            
        
        }
    }
    void SpawnPortal () {
        p.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitfirst;
        int layermask = 1<<6;
        if (Physics.Raycast(ray, out hitfirst,1000,~layermask) )
        {
            _portals = FindObjectsOfType<PairPortal>();
            if(_portals.Length == 0)
            {
                GameObject p = Instantiate(prefab, hitfirst.point, Quaternion.LookRotation(hitfirst.normal));
                p.transform.position += 0.05f *p.transform.forward;
                p.GetComponentInChildren<Camera>().enabled = false;
                
                //change Sceen 's material 第一個portal為橘色
                p.GetComponentInChildren<MeshRenderer>().material = Orangematerial;
                p.GetComponentInChildren<ParticleSystem>().Stop();
               
                _portals = FindObjectsOfType<PairPortal>();
                _portals[0].setPortalId (0) ;
                _portals[0].setCollider (hitfirst.collider) ;
                OrangePortal = _portals[0];
                turn ++;
                
            }
            else if(_portals.Length == 1)
            {
                GameObject p = Instantiate(prefab, hitfirst.point, Quaternion.LookRotation(-hitfirst.normal));
                p.transform.position -= 0.05f *p.transform.forward;
                p.GetComponentInChildren<Camera>().enabled = false;

                //change Sceen 's material 第二個portal為藍色
                p.GetComponentInChildren<MeshRenderer>().material = Bluematerial;
                
                _portals = FindObjectsOfType<PairPortal>();
                if(_portals[0].getCollider()) _portals[1].setCollider (hitfirst.collider) ;
                else _portals[0].setCollider (hitfirst.collider) ;
                //Add component to portal at simpleportal
                //set private variable in portal
                _portals[0].setPrivateVariable(_portals[1]);
                _portals[1].setPrivateVariable(_portals[0]);
                _portals[0].GetComponentInChildren<MeshRenderer>().material = PortalMaterial;
                _portals[1].GetComponentInChildren<MeshRenderer>().material = PortalMaterial;
                //_portals[0].GetComponentInChildren<Camera>().enabled = true;
                //_portals[1].GetComponentInChildren<Camera>().enabled = true;
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
                    _portals[1].setCollider(hitfirst.collider);
                    _portals[1].transform.position = hitfirst.point;
                    Debug.Log("portal 1"+_portals[1].transform.position);
                    _portals[1].transform.position += 0.05f *hitfirst.normal;
                    Debug.Log("portal 1"+_portals[1].transform.position);
                    _portals[1].transform.rotation = Quaternion.LookRotation(hitfirst.normal);
                    turn ++;
                }
                else
                {
                    _portals[0].setCollider(hitfirst.collider);
                    _portals[0].transform.position = hitfirst.point;
                    Debug.Log("portal 0"+_portals[0].transform.position);
                    _portals[0].transform.position -= 0.05f *hitfirst.normal;
                    Debug.Log("portal 0"+_portals[0].transform.position);
                    _portals[0].transform.rotation = Quaternion.LookRotation(-hitfirst.normal);

                    turn --;
                }
            }
        }
    }
    void SpawnCube () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitfirst;
        if (Physics.Raycast(ray, out hitfirst) )
        {
            GameObject p = Instantiate(CubePrefab, hitfirst.point, Quaternion.LookRotation(hitfirst.normal));
            p.transform.position += 3 *p.transform.forward;
        } 
    }
    
}
