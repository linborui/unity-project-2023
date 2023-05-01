using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    
    //public event Action<int,int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]       //在其他代碼中可以訪問，但不希望在inspector窗口中出現
    public bool isCritical; //判斷是否爆擊，其他代碼可以訪問，但不要在窗口出現

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
    #region Read from Data_SO

    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStates attacker,CharacterStates defender) //attacker 攻擊defender
    {
        //攻擊者的攻擊力減去防禦者的防禦力才是最後的傷害值，要嘛是實際傷害，要嘛是0傷害
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence,0); //考慮到防禦力比攻擊力還要大的狀況
        CurrentHealth = Mathf.Max(CurrentHealth - damage,0); //CurrentHealth private 直接獲得characterData裡面的值
        if(attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO: Update UI (血量面板)
        //UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        //TODO: 經驗update
    }
    public void TakeDamage(int damage,CharacterStates defender)
    {
        int currentDamage = Mathf.Max(damage-defender.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage,0);
        //UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
    }
    private int CurrentDamage()
    {
        //從最小傷害跟最大傷害中隨機取一個數字
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage); 
        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier; //傷害乘以爆擊倍率
            Debug.Log("爆擊 " + coreDamage + "血量" + CurrentHealth);
        }
        return (int)coreDamage;
    }
    #endregion
}
