using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDefeatVillage : MonoBehaviour
{
    private bool EnemyDead = false;
    private bool change = false;

    public AI enemy;
    public TimeShiftingController TimeShift;
    public GameObject AfterDefeat;
    public GameObject Fire;
    public Transform Buildings;

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

        if (change && TimeShift.PastBool == 0)
        {
            this.gameObject.tag = "IvyGenerateable";
            AfterDefeat.SetActive(true);
            Destroy(Fire);
            ChangeObjectLayer(Buildings, 0);
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

    private void ChangeObjectLayer(Transform parent, int targetLayer)
    {
        // 更改當前物件的層
        parent.gameObject.layer = targetLayer;

        // 遍歷所有子物件
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            child.gameObject.layer = targetLayer;
        }
    }

}
