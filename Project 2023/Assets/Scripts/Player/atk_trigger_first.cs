using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atk_trigger_first : MonoBehaviour
{
    public bool isEnemy = false;
    public float dmg = 30f;

    public void OnTriggerEnter(Collider other)
    {
        if(dmg == 0 && other.tag == "Weapon") return;
        if (!isEnemy && other.GetComponentInParent<AI>())
        {
            Vector3 point = other.ClosestPoint(transform.position);
            if(other.GetComponentInParent<Rockman>() != null) 
                other.GetComponentInParent<AI>().takeDamage(100 * dmg,point);
            else
                other.GetComponentInParent<AI>().takeDamage(dmg,point);
        }
        else if (isEnemy && other.GetComponentInParent<Player_interface>())
        {
            Vector3 point = other.ClosestPoint(transform.position);
            other.GetComponentInParent<Player_interface>().takeDamage(dmg, point);
        }
    }

}
