using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBossPlatform : MonoBehaviour
{
    public Transform startpoint;
    public Transform endpoint;
    public Transform bossendpoint;
    public Transform keyendpoint;
    public GameObject bossPlatform;
    public GameObject boss;
    public GameObject key;
    [HideInInspector]
    static public int GolemInLevel2Num; 
    // Start is called before the first frame update
    void Start()
    {
        GolemInLevel2Num = 7; 
        bossPlatform.transform.position =startpoint.position;
        bossPlatform.transform.position = endpoint.position;
            boss.transform.position = bossendpoint.position;
            key.transform.position = keyendpoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(GolemInLevel2Num == 0){
            bossPlatform.transform.position = endpoint.position;
            boss.transform.transform.position = bossendpoint.position;
            key.transform.position = keyendpoint.position;
        }
    }
    public void dead(){
        Debug.Log("number"+GolemInLevel2Num);
        GolemInLevel2Num--;
    }
}
