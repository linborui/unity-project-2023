using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class BoatController : MonoBehaviour
{
    GameObject player;
    Vector3 defaultPlayerTransform;
    float verticalInput;
    float horizontalInput;
    float startY;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("body"); 
        //defaultTransform = transform.position;
        startY = gameObject.transform.position.y;
    }
    bool isDrive = false;
    bool IsPlayerCloseToBoat(){
        return Vector3.Distance(player.transform.position, transform.position) < 2;
    }
    void setDriving(bool isDrive){
        this.isDrive = true;
        if(isDrive){
            //player.transform.position = transform.position;
        }
        else{
           // player.transform.position = defaultTransform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(IsPlayerCloseToBoat()){
            setDriving(!isDrive);
        }
        if(isDrive){
            Debug.Log("HI");
            float forwardThrust = 0;
            float turnThrust = 0;
            verticalInput = InputManager.GetAxisRaw("Vertical");
            horizontalInput = InputManager.GetAxisRaw("Horizontal");
            Debug.Log(verticalInput + "forward");
            if(verticalInput > 0){
                forwardThrust = 1;
            }
            if(verticalInput < 0){
                forwardThrust = -1;
            }
            if(transform.GetComponent<Rigidbody>()) Debug.Log("Has Rigidbody");
            else Debug.Log("No Rigidbody");
            transform.GetComponent<Rigidbody>().AddForce(transform.forward * forwardThrust* 5);
            if(horizontalInput > 0){
                turnThrust = -1;
            }
            if(horizontalInput < 0){
                turnThrust = 1;
            }
            transform.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * turnThrust * 5);
        }
        transform.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 5);
        Vector3 newPosition = transform.position;
       // newPosition.y = startY + Mathf.Sin(Time.timeSinceLevelLoad*2) / 8;
        //gameObject.transform.position = newPosition;
    }
}
