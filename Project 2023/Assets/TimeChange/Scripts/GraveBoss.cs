using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveBoss : MonoBehaviour
{
    public MoveSnakeStatue snakeStatue;

    [Header("Enemy")]
    public float generateEnemy = 2;
    public GameObject enemyPrefab;
    public Transform CountEnemy;
    public float DistanceWithPlayer = 20;

    [Header("Time")]
    public float AttackPeriod = 10;
    public float BreakPeriod = 20;

   [Header("Grave")]
    public Animator Grave;
    public string Up = "Up";
    public string Down = "Down";
    public int maxHitTimes = 10;
    public int currentHitTimes = 0;

    private Vector3 generatePos;
    private float timer = 0f;
    public bool attack = false;
    private float enemyNum = 0;

    private void Start()
    {
        GeneratingEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        enemyNum = CountEnemy.childCount;

        if(!attack && timer > BreakPeriod)        // 超過休息時間 (可攻擊)
        {
            attack = true;
            Grave.Play(Up);             //升起
            timer = 0;
        }
        else if(attack && timer > AttackPeriod)           //超過攻擊時間(生成小怪)
        {
            attack = false;
            Grave.Play(Down);           //降落
            GeneratingEnemy();
            timer = 0;
        }

        if (currentHitTimes >= maxHitTimes)
        {
            snakeStatue.gravenum++;
            Destroy(this.gameObject);
        }
    }


    private void GeneratingEnemy() {
        if(enemyNum < generateEnemy)
        {
            for(int i=0; i<(generateEnemy - enemyNum); i++)
            {
                randomEnemy(i+1);
            }
        }
    }

    private void randomEnemy(int i)
    {
        Vector3 randomPosition = Random.insideUnitSphere * DistanceWithPlayer * i;
        randomPosition.y = 0;
        randomPosition.x = Mathf.Abs(randomPosition.x);
        randomPosition += this.transform.position;
        enemyPrefab.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);

        GameObject e = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        e.transform.SetParent(CountEnemy);
    }

}
