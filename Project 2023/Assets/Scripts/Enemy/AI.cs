using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float MaxHP = 100;
    public float HP = 0;
    public float MaxStamina = 100;
    public float Stamina = 0;
    public float StaminaSp = 10f;
    public float sp = 1.5f;
    public float awareDis = 30f;
    [Range(-1, 1)]
    public float desx, desy;
    public float player_dis;
    public float smoothTime = 0.1f;
    public float percent = 0;
    public float iFrame = 0;
    public int state = 0;
    public int atkState = -1;
    public int acting = 0;
    public int deathWay = 2;
    public int dead_rand = 0;
    public bool awareness = false;
    public bool react = false;
    public bool dodge = false;
    public bool parry = false;
    public bool spin = true;
    public bool dead = false;
    public Vector2 dis;
    public Animator fsm;
    public CharacterController controller;
    public GameObject bloodEffect;
    public GameObject Aim;
    public RaycastHit forward, left, right, back;
    public float targetAngle;
    protected bool repeat = false;
    [Range(-1, 1)]
    protected float x, y;
    protected float xsmooth, ysmooth;
    protected float angle;
    protected float angVel;
    protected Vector3 Vel;
    protected List<Transform> weapons;
    protected List<Vector3> weapon_pos;
    protected List<Quaternion> weapon_rot;

    public void Reset(){
        HP = MaxHP;
        Stamina = MaxStamina;
            
        state = 0;
        atkState = -1;
        acting = 0;

        repeat = false;
        dodge = false;
        parry = false;
        react = false;
        dead = false;
        if(fsm){
            fsm.SetFloat("x", 0);
            fsm.SetFloat("y", 0);
            fsm.SetInteger("atk", 0);
            fsm.SetFloat("dead_rand", (float)dead_rand);
            fsm.SetBool("react", false);
            fsm.SetBool("dead", false);
            fsm.SetBool("dodge", false);
            fsm.SetBool("parry", false);

            fsm.Play("movement");
        }
    }

    public void takeDamage(float val,Vector3 pos)
    {
        if (iFrame > 0 || dodge == true || dead == true) return;
        //audioSource.PlayOneShot(audios[Random.Range(0,audios.Length)]);
        GameObject blood = Instantiate(bloodEffect, pos, Quaternion.identity);
        blood.GetComponent<ParticleSystem>().Play();
        react = true;
        iFrame = 1f;
        HP -= val;
    }

    public void Facing()
    {
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, out forward, 30, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.right, out left, 30, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.right, out right, 30, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.forward, out back, 30, -1, QueryTriggerInteraction.Ignore);

        // facing to player automative
        targetAngle = Mathf.Atan2(dis.x, dis.y) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref angVel, smoothTime);
        
        x = Mathf.SmoothDamp(x, desx, ref xsmooth, 0.2f);
        y = Mathf.SmoothDamp(y, desy, ref ysmooth, 0.2f);

        //Debug.Log("targetAngle: " + targetAngle + ", Angle: " + angle);
        if(spin == true) transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    
    public void Detect()
    {
        Vel =new Vector3(0, 0, 0);
        dis = new Vector2(Aim.transform.position.x - transform.position.x, Aim.transform.position.z - transform.position.z);
        percent = fsm.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        player_dis = Mathf.Sqrt(dis.x * dis.x + dis.y * dis.y);
        float angleDiff = Mathf.Abs(transform.eulerAngles.y - Vector3.SignedAngle(Aim.transform.position - transform.position, transform.forward, Vector3.up)) % 180;
        //Debug.Log("angleDiff: " + angleDiff);
        if (angleDiff < 60 && player_dis < awareDis)
            awareness = true;
        if (HP <= 0) {
            dead = true;
        }else if(HP <= 30){
            GetComponentInChildren<sliceable>().act = true;
        }
    }

    public bool IfDead()
    {
        if(dead == true){
            react = false;
            dodge = false;
            parry = false;
            state = 0;
            acting = 0;
            atkState = -1;
            
            //When testing you need to comment under forloop statement
            foreach (Transform child in weapons)
            {
                if(ObjectControl.controledObject == null || !child.Equals(ObjectControl.controledObject.transform)){
                    child.transform.SetParent(null);
                    
                    child.GetComponent<atk_trigger>().enabled = false;
                    child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }
            weapons.Clear();
            if(transform.GetComponent<life_time>() == null) this.gameObject.AddComponent<life_time>();

            SetAnimation();
            return true;
        }else{
            return false;
        }     
    }

    public void SetAnimation()
    {
        fsm.SetFloat("x", x);
        fsm.SetFloat("y", y);
        fsm.SetInteger("atk", atkState);
        fsm.SetFloat("dead_rand", (float)dead_rand);
        fsm.SetBool("react", react);
        fsm.SetBool("dead", dead);
        fsm.SetBool("dodge", dodge);
        fsm.SetBool("parry", parry);
        react = false;
    }

    // Start is called before the first frame update
    public void Start()
    {
        HP = MaxHP;
        Stamina = MaxStamina;
        dead_rand = Random.Range(0, deathWay);
        controller = GetComponent<CharacterController>();
        fsm = GetComponentInChildren<Animator>();
        weapons = new List<Transform>();

        //wait for recover weapon
        //weapon_pos = new List<Vector3>();
        //weapon_rot = new List<Quaternion>();

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in objectsWithTag)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                Aim = player;
            }
        }

        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in childs)
            if (child.CompareTag("Weapon")){
                child.tag = "Moveable";
                child.GetComponent<BoxCollider>().enabled = false;
                weapons.Add(child);
                //weapon_pos.Add(child.position);
                //weapon_rot.Add(child.rotation);
            }
    }
}