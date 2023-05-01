using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 30;
    public GameObject rockPrefab;
    public Transform handPos;
    //Animation Event
    public void KickOff()
    {
        //但有可能走到Player身邊時，Player已經離開了，所以要判斷一下
        if(attackTarget!=null && transform.isFacingTarget(attackTarget.transform))//判斷正前方的扇區
        {
            CharacterStates targetStates = attackTarget.GetComponent<CharacterStates>();
            //在產生傷害前要先決定要將敵人往哪個方向擊飛
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //driection.Normalize();
            //因為player沒有navMeshAgent
            //TODO: 看要不要做擊飛 rigidbody
            //targetStates.GetComponent<NavMeshAgent>().isStopped = true;
            //targetStates.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //根據個人喜好添加
            //targetStates.GetComponent<Animator>().SetTrigger("Dizzy");
            //產生傷害是這個函數最後要執行的
            targetStates.TakeDamage(characterStates,targetStates);
        }
    }
    //Animation Event
    public void ThrowRock()
    {
        if(attackTarget != null)
        {
            var rock = Instantiate(rockPrefab,handPos.position,Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
