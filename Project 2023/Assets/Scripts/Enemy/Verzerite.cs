using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verzerite : AI
{
    public float MaxPoise = 100;
    public float Poise = 100;
    public float PoiseSP = 10;
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
            if(transform.GetComponent<life_time>() == null) this.gameObject.AddComponent<life_time>();
            return true;
        }else{
            return false;
        }     
    }
    
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
            x = 0;
            y = 0;
            desx = 0;
            desy = 0;
            acting = 14;
        }
        iFrame = 0.5f;
    }

    public override void SetAnimation()
    {
        fsm.SetFloat("x", x);
        fsm.SetFloat("y", y);
        fsm.SetFloat("dash_x", dash_x);
        fsm.SetFloat("dash_y", dash_y);
        fsm.SetInteger("atk", atkState);
        fsm.SetBool("react", react);
        fsm.SetBool("dead", dead);
        fsm.SetBool("dodge", dodge);
        fsm.SetBool("parry", parry);
        react = false; 
        parry = false; 
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
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk_combo0")) acting = 6;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk_combo1")) acting = 7;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("elbow0")) acting = 8;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("elbow1")) acting = 9;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("kick0")) acting = 10;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("kick1")) acting = 11;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("knee")) acting = 12;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("pommel")) acting = 13;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 14;

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
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 1; repeat = true;}
                        break;
                    case 4:
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 2; repeat = true;}
                        break;
                    case 5:
                        if(Stamina >= 30) {if(!repeat)Stamina -= 30; atkState = 3; repeat = true;}
                        break;
                    case 6:
                        if(Stamina >= 50) {if(!repeat)Stamina -= 50; atkState = 4; repeat = true;}
                        break;
                    case 7:
                        if(Stamina >= 45) {if(!repeat)Stamina -= 45; atkState = 5; repeat = true;}
                        break;
                    case 8:
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 6; repeat = true;}
                        break;
                    case 9:
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 7; repeat = true;}
                        break;
                    case 10:
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 8; repeat = true;}
                        break;
                    case 11:
                        if(Stamina >= 25) {if(!repeat)Stamina -= 25; atkState = 9; repeat = true;}
                        break;
                    case 12:
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 10; repeat = true;}
                        break;
                    case 13:
                        if(Stamina >= 10) {if(!repeat)Stamina -= 10; atkState = 11; repeat = true;}
                        break;
                }
            }

            iFrame = Mathf.Max(iFrame - Time.deltaTime, 0);
            
            if(acting == 0){
                spin = true;
                repeatAtk = false;
                
                Stamina = Mathf.Min(Stamina + StaminaSp * Time.deltaTime, MaxStamina);
                Poise = Mathf.Min(Poise + PoiseSP * Time.deltaTime, MaxPoise);
                Vel = Quaternion.LookRotation(transform.forward) * new Vector3(x * sp , 0, y * sp);
            }else{
                state = 0;
                repeat = false;
                atkState = -1;
                //movement
                switch (acting)
                {
                    case 1:
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent > 0.2 && percent < 0.7) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(2 * sp * dash_x , 0, 2 * sp * dash_y);}
                        else if(percent > 0.7) dodge = false;
                        break;
                    case 2:
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent > 0.2 && percent < 0.64) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        break;
                    case 3:
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent < 0.5) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 2f * sp); spin = true;}
                        break;
                    case 4:
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent < 0.2) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        else if(percent >= 0.25 && percent < 0.38) {spin = true;}
                        else if(percent >= 0.38 && percent < 0.5) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 3f * sp);}
                        else if(percent >= 0.5 && percent < 0.7) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        break;
                    case 5:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.24f) {spin = true;}
                        else if (percent < 0.6f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        break;
                    case 6:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.2f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 0 * sp);}
                        else if (percent >= 0.3f && percent < 0.42f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        else if (percent >= 0.42f && percent < 0.62f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if (percent >= 0.65f && percent < 0.82f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        break;
                    case 7:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.16f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 0 * sp); spin = true;}
                        else if (percent >= 0.16f && percent < 0.3f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp);}
                        else if (percent >= 0.3f && percent < 0.4f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        else if (percent >= 0.48f && percent < 0.7f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, sp); spin = true;}
                        break;
                    case 8:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.4f) {spin = true;}
                        break;
                    case 9:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent >= 0.27f && percent < 0.57f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 1.5f * sp);}
                        break;
                    case 10:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.4f) {spin = true;}
                        break;
                    case 11:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.35f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(-sp, 0, 0); spin = true;}
                        else if (percent >= 0.5f && percent < 0.7f) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 1.5f * sp);}
                        break;
                    case 12:
                        Vel = Vector3.zero;
                        spin =  false;
                        if (percent < 0.53f) {spin = true;}
                        break;
                    case 13:
                        Vel = Vector3.zero;
                        spin =  false;
                        break;
                    case 14:
                        Vel = Vector3.zero;
                        spin =  false;
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
