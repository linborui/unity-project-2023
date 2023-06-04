using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; //攻擊距離
    public float skillRange;  //遠程距離
    public float coolDown;    
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier; //爆擊倍率
    public float criticalChance;     //爆擊機率
}
