using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using Luminosity.IO;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Player_agent : MonoBehaviour
{
    public Player_interface status;
    public AI enemy_status;
    public Agent_weapon weapon;
    public float moveSpeed;
    public float airSpeedMult;
    public float runSpeedMult;
    public float straightenSpeed;
    public static bool moving;
    public static bool running = true;

    public bool swinging;

    public bool sameButtonRunDash;
    public float buttonPressTime;
    float runPressTime;
    float dashPressTime;

    public float jumpForce;
    int jumpCount;
    public static bool jumping;

    public float groundDrag;
    public float playerHeight;
    public static bool onGround;

    public static bool topBlock;

    [Range(-1, 1)]
    public int verticalInput, horizontalInput;

    public int dash_direction;
    public bool jump;
    public bool dash;
    public bool run;

    Vector3 inputDirection;
    Vector3 moveDirection;
    Vector3 moveForce;
    Quaternion rotation;
    float colliderHeight;

    public static bool dashing;
    public float dashSpeed;
    public float dashTime;
    public float dashCD;

    public GameObject Aim;
    public Vector2 dis;
    protected float angle;
    protected float targetAngle;
    protected float angVel;
    protected float desicion_Delay;

    [Range(0, 1)]
    public float Intensity = 0.25f;
    [Range(0, 1)]
    public float Clamp = 0.03f;
    public bool Off = false;

    private RaycastHit forward, left, right, back;
    private MotionBlur mb;
    bool canDash;
    float dashStartTime;
    Vector3 dashDirection;

    new Rigidbody rigidbody;
    CapsuleCollider capsuleCollider;

    Vector3 savePoint;
    Camera cam;
    public static bool isTransport;
    // Add in Portal Level
    //private CharacterStates characterStates;
    // void Awake()
    // {
    //     characterStates = GetComponent<CharacterStates>();
    // }
    public void Reset(){
        status.HP = status.MaxHP;
        status.Stamina = status.MaxStamina;
        status.toxicFrame = 0;
        status.iFrame = 0;
        status.dead = false;
    
        rigidbody.velocity = Vector3.zero;
        running = true;
        jump = false;
        run = false;
        dash = false;
        
        canDash = true;
        dashing = false;
    }

    void Start()
    {
        //Add in Portal Level
        //GameManager.Instance.RegisterPlayer(characterStates);
        status = transform.GetComponent<Player_interface>();
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in objectsWithTag)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                Aim = enemy;
            }
        }

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        capsuleCollider = GetComponent<CapsuleCollider>();
        colliderHeight = capsuleCollider.height;
        jumpCount = 0;
        savePoint = transform.position;
        cam = Camera.main;

        status = transform.GetComponent<Player_interface>();
        enemy_status = Aim.GetComponent<AI>();
        weapon = transform.GetComponentInChildren<Agent_weapon>();
        //判斷現在傳送該物體是否需要改變相機，如果是玩家傳送就要改變main相機的位置，
        //非玩家傳送則不需要改變相機
        //isPlayer = true;
        //isTransport= false;
        //transform.GetComponentInChildren<atk_trigger_first>().dmg = 0;

        float mass = rigidbody.mass;
        moveSpeed *= mass;
        jumpForce *= mass;
        dashSpeed *= mass;
    }

    void Update()
    {
        Decide();
        DetectAround();
        Input();
        speedControl();
        Drag();
    }

    void FixedUpdate()
    {
        MovePlayer();
        Straighten();
    }

    void Decide()
    {
        if(Off) return;

        dis = new Vector2(Aim.transform.position.x - transform.position.x, Aim.transform.position.z - transform.position.z);
        desicion_Delay = Mathf.Max(desicion_Delay - Time.deltaTime, 0);
        float distance =  Mathf.Sqrt(dis.x * dis.x + dis.y * dis.y);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, out forward, 100, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.right, out left, 100, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.right, out right, 100, -1, QueryTriggerInteraction.Ignore);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.forward, out back, 100, -1, QueryTriggerInteraction.Ignore);

        Vector3 directionOfEnemy = transform.position - Aim.transform.position;
        directionOfEnemy.Normalize();

        float dotProduct = Vector3.Dot(directionOfEnemy, Aim.transform.forward);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
        //check enemy is facing player.

        weapon.IframeSet(status.iFrame);

        if(distance >= 10){
            run = true;
            verticalInput = 1;
            
            if(desicion_Delay == 0){
                if(enemy_status.state > 1){
                    desicion_Delay = Random.Range(0.2f, 1f);
                    if(left.distance < 2 && right.distance < 2)
                        dash_direction = 4;
                    else if(left.distance < 2)
                        dash_direction = Random.Range(1, 3);
                    else if(right.distance < 2)
                        dash_direction = Random.Range(5, 8);
                    else{
                        int side = Random.Range(0, 2);
                        if(side == 0)
                            dash_direction = Random.Range(1, 3);
                        else
                            dash_direction = Random.Range(5, 8);
                    } 
                    dash = true;
                }
            }
        }else if(distance >= 1.8 && distance < 10){
            if(desicion_Delay == 0){
                verticalInput = 1;
                run = false;
                desicion_Delay = Random.Range(0.2f, 1f);
                if(left.distance < 2 && right.distance < 2)
                    horizontalInput = 0;
                else if(left.distance < 2)
                    horizontalInput = 1;
                else if(right.distance < 2)
                    horizontalInput = -1;
                else
                    horizontalInput = Random.Range(-1, 2);

                if(enemy_status.state > 1){
                    verticalInput = 0;
                    horizontalInput = Random.Range(0, 2) == 0 ? 1 : -1;
                    if(status.Stamina >= 60){
                        if(left.distance < 2 && right.distance < 2)
                            dash_direction = 4;
                        else if(left.distance < 2)
                            dash_direction = Random.Range(1, 3);
                        else if(right.distance < 2)
                            dash_direction = Random.Range(5, 8);
                        else{
                            int side = Random.Range(0, 2);
                            if(side == 0)
                                dash_direction = Random.Range(1, 3);
                            else
                                dash_direction = Random.Range(5, 8);
                        }
                        dash = true;
                    }
                    run = true;
                }
            }
        }else{
            verticalInput = 0;
            if(desicion_Delay == 0){
                run = false;
                desicion_Delay = Random.Range(0.2f, 1f);
                if(left.distance < 2 && right.distance < 2)
                    horizontalInput = 0;
                else if(left.distance < 2)
                    horizontalInput = 1;
                else if(right.distance < 2)
                    horizontalInput = -1;
                else
                    horizontalInput = Random.Range(-1, 2);

                if(enemy_status.state > 1 || status.toxicFrame > Random.Range(0, 2)){
                    verticalInput = -1;
                    horizontalInput = Random.Range(0, 2) == 0 ? 1 : -1;
                    jump = Random.Range(0, 2) == 0 ? true : false;

                    if(status.Stamina >= 30){
                        if(left.distance < 2 && right.distance < 2)
                            dash_direction = 4;
                        else if(left.distance < 2)
                            dash_direction = Random.Range(2, 4);
                        else if(right.distance < 2)
                            dash_direction = Random.Range(4, 7);
                        else
                            dash_direction = Random.Range(2, 7);
                        dash = true;
                    }
                    run = true;
                }
            }
            weapon.Atk = true;
        }
    }

    void MovePlayer()
    {
        if (swinging) return;
        rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        if (dashing && Time.time < dashStartTime + dashTime)
            moveForce = dashDirection * dashSpeed;
        else
        {
            if (dashing)
            {
                Vector3 vel;
                
                vel = rotation * Vector3.forward * 5f;
                rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f) + vel;
                rigidbody.useGravity = true;
                dashing = false;
            }
            inputDirection = Vector3.forward * verticalInput + Vector3.right * horizontalInput;
            moveDirection = rotation * inputDirection;
            moveDirection = moveDirection.normalized;
            moveForce = moveDirection * moveSpeed;
            if (running)
                moveForce *= runSpeedMult;
            if (!onGround)
                moveForce *= airSpeedMult;
        }
        rigidbody.AddForce(moveForce, ForceMode.Force);
    }

    void Straighten()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, straightenSpeed * Time.deltaTime);
    }

    public void Facing()
    {
        targetAngle = Mathf.Atan2(dis.x, dis.y) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref angVel, 0.1f);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    void Input()
    {
        Facing();

        if (verticalInput == 0 && horizontalInput == 0)
            moving = false;
        else
            moving = true;

        if (run && onGround && moving)
            running = run;

        if (dash){
            if (Time.time > dashStartTime + dashCD && !topBlock && canDash)
                Dash(dash_direction);
            dash = false;
        }

        if (jump){
            Jump();
            jump = false;
        }

        if (onGround)
            if (!dashing)
                canDash = true;
    }

    void DetectAround()
    {
        rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        onGround = LandscapeDetect();
        
        rigidbody.useGravity = true;
    }

    bool LandscapeDetect()
    {
        Vector3 pos;
        float x, z, r;
        r = capsuleCollider.radius;
        for (float degree = 0; degree < 2 * Mathf.PI; degree += Mathf.PI / 8)
        {
            x = r * Mathf.Sin(degree);
            z = r * Mathf.Cos(degree);
            pos = transform.position + new Vector3(x, 0.1f, z);
            if (Physics.Raycast(pos, Vector3.down, 0.2f, -1))
                return true;
        }
        return false;
    }

    void Drag()
    {
        if (swinging)
            rigidbody.drag = 0;
        else if (onGround || dashing)
            rigidbody.drag = groundDrag;
        else
            rigidbody.drag = groundDrag * airSpeedMult;
    }

    void speedControl()
    {
        Vector3 curVel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (curVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = curVel.normalized * moveSpeed;
            rigidbody.velocity = new Vector3(limitVel.x, rigidbody.velocity.y, limitVel.z);
        }
    }

    void Jump()
    {
        if (onGround)
        {
            Vector3 upForce = jumpForce * transform.up;
            if (running)
                rigidbody.AddForce(1.2f * upForce, ForceMode.Impulse);
            else
                rigidbody.AddForce(upForce, ForceMode.Impulse);
        }
    }

    void Dash(int dir)
    {
        Camera cam = Camera.main;
        canDash = false;
        dashing = true;
        running = true;
        Quaternion Dir = Quaternion.Euler(0f, (dir * 45f) % 360, 0f);
        
        dashDirection = (transform.rotation * Dir * Vector3.forward).normalized;

        dashStartTime = Time.time;
        rigidbody.useGravity = false;
    }

    void Restart()
    {
        topBlock = false;
        rigidbody.velocity = Vector3.zero;
        transform.position = savePoint;
        transform.rotation = Quaternion.identity;
        running = true;
        swinging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Save"))
        {
            savePoint = other.transform.position;
        }
        else if (other.CompareTag("Dead"))
        {
            Restart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Restart();
        }
    }

    public Vector3 getDashDirection(){
        return dashDirection;
    }
    public void setDashDirection(Vector3 direst){
        dashDirection = direst;
    }
}