using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockman : AI
{
    public Cast_magic R;
    public float MaxPoise = 200;
    public float Poise = 200;
    public float PoiseSP = 10;
    bool repeatAtk = false;

    public override void takeDamage(float val,Vector3 pos)
    {
        if (iFrame > 0 || dodge == true || dead == true) return;

        GameObject blood = Instantiate(bloodEffect, pos, Quaternion.identity);
        blood.GetComponent<ParticleSystem>().Play();

        if(!awareness) {
            awareness = true;
            HP -= 2 * val;
        }else HP -= val;

        Poise -= val;

        if(Poise <= 0){
            Poise = MaxPoise;
            react = true;
        }
        iFrame = 0.5f;
    }

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
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("cast")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 4;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1: //roar
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 0; repeat = true;} /**no dodge**/
                        break;
                    case 2: //stab
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 1; repeat = true;}
                        break;
                    case 3: //cast
                        if(Stamina >= 20) {if(!repeat)Stamina -= 30; atkState = 2; repeat = true;}
                        break;
                }
            }

            iFrame = Mathf.Max(iFrame - Time.deltaTime, 0);
            
            if(acting == 0){ //movement
                spin = true;
                repeatAtk = false;
                
                Stamina = Mathf.Min(Stamina + StaminaSp * Time.deltaTime, MaxStamina);
                Poise = Mathf.Min(Poise + PoiseSP * Time.deltaTime, MaxPoise);
                Vel = Quaternion.LookRotation(transform.forward) * new Vector3(x * sp , 0, y * sp);
            }else{
                repeat = false;
                state = 0;
                atkState = -1;
                //movement
                switch (acting)
                {
                    case 1: //roar V
                        spin = false;
                        Vel = Vector3.zero;
                        break;
                    case 2: //stab V
                        if (percent > 0.1 && percent < 0.5) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if(percent > 0.6) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 3: //cast V
                        if(!repeatAtk && percent > 0.6f) {
                            repeatAtk = true;
                            R.set = true;
                        }
                        if (percent > 0.7f) {Vel = Vector3.zero; spin =  false;}
                        break;
                    case 4: //react get hit 
                        if (percent < 0.7f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -2f * sp); spin = false;}
                        else if (percent > 0.8f) {Vel = Vector3.zero;}
                        break;
                }
            }
        }
        //Debug.Log(acting + " " + Vel);
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
