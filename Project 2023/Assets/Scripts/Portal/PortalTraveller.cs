using System.Collections.Generic;
using UnityEngine;

//任何要穿過傳送門的東西都要記得繼承這個類別
public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    public Vector3 previousOffsetFromPortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    public bool isPlayer;
 
    public virtual void Teleport (Transform fromPortal, Transform toPortal){//, Vector3 pos, Quaternion rot) {
        //transform.position = pos;
        //transform.rotation = rot;
    }
    // Called when first touches portal
    public virtual void EnterPortalThreshold () {
        graphicsObject = this.gameObject;
        if(graphicsClone == null) {
            graphicsClone = new GameObject();
            graphicsClone.AddComponent<MeshFilter>();
            graphicsClone.AddComponent<MeshRenderer>();
            MeshRenderer met = graphicsClone.GetComponent<MeshRenderer>();
            MeshFilter mesh = graphicsClone.GetComponent<MeshFilter>();
            
            if(this.GetComponentInChildren<SkinnedMeshRenderer>()){
                met.material = this.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial;
                mesh.sharedMesh = this.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            }else{
                met.sharedMaterial = this.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                mesh.sharedMesh = this.GetComponentInChildren<MeshFilter>().sharedMesh;
            }
            originalMaterials = GetMaterials (graphicsClone);
            cloneMaterials = GetMaterials (graphicsClone);
        }
        else {
            graphicsClone.SetActive (true);
        }
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold () {
        graphicsClone.SetActive (false);
        // Disable slicing
        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceNormal", Vector3.zero);
        }
    }

    //更新shader的參數
    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone) 
            {
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } 
            else 
            {
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
}