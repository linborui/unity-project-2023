using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSkeleton : AI
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
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("stab")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("sweap")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 3;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1: //stab
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 0; repeat = true;} 
                        break;
                    case 2: //sweap
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 1; repeat = true;}
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
                    case 1: //stab 
                        if (percent > 0.1 && percent < 0.3) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if(percent > 0.8) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 2: //sweap (frame 20 start sweap to fram 60)
                        if (percent > 0.3 && percent < 0.8)  {Vel = Vector3.zero; spin =  false;}
                        else {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        break;
                    case 3: //react get hit 
                        if (percent < 0.2f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -2f * sp); spin = false;}
                        else if (percent > 0.5f) {Vel = Vector3.zero;}
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
