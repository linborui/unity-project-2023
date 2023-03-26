using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using UnityEngine.Rendering;
public class CreatePortal : MonoBehaviour
{
    // Start is called before the first frame update
    //Add on the camera 
    

    public GameObject CubePrefab;
    public Material Orangematerial;
    public Material Bluematerial;
    public Material PortalMaterial;
   
    public PairPortal InPortal;
    public PairPortal OutPortal;
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
        _cam = GetComponent<Camera>();
        InPortal.GetComponentInChildren<MeshRenderer>().material = Orangematerial;
        OutPortal.GetComponentInChildren<MeshRenderer>().material = Bluematerial;
        InPortal.GetComponentInChildren<Camera>().enabled = false;
        OutPortal.GetComponentInChildren<Camera>().enabled = false;
        RenderPipelineManager.beginCameraRendering += RenderPortal;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPortal;
    }

    void RenderPortal(ScriptableRenderContext context, Camera camera)
    {
        InPortal.PrePortalRender(context);
        OutPortal.PrePortalRender(context);
        InPortal.Render(context);
        OutPortal.Render(context);
        InPortal.PostPortalRender(context);
        OutPortal.PostPortalRender(context);

    }
    void Update () {
        if (InputManager.GetButtonDown("PortalIn")){
            p.SetActive(true);
        }
        if (InputManager.GetButton("PortalIn") /*&& mouseX != lastmouseX && mouseY != lastmouseY*/){
            //PositionShow();
        }
        if (InputManager.GetButtonUp("PortalIn")) {
            p.SetActive(false);
            SpawnPortal (0);
        }
        if (InputManager.GetButtonDown("PortalIn")){
            p.SetActive(true);
        }
        if (InputManager.GetButton("PortalOut") /*&& mouseX != lastmouseX && mouseY != lastmouseY*/){
            //PositionShow();
        }
        if (InputManager.GetButtonUp("PortalOut")) {
            p.SetActive(false);
            SpawnPortal (1);
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
    bool orange = true;
    void SpawnPortal (int portalID) {
        p.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitfirst;
        int layermask = 1<<6;
        if (Physics.Raycast(ray, out hitfirst,1000,~layermask) )
        {
            // Orient the portal according to camera look direction and surface direction.
            var cameraRotation = _cam.transform.rotation;
            var portalRight = cameraRotation * Vector3.right;
            
            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hitfirst.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward);

            if(portalID == 0)
            {
                InPortal.transform.rotation = Quaternion.LookRotation(portalForward, portalUp);
                InPortal.transform.position = hitfirst.point ;
                InPortal.transform.position -= 1f *InPortal.transform.forward;
                //InPortal.GetComponentInChildren<ParticleSystem>().Stop();
                if(InPortal.getCollider()) InPortal.setCollider (hitfirst.collider) ;
                else InPortal.setCollider (hitfirst.collider) ;

                
            }
            else if(portalID == 1)
            {
                OutPortal.transform.rotation = Quaternion.LookRotation(-portalForward, portalUp);
                OutPortal.transform.position = hitfirst.point ;
                OutPortal.transform.position += 1f *OutPortal.transform.forward;
                
                if(OutPortal.getCollider()) OutPortal.setCollider (hitfirst.collider) ;
                else OutPortal.setCollider (hitfirst.collider) ;
                
                InPortal.GetComponentInChildren<MeshRenderer>().material = PortalMaterial;
                OutPortal.GetComponentInChildren<MeshRenderer>().material = PortalMaterial;

                
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
