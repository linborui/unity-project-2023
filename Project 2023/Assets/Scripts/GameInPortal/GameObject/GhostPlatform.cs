using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlatform : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] float disappearTime = 3f;
    Animator myAnim;
    [SerializeField] bool canReset;
    [SerializeField] float resetTime;
    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetFloat("DisappearTime", 1/disappearTime);
    }
    private void OnCollisionEnter(Collision collision){
        if(collision.transform.tag == playerTag){
            myAnim.SetBool("Trigger",true);
        }
    }
    public void TriggerReset(){
        if(canReset){
            StartCoroutine(Reset());
        }
    }
    IEnumerator Reset(){
        yield return new WaitForSeconds(resetTime);
        myAnim.SetBool("Trigger",false);
        //Disable collider
        GetComponent<Collider>().isTrigger = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
