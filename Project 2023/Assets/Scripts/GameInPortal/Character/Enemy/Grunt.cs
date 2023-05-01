using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 15;
    public void KickOff(){ 
        if(attackTarget != null){ //不為空才會執行這個方法
            transform.LookAt(attackTarget.transform);
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            //因為player沒有navMeshAgent
            //TODO: 看要不要做擊飛
            //attackTarget.GetComponent<NavMeshAgent>().isStopped = true; //可能穿越到他的身邊，或距離他太近
            //attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce; //給一個力把玩家擊飛
            //attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
