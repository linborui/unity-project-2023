using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDefeatVillage : MonoBehaviour
{
    private bool EnemyDead = false;
    private bool change = false;

    public AI enemy;
    public GameObject AfterDefeat;
    public TimeShiftingController TimeShift;

    // Update is called once per frame
    void Update()
    {
        if(!EnemyDead)
            DetectEnemy();
        else
        {
            if(TimeShift.PastBool == 2)
            {
                change = true;
            }
        }

        if (change)
        {
            this.gameObject.tag = "IvyGenerateable";
            AfterDefeat.active = true;
            this.enabled = false;
        }
    }


    public void DetectEnemy()
    {
        if (enemy == null) return;

            if (enemy.dead)
            {
            EnemyDead = true;
            }

    }

}
