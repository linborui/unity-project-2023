using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    void OnEnable(){
        Invoke("destroy",1000f);
    }
    
    void destroy(){
        Destroy(transform.gameObject);
    }
}
