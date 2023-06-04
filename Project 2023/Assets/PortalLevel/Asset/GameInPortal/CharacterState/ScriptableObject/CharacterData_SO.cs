using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Satas Info")] //基本的狀態
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
}
