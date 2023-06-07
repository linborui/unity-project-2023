using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class observer : MonoBehaviour
{
    public GameObject judge, ai;
    public Player_interface judge_status;
    public AI ai_status;
    public Training_scripts scripts;
    public float period = 60f;
    private float Timer = 0f;
    private int DoublePoint = 0;
    public float ai_hp = 0f, judge_hp = 0f;
    public int range = 19;
    public float face = 0f;
    // Start is called before the first frame update

    void Start()
    {
        scripts = ai.GetComponent<Training_scripts>();
        ai_status = ai.GetComponent<AI>();
        judge_status = judge.GetComponent<Player_interface>();
        ai_hp = ai_status.MaxHP;
        judge_hp = judge_status.MaxHP;
        Timer = period;
        Randomnize();
    }

    void Randomnize()
    {
        CharacterController controller = ai.transform.GetComponent<CharacterController>();

        float dis = 1000;
        transform.position = new Vector3(Random.Range(-dis, dis + 1), Random.Range(-dis, dis + 1), Random.Range(-dis, dis + 1));
        transform.rotation = Quaternion.Euler(0, Random.Range(-180, 181), 0);

        controller.enabled = false;
        ai.transform.localPosition = new Vector3( Random.Range( -range, range + 1), 3, Random.Range( -range, range + 1));
        ai.transform.rotation = Quaternion.Euler(0, Random.Range( -180, 181), 0);
        controller.enabled = true;
        judge.transform.localPosition = new Vector3( Random.Range( -range, range + 1), 3, Random.Range( -range, range + 1));
        judge.transform.rotation = Quaternion.Euler(0, Random.Range( -180, 181), 0);
    }

    // Update is called once per frame
    void Update()
    {
        ai_hp = ai_status.HP;

        if(judge_status.HP < judge_hp){
            float point = judge_hp - judge_status.HP;
            scripts.SetPoint((int)point);
            judge_hp = judge_status.HP;
        }

        Timer = Mathf.Max(Timer - Time.deltaTime, 0);
        if(ai_status.HP <= 0 || judge_status.HP <= 0 || Timer == 0) {
            if(ai_status.HP <= 0) scripts.SetPoint(-50);

            ai_hp = ai_status.MaxHP;
            judge_hp = judge_status.MaxHP;
            scripts.Reset = true;

            Timer = period;
            Randomnize();
        }
    }
}
