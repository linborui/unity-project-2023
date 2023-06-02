using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_weapon : MonoBehaviour
{
    public bool Atk = false;
    public bool sweaping = false;
    public float myIframe = 0;
    private Quaternion desDeg;
    private Vector3 moveVel;
    private Vector3 sweapNormal;

    public void IframeSet(float frame){
        myIframe = frame;
    }

    bool CanSweap(){

        if(Mathf.Abs(transform.localEulerAngles.x) < 20f && Mathf.Abs(transform.localEulerAngles.y) < 20f && Mathf.Abs(transform.localEulerAngles.z) < 20f){
            return true;
        }
        return false;
    }

    void SwapeSword() { 
        float xAxis = Random.Range(0, 2) == 0 ? -1 : 1;
        float yAxis = Random.Range(0, 2) == 0 ? -1 : 1;
        float zAxis = 0.4f;
        
        if(sweaping) {
            if(transform.localRotation != desDeg){
                GetComponent<BoxCollider>().enabled = true;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, desDeg, 15f * Time.deltaTime);
            }else{
                GetComponent<BoxCollider>().enabled = false;
                sweaping = false;
            }
        }
        else if(Atk && CanSweap()){
            transform.localPosition = new Vector3(0, 1.2f, zAxis);
            float vx = xAxis, vy = yAxis;
            float v = Mathf.Sqrt(vx * vx + vy * vy) * Time.deltaTime;

            sweapNormal = Vector3.Cross(new Vector3( vx, vy, 0), new Vector3( 0, 0, 1)).normalized;
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(vx, -vy) * 180 / Mathf.PI);
            desDeg = Quaternion.AngleAxis(-179, sweapNormal) * transform.localRotation;
            sweaping = true;
        }
        else if(!Atk){
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0f, 1.2f, zAxis),ref moveVel, 0.1f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 10f * Time.deltaTime);
        }

        Atk = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(myIframe > 0){
            Atk = false;
            sweaping = false;
        }
        SwapeSword();
    }
}
