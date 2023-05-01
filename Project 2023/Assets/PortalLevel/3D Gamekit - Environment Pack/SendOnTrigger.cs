using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendOnTrigger : MonoBehaviour
{
    public GameObject target;
    public enum State
    {
        CanMOVE = 0,
        Block = 1,
        Fort = 2
    }
    public State currentState;
    int coolTime = 0;

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
            if( coolTime > 0) return;
            Debug.Log("target.transform.rotation" + target.transform.rotation);
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
            }
             Debug.Log("target.transform.rotation"+ target.transform.rotation);
            coolTime = 5;
        }
    }
}
