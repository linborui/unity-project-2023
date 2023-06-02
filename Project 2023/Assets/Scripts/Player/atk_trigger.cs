using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atk_trigger : MonoBehaviour
{
    public bool isEnemy = false;
    public float dmg = 30f;

    public void OnTriggerExit(Collider other)
    {
        if(dmg == 0) return;
        if (!isEnemy && other.GetComponentInParent<AI>())
        {
            Vector3 point = other.ClosestPoint(transform.position);
            other.GetComponentInParent<AI>().takeDamage(dmg,point);
        }
        else if (isEnemy && other.GetComponentInParent<Player_interface>())
        {
            Vector3 point = other.ClosestPoint(transform.position);
            other.GetComponentInParent<Player_interface>().takeDamage(dmg, point);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
