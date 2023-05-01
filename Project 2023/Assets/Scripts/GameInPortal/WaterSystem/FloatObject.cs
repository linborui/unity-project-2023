using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
[RequireComponent(typeof(Rigidbody))]
public class FloatObject : MonoBehaviour
{
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;
    float waterHeight = -33.27f;
    float verticalInput;
    float horizontalInput;
    Rigidbody m_Rigidbody;
    bool underwater;
    bool playerIsOn;
    Transform player;
    Transform originalParent;
    Vector3 Distance;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    public void setWaterHeight(float height){
        waterHeight = height;
    }
    void OnCollisionEnter(Collision other){
        if(other.gameObject.tag == "Player"){
            playerIsOn = true;
            player = other.transform;
            originalParent = other.transform.parent.parent;
            other.transform.parent.parent = transform;
            Distance= player.transform.position - transform.position;
        }
    }
    void OnCollisionExit(Collision other){
        if(other.gameObject.tag == "Player"){
            playerIsOn = false;
            other.transform.parent.parent = originalParent;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float difference = transform.position.y - waterHeight;
        if(difference < 0){
            m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), 
                                           transform.position,
                                           ForceMode.Force);
            if(!underwater){
                underwater = true;
                SwitchState(true);
            }
        }
        else if(underwater){
            underwater = false;
            SwitchState(false);
        }
    }
    void SwitchState(bool isUnderwater){
        if(isUnderwater){
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterAngularDrag;
        }
        else{
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airAngularDrag;
        }   
    }
    void Update()
    {
        if(playerIsOn){
            float forwardThrust = 0;
            float turnThrust = 0;
            verticalInput = InputManager.GetAxisRaw("Vertical");
            horizontalInput = InputManager.GetAxisRaw("Horizontal");
            if(verticalInput > 0){
                forwardThrust = 1;
            }
            if(verticalInput < 0){
                forwardThrust = -1;
            }
            //add force and inherit to child
            transform.GetComponent<Rigidbody>().AddForce(player.transform.forward * forwardThrust* 50);
            
            
             Physics.SyncTransforms();
            if(horizontalInput < 0){
                turnThrust = 1;
            }
            if(horizontalInput > 0){
                turnThrust = -1;
            }
            transform.GetComponent<Rigidbody>().AddForce(player.transform.right * forwardThrust* 50);
            Quaternion rotation = Quaternion.Slerp(transform.rotation,player.transform.rotation, 5 * Time.deltaTime);
            //roate to the direction player look at
            transform.GetComponent<Rigidbody>().MoveRotation(rotation);
            //transform.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * turnThrust * 10);
             Physics.SyncTransforms();
            transform.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 5);
            Vector3 newPosition = transform.position;
            Physics.SyncTransforms();
            player.transform.position =  transform.position + Distance;
        }    
        if(InputManager.GetButtonDown("Jump")){
            playerIsOn = false;
        }
        
    }
}
