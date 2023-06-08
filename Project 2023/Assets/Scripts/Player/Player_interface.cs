using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Player_interface : MonoBehaviour
{
    public float MaxHP = 100;
    public float HP = 0;
    public float MaxStamina = 100;
    public float Stamina = 0;
    public float StaminaSp = 10f;
    public float iFrame = 0;
    public float toxicFrame = 0;
    public float healFrame = 0;
    public float actingFrame = 0;
    public float toxicDmg = 0;
    public bool dead = false;
    public bool UI = false;
    public Image Health;
    public Image Energy;

    public void takeDamage(float val,Vector3 pos)
    {
        if (iFrame > 0 || dead == true) return;
        //audioSource.PlayOneShot(audios[Random.Range(0,audios.Length)]);
        //GameObject blood = Instantiate(bloodEffect, pos, Quaternion.identity);
        //blood.GetComponent<ParticleSystem>().Play();
        iFrame = 1f;
        HP -= val;
    }
    public void costStamina(float val)
    {
        Stamina -= val;
    }
    
    public void Healing()
    {
        if(Stamina > 1)
            healFrame = 0.25f;
    }

    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
        Stamina = MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead == true) return;

        if(HP <= 0) dead = true;
        iFrame = Mathf.Max(iFrame - Time.deltaTime, 0);
        toxicFrame = Mathf.Max(toxicFrame - Time.deltaTime, 0);
        healFrame = Mathf.Max(healFrame - Time.deltaTime, 0);
        actingFrame = Mathf.Max(actingFrame - Time.deltaTime, 0);

        if(healFrame == 0){
            if(toxicFrame > 0){
                Stamina = Mathf.Min(Stamina + StaminaSp * 0.5f * Time.deltaTime, MaxStamina);
                HP -= 5 * Time.deltaTime;
            }
            else
                if(actingFrame == 0) Stamina = Mathf.Min(Stamina + StaminaSp * Time.deltaTime, MaxStamina);
        }else{
            if(toxicFrame > 0){
                HP += Mathf.Min(2f * Stamina, 5f) * Time.deltaTime;
                Stamina = Mathf.Max(Stamina - 4f * StaminaSp * Time.deltaTime, 0);
            }else{
                HP += 20f * Time.deltaTime;
                Stamina = Mathf.Max(Stamina - 2f * StaminaSp * Time.deltaTime, 0);
            }
        }

        HP = Mathf.Min(HP, MaxHP);
        Stamina = Mathf.Max(Stamina, 0);

        if(UI){
            Health.fillAmount = HP / MaxHP;
            Energy.fillAmount = Stamina / MaxStamina;
        }
    }
}
