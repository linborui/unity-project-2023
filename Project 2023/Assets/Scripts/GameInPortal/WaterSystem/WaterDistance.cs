using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDistance : MonoBehaviour
{
    public float distance = 0.0f;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "player"){
            other.GetComponent<FloatObject>().setWaterHeight(distance);
        }
        
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Water"){
            other.GetComponent<FloatObject>().setWaterHeight(-500);
        }
        
    }
}
