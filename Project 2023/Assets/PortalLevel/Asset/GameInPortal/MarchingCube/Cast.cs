using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Cast : MonoBehaviour
{
    Vector3 _hitPoint;
    Vector3 _hitPrevPoint;
    public float BrushSize = 2f;
    public bool add;
    // Start is called before the first frame update
    private void LateUpdate()
    {
        
        Terraform();
    
    }

    private void Terraform()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position,transform.forward, out hit, 1000))
        {
            Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
            if (hitChunk == null)
                return;
            //Calculate the two object distance
            float distance = Vector3.Distance(transform.position, hit.point);
            
            _hitPoint = hit.point;

               
            if(distance > 50) add =false ;
            else add = false;
            hitChunk.EditWeights(_hitPoint, BrushSize, add);
        }
    }
}
