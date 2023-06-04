using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Continuos_damage : MonoBehaviour
{
    public int dmg = 3;
    public void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player") return;
        Player_interface p = other.GetComponentInParent<Player_interface>();
        p.toxicDmg = dmg;
        p.toxicFrame = 2f;
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
