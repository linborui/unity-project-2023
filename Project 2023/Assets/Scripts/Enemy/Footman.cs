using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footman : AI
{
    //public Cast_magic R;
    bool repeatAtk = false;

    // Update is called once per frame
    void Update()
    {
        if(IfDead()) return;
        Detect();

        if(awareness == true) {
            Facing();

            if(fsm.GetCurrentAnimatorStateInfo(0).IsName("movement")) acting = 0;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("dodge")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("stab")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("sweap")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 4;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1: //dodge 
                        Vel = Vector3.zero;
                        if (percent > 0.2 && percent < 0.7) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -sp); spin = false;}
                        else if(percent > 0.7) dodge = false;
                        break;
                    case 2: //stab
                        if(Stamina >= 10) {if(!repeat)Stamina -= 30; atkState = 0; repeat = true;} 
                        break;
                    case 3: //sweap
                        if(Stamina >= 20) {if(!repeat)Stamina -= 10; atkState = 1; repeat = true;}
                        break;
                }
            }

            iFrame = Mathf.Max(iFrame - Time.deltaTime, 0);
            
            if(acting == 0){ //movement
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
                    case 1: // dodge
                        Vel = Vector3.zero;
                        spin =  false;
                        break;
                    case 2: //stab  8/40 32/40
                        if (percent > 0.1 && percent < 0.4) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if(percent > 0.8) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 3: //sweap (frame 10/40 start sweap to fram 30/40)
                        if (percent > 0.25 && percent < 0.75)  {Vel = Vector3.zero; spin =  false;}
                        else {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        break;
                    case 4: //react get hit 20/40
                        if (percent < 0.5f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -2f * sp); spin = false;}
                        else if (percent > 0.5f) {Vel = Vector3.zero;}
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
