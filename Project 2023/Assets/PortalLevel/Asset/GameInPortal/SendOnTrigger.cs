using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class SendOnTrigger : MonoBehaviour
{
    public GameObject target;
    public SendOnTrigger otherButton;
    public int state;
    public enum State
    {
        CanMOVE = 0,
        Block = 1,
        Fort = 2,
        Rotation = 3,
        Portal = 4,
        Cycle = 5,
    }
    public State currentState;
    private float RotateSpeed = 50f;
    int coolTime = 0;
    private Quaternion FirstAngles;
    private Quaternion AppendAngles;
    [HideInInspector]
    public bool startRotate = false,startCycle = false;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CoolDown", 0, 1);
    }
    void CoolDown(){
        coolTime--;
    }
    void OnTriggerEnter(Collider other){
        Quaternion goal;
        if(other.gameObject.tag == "Player"){
            if(currentState == State.Portal)
            {
                FindObjectOfType<PortalPlaceSet>().UpdateState(state);
            }
            else{
            if( coolTime > 0) return;
            
            switch (currentState)
            {
                case State.CanMOVE:
                    goal =  Quaternion.Euler(Vector3.up * 0);
                    target.transform.rotation = goal;
                    currentState = State.Block;
                    break;
                case State.Block:
                    goal =  Quaternion.Euler(Vector3.up * (90));
                    target.transform.rotation = goal;
                    currentState = State.CanMOVE;
                    break;
                case State.Fort:
                    target.GetComponent<LaunchProjectile>().enabled = true;
                    break;
                case State.Rotation:
                    if(!startRotate){
                        FirstAngles = target.transform.rotation;
                        if(target.GetComponent<PortalContainer>().AppendGameObject!= null)
                            AppendAngles= target.GetComponent<PortalContainer>().AppendGameObject.transform.rotation;
                    }
                    else{
                        target.transform.rotation = FirstAngles;
                        if(target.GetComponent<PortalContainer>().AppendGameObject!= null)
                            target.GetComponent<PortalContainer>().AppendGameObject.transform.rotation = AppendAngles;
                    }
                    startRotate = !startRotate;
                    break;
                case State.Cycle:
                    startCycle = !startCycle;
                    
                    break;
            }
            
            coolTime = 5;
            }
        }
    }
    void Update()
    {
        if(startRotate)
        {
            target.transform.Rotate(0,RotateSpeed * Time.deltaTime,0);
            if(target.GetComponent<PortalContainer>().AppendGameObject!= null)
                target.GetComponent<PortalContainer>().AppendGameObject.transform.Rotate(0,RotateSpeed * Time.deltaTime,0);
        }
        if(startCycle && currentState == State.Cycle)
        {
            target.transform.Rotate(0,0,6 * Time.deltaTime);
        }
    }

}
