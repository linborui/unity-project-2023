using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrkAssasin : AI
{
    bool repeatAtk = false;

    public override bool IfDead()
    {
        if(dead == true){
            react = false;
            dodge = false;
            parry = false;
            state = 0;
            acting = 0;
            atkState = -1;
            
            //When testing you need to comment under forloop statement
            foreach (Transform child in weapons)
            {
                if(ObjectControl.controledObject == null || !child.Equals(ObjectControl.controledObject.transform)){
                    child.transform.SetParent(null);
                    
                    child.GetComponent<atk_trigger>().enabled = false;
                    child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }
            weapons.Clear();
            if(transform.GetComponent<life_time>() == null) this.gameObject.AddComponent<life_time>();

            Object.Destroy(this.gameObject);
            return true;
        }else{
            return false;
        }     
    }

    // Update is called once per frame
    void Update()
    {
        if(IfDead()) return;
        Detect();

        if(awareness == true) {
            Facing();

            if(fsm.GetCurrentAnimatorStateInfo(0).IsName("movement")) acting = 0;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("dodge")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk0")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk1")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk2")) acting = 4;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk3")) acting = 5;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("gethit")) acting = 6;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1:
                        if(Stamina >= 10) {if(!repeat){Stamina -= 10; dodge = true;} repeat = true;}
                        break;
                    case 2:
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 0; repeat = true;}
                        break;
                    case 3:
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 1; repeat = true;}
                        break;
                    case 4:
                        if(Stamina >= 40) {if(!repeat)Stamina -= 40; atkState = 2; repeat = true;}
                        break;
                    case 5:
                        if(Stamina >= 30) {if(!repeat)Stamina -= 30; atkState = 3; repeat = true;}
                        break;
                }
            }

            iFrame = Mathf.Max(iFrame - Time.deltaTime, 0);
            
            if(acting == 0){
                spin = true;
                repeatAtk = false;
                
                Stamina = Mathf.Min(Stamina + StaminaSp * Time.deltaTime, MaxStamina);
                Vel = Quaternion.LookRotation(transform.forward) * new Vector3(x * sp , 0, y * sp);
            }else{
                state = 0;
                repeat = false;
                atkState = -1;
                //movement
                switch (acting)
                {
                    case 1:
                        Vel = Vector3.zero;
                        if (percent > 0.2 && percent < 0.8) { Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0, 0, -sp); spin = false; }
                        else if (percent > 0.8) dodge = false;
                        break;
                    case 2:
                        spin = true;
                        Vel = Vector3.zero;
                        if (percent < 0.25f) { Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0, 0, 0.2f * sp); spin = false; }
                        break;
                    case 3:
                        spin = true;
                        Vel = Vector3.zero;
                        if (percent >= 0.5f && percent < 0.3f) { Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0, 0, 0.3f * sp); spin = false; }
                        break;
                    case 4:
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent >= 0.3f && percent < 0.9f) { Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0, 0, 1.5f * sp); }
                        break;
                    case 5:
                        spin =  false;
                        Vel = Vector3.zero;
                        if (percent >= 0.3f && percent < 0.7f) { Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0, 3, 2 * sp); }
                        break;
                    case 6:
                        if (percent < 0.7f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -1f * sp); spin = false;}
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
