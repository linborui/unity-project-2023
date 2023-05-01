using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD };
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    protected CharacterStates characterStates; //子類可訪問數據
    [Header("Basic Settings")]
    public float sightRadius;
    protected GameObject attackTarget;
    public bool isGuard;              //判斷是不是站裝敵人
    public float speed;               //紀錄原有的速度，原先都慢慢走，見到玩家走速追擊
    public float lookAtTime;          //到定點會停下來看的時間
    private float remainLookAtTime;   //跟看的時間做比較
    private float lastAttackTime;
    private Quaternion guardRotation; //紀錄本身旋轉的角度，Guard時會用到
    [Header("Patrol State")]          //enemy 為partrol的參數
    public float PatrolRange;
    private Vector3 wayPoint;         //路上的一個點
    private Vector3 guardPos;         //初始守衛座標位置
    //bool 配合動畫
    bool isWalk;
    bool isChase;
    bool isDead;
    bool isFollow;
    //判斷player是否死了
    bool playerDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        coll = GetComponent<Collider>();

        speed = agent.speed;                //speed 等於 agent本來的speed
        guardPos = transform.position;      //一開始拿到出場的座標
        guardRotation = transform.rotation; //初始化旋轉角度
        remainLookAtTime = lookAtTime;
        playerDead = false;
    }
    void Start(){
        //FIXME:場景切換後，修改掉
        GameManager.Instance.AddObserver(this);//this 因為已經掛載此街口
        if(isGuard)
            enemyStates = EnemyStates.GUARD;
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint(); //一開始就要設一個點
        }
    }
    /* 切換場景時啟用
    void OnEnable(){ //啟用時調用
        Debug.Log("HI");
        GameManager.Instance.AddObserver(this);//this 因為已經掛載此街口
    }
    */
    void OnDisable(){//銷毀時調用
        if(!GameManager.isInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    #region Updatestates
    void Update()
    {
        if(characterStates.CurrentHealth == 0) isDead = true;
        if(!playerDead){
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime; //人物在任何狀態都應該小於Cooldown的時間
        }
    }
    void SwitchAnimation(){ //切換動畫
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow",isFollow);
        anim.SetBool("Critical",characterStates.isCritical);
        anim.SetBool("Dead",isDead);
    }

    void SwitchStates(){
        if(isDead)             enemyStates = EnemyStates.DEAD;
        else if(FoundPlayer()) enemyStates = EnemyStates.CHASE; //如果發現player 切換到CHASE的狀態
        
        switch(enemyStates){
            case EnemyStates.GUARD:
                Guard();
                break;
            case EnemyStates.PATROL:
                Patrol();
                break;
            case EnemyStates.CHASE:
                Chase();
                break;
            case EnemyStates.DEAD:
                Dead();
                break;
        }
    }
    #endregion
    #region 4 states
    void Guard(){
        isChase = false; //回到站裝追擊狀態變false 配合動畫
        if(transform.position != guardPos)
        {
            isWalk = true; //移動動畫
            agent.isStopped = false; //可移動座標位置
            agent.destination = guardPos;
            if(Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance) //判斷是否到達目的地
            {
                isWalk = false;  //歸位，chase中不用在執行了
                transform.rotation = Quaternion.Lerp(transform.rotation , guardRotation, 0.01f); //將旋轉角度緩慢歸位
            }
        }
    }
    void Patrol(){
        isChase = false;            // 進入正常的移動狀態
        agent.speed = speed * 0.5f; // 守衛模式下速度減半
        //判斷是否到了隨機巡邏點
        if(Vector3.Distance(wayPoint,transform.position)<= agent.stoppingDistance ) //性能消耗較大
        {
            isWalk = false;
            if(remainLookAtTime > 0) remainLookAtTime -= Time.deltaTime; //還需看一會兒
            else GetNewWayPoint();  //不用看了
        }
        else //沒到給定的點
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }
    void Chase(){
        //配合動畫
        isWalk = false;
        isChase = true;
        agent.speed = speed; //追擊模式下，速度變回原本的速度
        if(!FoundPlayer()){  //沒找到敵人，拉拖回到上一個狀態
            isFollow = false; 
            if(remainLookAtTime > 0) //先保持原位不動，看一會兒
            {
                agent.destination = transform.position; //一拖戰就會回來
                remainLookAtTime -= Time.deltaTime;
            }
            //回到原狀態
            else if(isGuard) enemyStates = EnemyStates.GUARD; //如果原有狀態為Guard 回復到guard狀態
            else             enemyStates = EnemyStates.PATROL;
        }
        else
        {
            agent.destination = attackTarget.transform.position;
            isFollow = true;
            agent.isStopped = false; //找不到player時，就可以繼續walk了
        }
        //在攻擊範圍內 or 在遠程攻擊範圍內 則攻擊
        if(TargetInAttackRange() || TargetInSkillTange())
        {
            isFollow = false;
            agent.isStopped = true; //停止狀態 之後還要修復
            if(lastAttackTime < 0 )
            {
                lastAttackTime = characterStates.attackData.coolDown; //將計時器歸零
                //爆擊判斷 透過隨機爆擊率
                characterStates.isCritical = Random.value<characterStates.attackData.criticalChance; //隨機值是否小於爆擊率
                Attack(); //執行攻擊
            }
        }
    }
    void Dead(){
        coll.enabled = false;
        //agent.enabled = false;
        agent.radius = 0;
        Destroy(gameObject,2f);
    }
    #endregion
  

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange()) //近身攻擊動畫
            anim.SetTrigger("Attack");
        if(TargetInSkillTange())  //技能攻擊動畫
            anim.SetTrigger("Skill");
    }

    bool FoundPlayer(){ //找到玩家
        
        var colliders = Physics.OverlapSphere(transform.position,sightRadius); //物理球體範圍是否有碰撞體
        foreach(var target in colliders){
            if(target.CompareTag("Player")){
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if(attackTarget!=null)
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStates.attackData.attackRange;
        else
            return false;
    }
    bool TargetInSkillTange()
    {
        if(attackTarget!=null)
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStates.attackData.skillRange;
        else
            return false;
    }
    void GetNewWayPoint(){ //隨機生成一個Vector3的點
        remainLookAtTime = lookAtTime; //恢復時間，過了觀察的時間
        float randomX = Random.Range(-PatrolRange,PatrolRange);
        float randomZ = Random.Range(-PatrolRange,PatrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y,guardPos.z + randomZ);
        
        NavMeshHit hit;
        //先判斷點是不是walkable
        wayPoint = NavMesh.SamplePosition(randomPoint,out hit,PatrolRange,1)? hit.position:transform.position;
        //如果為false，上面的判斷會讓他繼續找一個新的wayPoint
        
        wayPoint = randomPoint;
        agent.destination = wayPoint;
    }
    void OnDrawGizmosSelected() // 方便觀察
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }
    //Animation Event
    void Hit() //敵人是被動的產生攻擊，走到人物的身邊就攻擊
    {
        //但有可能走到Player身邊時，Player已經離開了，所以要判斷一下
        if(attackTarget!=null && transform.isFacingTarget(attackTarget.transform))//判斷正前方的扇區
        {
            CharacterStates targetStates = attackTarget.GetComponent<CharacterStates>();
            targetStates.TakeDamage(characterStates,targetStates);
        }
    }
    public void EndNotify()
    {
        //獲勝動畫
        anim.SetBool("Win", true);
        playerDead = true;
        //停止所有移動
        //停止所有agent
        isChase= false;
        isWalk = false;
        attackTarget = null; //達到停止agent的方法，不會再找尋目標，設置移動位置，或者設置攻擊目標

    }
}
