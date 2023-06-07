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
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("critical")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("kick")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("strike1")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("strike2")) acting = 4;
            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1: //critical
                        if(Stamina >= 10) {if(!repeat)Stamina -= 20; atkState = 0; repeat = true;} 
                        break;
                    case 2: //kick
                        if(Stamina >= 20) {if(!repeat)Stamina -= 5; atkState = 1; repeat = true;}
                        break;
                    case 3: //strike1
                        if(Stamina >= 20) {if(!repeat)Stamina -= 10; atkState = 2; repeat = true;}
                        break;
                    case 4: //strike2
                        if(Stamina >= 20) {if(!repeat)Stamina -= 10; atkState = 3; repeat = true;}
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
                    case 1: //strile 
                        if (percent > 0.1 && percent < 0.3) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if(percent > 0.4) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 2: //kick ( 5->15 kick )  total : 30
                        if (percent > 0.16 && percent < 0.5)  {Vel = Vector3.zero; spin =  false;}
                        else {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        break;
                    case 3: //strike1 ( 10-32 strike1 )  total : 60 
                        if (percent < 0.2f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -2f * sp); spin = false;}
                        else if (percent > 0.5f) {Vel = Vector3.zero;}
                        break;
                    case 4: //strike2 ( 10-20 strike2 )  total : 70
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
