using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectCast : MonoBehaviour
{
    public Transform player;
    public bool forwardMove;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position and player is on the same plane but transform.position is 30 units in front of player
        if(forwardMove){
            transform.position = player.position + player.forward * 30 - player.up * 1.5f;
            transform.forward = -player.forward;
        }
        else{
            transform.position = player.position - player.forward * 30 - player.up * 1.5f;
            transform.forward = player.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(forwardMove){
            transform.position = player.position + player.forward * 30 - player.up * 1.5f;
            transform.forward = -player.forward;
        }
        else{
            transform.position = player.position - player.forward * 30 - player.up * 1.5f;
            transform.forward = player.forward;
        }
    }
    Vector3 _hitPoint;
    Vector3 _hitPrevPoint;
    public float BrushSize = 1f;
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
            if (hitChunk == null){
                Debug.Log("hitChunk == null");
                return;
            }
            //Calculate the two object distance
            float distance = Vector3.Distance(transform.position, hit.point);
            if(distance < 5) return;
            _hitPoint = hit.point;
            _hitPoint = new Vector3(_hitPoint.x , _hitPoint.y , _hitPoint.z );
               
            //if(distance > 10) add =true ;
            //else add = false;
            if(forwardMove) add = true;
            else add = false;
            Debug.Log("distance = " + distance);
            hitChunk.EditWeights(_hitPoint, BrushSize, add);
        }
    }
}
