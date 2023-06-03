using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockman : AI
{
    public Cast_magic L, R;
    bool repeatAtk = false;

    // Update is called once per frame
    void Update()
    {
        if(IfDead()) return;
        Detect();

        if(awareness == true) {
            Facing();

            if(fsm.GetCurrentAnimatorStateInfo(0).IsName("movement")) acting = 0;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("roar")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("stab")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("cast0")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 4;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1:
                        if(Stamina >= 10) {if(!repeat){Stamina -= 10; dodge = true;} repeat = true;} /**no dodge**/
                        break;
                    case 2:
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 0; repeat = true;}
                        break;
                    case 3:
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 1; repeat = true;}
                        break;
                    case 4:
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 2; repeat = true;}
                        break;
                    case 5:
                        if(Stamina >= 25) {if(!repeat)Stamina -= 25; atkState = 3; repeat = true;}
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
                repeat = false;
                state = 0;
                atkState = -1;
                //movement
                switch (acting)
                {
                    case 1:
                        Vel = Vector3.zero;
                        if (percent > 0.2 && percent < 0.8) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(x * sp , 0, -sp); spin = false;}
                        else if(percent > 0.8) dodge = false;
                        break;
                    case 2:
                        if (percent < 0.2) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 0.2f); spin = true;}
                        else if (percent > 0.2 && percent < 0.4) {Vel = Vector3.zero; spin =  false;}
                        else if (percent > 0.4 && percent < 0.6) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 0.2f); spin = true;}
                        else if (percent > 0.6 && percent < 0.8) {Vel = Vector3.zero; spin =  false;}
                        else if (percent > 0.8 && percent < 0.85) {spin = true;}
                        break;
                    case 3:
                        if (percent > 0.1 && percent < 0.5) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if(percent > 0.6) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 4:
                        if(!repeatAtk && percent > 0.4f) {
                            repeatAtk = true;
                            R.set = true;
                        }
                        
                        if (percent > 0.5f) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 5:
                        if(!repeatAtk && percent > 0.2f) {
                            repeatAtk = true;
                            L.set = true;
                        }
                        if (percent > 0.4f) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 6:
                        if (percent < 0.7f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -2f * sp); spin = false;}
                        else if (percent > 0.8f) {Vel = Vector3.zero;}
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
