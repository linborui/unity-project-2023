using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OX : AI
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

            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");

            Transform[] childs = transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in childs)
                if (child.CompareTag("Bones")){
                    child.transform.SetParent(null);
                    child.gameObject.AddComponent<life_time>();
                    child.gameObject.AddComponent<sliceable>();
                }
            foreach (Transform child in childs)
                if (child.CompareTag("Bones")){
                    Mesh bone = new Mesh();
                    child.GetComponent<SkinnedMeshRenderer>().BakeMesh(bone);
                    child.GetComponent<SkinnedMeshRenderer>().sharedMesh = bone;
                    child.GetComponent<MeshCollider>().sharedMesh = bone;
                    child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    child.GetComponent<Rigidbody>().AddForce((transform.position - Aim.transform.position).normalized * 5, ForceMode.Impulse);
            }
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
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk1")) acting = 1;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk2")) acting = 2;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk3")) acting = 3;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk4_kick")) acting = 4;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("atk5_kick")) acting = 5;
            else if(fsm.GetCurrentAnimatorStateInfo(0).IsName("react")) acting = 6;

            if(state != acting && acting == 0){
                switch (state)
                {
                    case 1:
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 0; repeat = true;}
                        break;
                    case 2:
                        if(Stamina >= 15) {if(!repeat)Stamina -= 15; atkState = 1; repeat = true;}
                        break;
                    case 3:
                        if(Stamina >= 35) {if(!repeat)Stamina -= 35; atkState = 2; repeat = true;}
                        break;
                    case 4:
                        if(Stamina >= 25) {if(!repeat)Stamina -= 25; atkState = 3; repeat = true;}
                        break;
                    case 5:
                        if(Stamina >= 20) {if(!repeat)Stamina -= 20; atkState = 4; repeat = true;}
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
                    case 1:  // ( side swing ) 6/37 (0.16%)  - 16/37 (0.43%)
                        spin = false;
                        Vel = Vector3.zero;
                        break;
                    case 2:  // ( right down to leftup ) 8/43 (0.18%) - 17/43 (0.4%)
                        spin = false;
                        Vel = Vector3.zero;
                        if (percent > 0.5 ) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, -sp); spin = false;}
                        break;
                    case 3:  // ( front cut ) 正面劈 10/46 (0.21%) - 24/46 (0.52%)
                        if (percent > 0.2 && percent < 0.5){ Vel = Vector3.zero; spin = true;}
                        else if (percent > 0.5 ) {Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 0, 0.5f * sp); spin = true;}
                        break;
                    case 4:  // (左腳踢) 16/52 (0.30%) - 32/52 (0.615)
                        if(percent > 0.3 && percent < 0.6) {Vel = Vector3.zero; spin = false;}
                        else if(percent > 0.65){Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 3, 2 * sp); spin = true;}
                        break;
                    case 5:  // (右腳踢) 4/38 (0.1%) - 19/38 (0.5)
                        if(percent > 0.1 && percent < 0.5) {Vel = Vector3.zero; spin = false;}
                        else if(percent > 0.5){Vel = Quaternion.LookRotation(transform.forward) * new Vector3(0 , 3, 2 * sp); spin = true;}
                        break;
                }
            }
        }
        SetAnimation();
        controller.Move((Vel + new Vector3(0, -9.8f, 0)) * Time.deltaTime);
    }
}
