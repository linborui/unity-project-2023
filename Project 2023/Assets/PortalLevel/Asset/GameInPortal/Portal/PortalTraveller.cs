using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Every thing that need to transport the portal need to inherit this class or just mount this code
public class PortalTraveller : MonoBehaviour {
    public GameObject graphicsClone { get; set; }
    public int previousOffsetFromPortal { get; set; }
    
    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    private int inPortalCount = 0;
    private Portal inPortal;
    private Portal outPortal;
    private PortalForPair inPairPortal;
    private PortalForPair outPairPortal;
    public Rigidbody rigidBody;
    protected Collider collider;
    [HideInInspector]
    public static bool isTransporting { get; set; } = false;
    public bool isPlayer = false;
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    public virtual void Teleport (Transform fromPortal, Transform toPortal){ 
        if(!isPlayer){
            Matrix4x4 m = toPortal.localToWorldMatrix * fromPortal.worldToLocalMatrix * transform.localToWorldMatrix;
            transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);
            Physics.SyncTransforms ();    
            GetComponent<Rigidbody>().velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (GetComponent<Rigidbody>().velocity ));
        }
        else{
            PlayerMovement.isTransport = true;       
            Matrix4x4 pre = toPortal.localToWorldMatrix * fromPortal.worldToLocalMatrix;
            Matrix4x4 m = pre * transform.localToWorldMatrix;
            Matrix4x4 c = pre * Camera.main.transform.localToWorldMatrix; 
            Camera.main.transform.SetPositionAndRotation (c.GetColumn (3), c.rotation);
            transform.SetPositionAndRotation (m.GetColumn (3), m.rotation); 
            Camera.main.GetComponent<PlayerCam>().ResetTargetRotation();
            rigidBody.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidBody.velocity ));
            Vector3 dashDirection = GetComponent<PlayerMovement>().getDashDirection();
            GetComponent<PlayerMovement>().setDashDirection(toPortal.TransformVector (fromPortal.InverseTransformVector (dashDirection )));
            Physics.SyncTransforms ();
            PlayerMovement.isTransport = false;  
        }
    }
    public virtual void ExitPortalThreshold () {
        graphicsClone.SetActive (false);
        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceNormal", Vector3.zero);
        }
    }
    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone){
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } 
            else{
                originalMaterials[i].SetFloat ("sliceOffsetDst", dst);
            }
        }
    }

    Material[] GetMaterials (GameObject g) {
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }
        return matList.ToArray ();
    }
    
    protected virtual void Awake()
    {
        graphicsClone = new GameObject();
        graphicsClone.SetActive(false);
        MeshRenderer met = graphicsClone.AddComponent<MeshRenderer>();//.materials;
        MeshFilter mesh = graphicsClone.AddComponent<MeshFilter>();
        graphicsClone.transform.localScale = transform.localScale;

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if(this.GetComponentInChildren<SkinnedMeshRenderer>()){
            met.material = this.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial;
            Mesh bakedMesh = new Mesh();
            this.GetComponentInChildren<SkinnedMeshRenderer>().BakeMesh(bakedMesh);
            mesh.sharedMesh = bakedMesh;
        }
        else{
            met.sharedMaterial = this.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            mesh.sharedMesh = this.GetComponentInChildren<MeshFilter>().sharedMesh;
        }
        originalMaterials = GetMaterials (graphicsClone);
        cloneMaterials = GetMaterials (graphicsClone);
    }

    private void LateUpdate()
    {
        if(inPairPortal == null || outPairPortal == null)
        {
            return;
        }
        if(graphicsClone.activeSelf && inPairPortal.IsPlaced && outPairPortal.IsPlaced)
        {
            var inTransform = inPairPortal.transform;
            var outTransform = outPairPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            graphicsClone.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            graphicsClone.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            graphicsClone.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
        }
    }
    public void SetIsInPortal(Portal inPortal, Portal outPortal, Collider wallCollider)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;
        graphicsClone.SetActive(false);
    }
    public void ExitPortal()
    {
        graphicsClone.SetActive(false);     
    }
    public void SetIsInPairPortal(PortalForPair inPortal, PortalForPair outPortal, Collider wallCollider)
    {
        this.inPairPortal = inPortal;
        this.outPairPortal = outPortal;
        Physics.IgnoreCollision(collider, wallCollider);
        graphicsClone.SetActive(false);
        ++inPortalCount;
    }
    public void ExitPairPortal(Collider wallCollider)
    {
        Physics.IgnoreCollision(collider, wallCollider, false);
        --inPortalCount;

        if (inPortalCount == 0)
        {
            graphicsClone.SetActive(false);
        }
    }

    public void Warp()
    {
        var inTransform = inPairPortal.transform;
        var outTransform = outPairPortal.transform;
        
        if(isPlayer)
        { 
            isTransporting = true;
            Camera cam = Camera.main;
            Vector3 relativePosCam = inTransform.InverseTransformPoint(cam.transform.position);
            relativePosCam = halfTurn * relativePosCam;
            cam.transform.position = outTransform.TransformPoint(relativePosCam);
            
            Quaternion relativeRotCam = Quaternion.Inverse(inTransform.rotation) * cam.transform.rotation;
            relativeRotCam = halfTurn * relativeRotCam;
            cam.transform.rotation = outTransform.rotation * relativeRotCam;
            rigidBody.useGravity = false;
        }
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;
        Vector3 relativeVel = inTransform.InverseTransformDirection(rigidBody.velocity);
        relativeVel = halfTurn * relativeVel;
        rigidBody.velocity = outTransform.TransformDirection(relativeVel);
        

        var tmp = inPairPortal;
        inPairPortal = outPairPortal;
        outPairPortal = tmp;
        if(isPlayer){
            PlayerCam playerCam = Camera.main.GetComponent<PlayerCam>();
            playerCam.ResetTargetRotation();
            isTransporting = false;
            rigidBody.useGravity = true;
        }
    }
}