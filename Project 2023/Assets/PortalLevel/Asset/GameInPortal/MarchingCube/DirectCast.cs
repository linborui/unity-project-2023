using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectCast : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public int index;
    Vector3 _hitPoint;
    private float BrushSize = 1.2f;
    
    public void setWake(){
        if(!start)
        InvokeRepeating("Terraform",0f,0.2f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        
            StairManage.indexList.Enqueue(index);
        
    } 
    void OnTriggerExit(Collider other)
    {
        start = true;
        InvokeRepeating("UnTerraform",5f,0.2f);
        
    }
    private void Terraform()
    {
        if(start)return;
        RaycastHit hit;

        if (Physics.Raycast(transform.position,transform.forward, out hit, 1000))
        {
            Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
            if (hitChunk == null){
                return;
            }
            //Calculate the two object distance
            float distance = Vector3.Distance(transform.position, hit.point);
            if(distance <0.5f){
                CancelInvoke("Terraform");
                return;
            }
            _hitPoint = hit.point;
            hitChunk.EditWeights(_hitPoint, BrushSize, true);
        }
    }
    bool start = false;
    private void UnTerraform()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position,transform.forward, out hit, 1000))
        {
            Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
            if (hitChunk == null){
                return;
            }
            //Calculate the two object distance
            float distance = Vector3.Distance(transform.position, hit.point);
            if(distance >3.9f){
                start = false;
                return;
            }
            _hitPoint = hit.point;
            hitChunk.EditWeights(_hitPoint, BrushSize, false);
        }
    }
}
