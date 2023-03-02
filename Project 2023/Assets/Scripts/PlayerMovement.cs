using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerMovement : PortalTraveller
{
    public float gravity;
    public float moveSpeed;
    public float airSpeedMult;
    public float runSpeedMult;
    public static bool moving;
    public static bool running;

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

    float verticalInput;
    float horizontalInput;

    Vector3 gravityForce;
    Vector3 inputDirection;
    Vector3 moveDirection;
    Vector3 moveForce;
    Quaternion rotation;
    float colliderHeight;

    public static bool crouching;
    public static bool sliding;
    public float crouchSpeedMult;
    float crouchCD = 0.2f;
    float crouchStartTime;

    public static bool onWall;
    public float backIgnoreWallDegree;
    int wallSide;
    GameObject detectWall;
    GameObject ignoreWall;

    public static bool dashing;
    public float dashSpeed;
    public float dashTime;
    public float dashCD;
    bool canDash;
    float dashStartTime;
    Vector3 dashDirection;

    new Rigidbody rigidbody;
    Animator animator;
    CapsuleCollider capsuleCollider;

    Vector3 savePoint;
    Camera cam;
    public static bool isTransport;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        colliderHeight = capsuleCollider.height;
        jumpCount = 0;
        detectWall = null;
        ignoreWall = null;
        savePoint = transform.position;
        cam = Camera.main;
        //判斷現在傳送該物體是否需要改變相機，如果是玩家傳送就要改變main相機的位置，
        //非玩家傳送則不需要改變相機
        isPlayer = true;
        isTransport= false;
    }

    void Update()
    {
        DetectAround();
        Input();
        speedControl();
        Drag();
        AnimatorUpdate();
    }

    void FixedUpdate()
    {
        MovePlayer();
        Gravity();
    }

    void MovePlayer()
    {
        rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        if (dashing && Time.time < dashStartTime + dashTime)
        {
            moveForce = dashDirection * dashSpeed;
        }
        else
        {
            if (dashing)
            {
                dashing = false;
                Vector3 vel;
                if (onWall)
                    vel = Vector3.zero;
                else
                    vel = rotation * Vector3.forward * 5f;
                rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f) + vel;
            }
            inputDirection = Vector3.forward * verticalInput + Vector3.right * horizontalInput;
            moveDirection = rotation * inputDirection;
            moveDirection = moveDirection.normalized;
            moveForce = moveDirection * moveSpeed;
            if (running)
                moveForce *= runSpeedMult;
            if (crouching)
                moveForce *= crouchSpeedMult;
            if (!onGround)
                moveForce *= airSpeedMult;
        }
        if (!onWall)
            rigidbody.AddForce(moveForce, ForceMode.Force);
    }

    void Gravity()
    {
        if (!onWall)
        {
            gravityForce = Vector3.down * gravity;
            rigidbody.AddForce(gravityForce, ForceMode.Force);
        }
    }

    void Input()
    {
        verticalInput = InputManager.GetAxisRaw("Vertical");
        horizontalInput = InputManager.GetAxisRaw("Horizontal");

        if (verticalInput == 0 && horizontalInput == 0)
        {
            moving = false;
            running = false;
            if (sliding)
            {
                SlideReset();
            }
        }
        else
        {
            moving = true;
        }

        if (sameButtonRunDash)
        {
            if (InputManager.GetButtonDown("Run"))
            {
                dashPressTime = Time.time;
                if (onGround && moving && !crouching)
                    runPressTime = Time.time;
            }
            if (InputManager.GetButton("Run"))
            {
                if (onGround && moving && !crouching && runPressTime > 0 && Time.time >= runPressTime + buttonPressTime)
                {
                    running = !running;
                    if (sliding)
                    {
                        SlideReset();
                        Crouch();
                    }
                    runPressTime = -1f;
                    dashPressTime = -1f;
                }
            }
            if (InputManager.GetButtonUp("Dash") && dashPressTime > 0 && Time.time < dashPressTime + buttonPressTime)
            {
                if (Time.time > dashStartTime + dashCD && !topBlock && canDash)
                {
                    if (crouching)
                    {
                        CrouchReset();
                    }
                    if (sliding)
                    {
                        SlideReset();
                    }
                    Dash();
                }
            }
        }
        else
        {
            if (InputManager.GetButtonDown("Run") && onGround && moving && !crouching)
            {
                running = !running;
                if (sliding)
                {
                    SlideReset();
                    Crouch();
                }
            }

            if (InputManager.GetButtonDown("Dash"))
            {
                if (Time.time > dashStartTime + dashCD && !topBlock && canDash)
                {
                    if (crouching)
                    {
                        CrouchReset();
                    }
                    if (sliding)
                    {
                        SlideReset();
                    }
                    Dash();
                }
            }
        }

        if (InputManager.GetButtonDown("Jump"))
        {
            if (crouching)
            {
                CrouchReset();
            }
            if (sliding)
            {
                SlideReset();
            }
        }
        if (InputManager.GetButton("Jump") && !crouching && !sliding)
        {
            Jump();
        }
        if (InputManager.GetButtonUp("Jump"))
        {
            JumpReset();
        }

        if (InputManager.GetButtonDown("Crouch"))
        {
            if (onGround && Time.time > crouchStartTime + crouchCD)
            {
                if (crouching)
                {
                    CrouchReset();
                }
                else if (sliding)
                {
                    SlideReset();
                }
                else if (running)
                {
                    Slide();
                }
                else
                {
                    Crouch();
                }
            }
            else if (onWall)
            {
                ignoreWall = detectWall;
            }
        }

        if (InputManager.GetButtonDown("Restart"))
        {
            Restart();
        }

        if (onGround)
        {
            ignoreWall = null;
            if (!dashing)
            {
                canDash = true;
            }
        }
        else
        {
            if (crouching && Time.time > crouchStartTime + crouchCD)
            {
                CrouchReset();
            }
            if (sliding && Time.time > crouchStartTime + crouchCD)
            {
                SlideReset();
            }
        }
    }

    void DetectAround()
    {
        rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        onGround = LandscapeDetect();
        topBlock = TopDetect();
        onWall = WallDetect();
    }

    bool LandscapeDetect()
    {
        Vector3 pos;
        if (sliding)
        {
            float r, h;
            r = capsuleCollider.radius;
            h = colliderHeight / 2;
            for (int i = -1; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    pos = transform.position + rotation * new Vector3(i * r, 0.1f - r, -j * h);
                    if (Physics.Raycast(pos, Vector3.down, 0.2f, -1))
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            float x, z, r;
            r = capsuleCollider.radius;
            for (float degree = 0; degree < 2 * Mathf.PI; degree += Mathf.PI / 8)
            {
                x = r * Mathf.Sin(degree);
                z = r * Mathf.Cos(degree);
                pos = transform.position + new Vector3(x, 0.1f, z);
                if (Physics.Raycast(pos, Vector3.down, 0.2f, -1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool TopDetect()
    {
        if (crouching || sliding)
        {
            Vector3 pos;
            float x, z, r, l;
            r = capsuleCollider.radius;
            l = colliderHeight - 0.1f;
            if (sliding)
                l -= r;
            for (float degree = 0; degree < 2 * Mathf.PI; degree += Mathf.PI / 8)
            {
                x = r * Mathf.Sin(degree);
                z = r * Mathf.Cos(degree);
                pos = transform.position + new Vector3(x, 0.1f, z);
                RaycastHit hit;
                if (Physics.Raycast(pos, Vector3.up, out hit, l, -1))
                {
                    if (!hit.transform.gameObject.Equals(gameObject))
                        return true;
                }
            }
        }
        return false;
    }

    bool WallDetect()
    {
        if (!onGround)
        {
            RaycastHit hit;
            Vector3 pos, igPos;
            Quaternion igRot;
            float r, h;
            r = capsuleCollider.radius;
            h = colliderHeight / 2;
            foreach(int side in new int[]{ 1, -1})
            {
                igRot = Quaternion.Euler(0f, backIgnoreWallDegree * side, 0f);
                for (int i = -1; i < 2; i++)
                {
                    pos = rotation * new Vector3((r - 0.1f) * side, h + i * r, 0f);
                    igPos = igRot * pos;
                    Debug.DrawRay(transform.position + pos, transform.right * side * 0.2f, Color.green, 0.5f);
                    Debug.DrawRay(transform.position + igPos, igRot * transform.right * side * 0.2f, Color.red, 0.5f);
                    if (Physics.Raycast(transform.position + igPos, igRot * transform.right * side, 0.2f, -1))
                    {
                        wallSide = 0;
                        return false;
                    }
                    if (Physics.Raycast(transform.position + pos, transform.right * side, out hit, 0.2f, -1))
                    {
                        if (hit.transform.gameObject.Equals(ignoreWall))
                        {
                            wallSide = 0;
                            return false;
                        }
                        if (!onWall)
                            rigidbody.velocity = Vector3.zero;
                        detectWall = hit.transform.gameObject;
                        wallSide = side;
                        return true;
                    }
                }
            }
        }
        wallSide = 0;
        return false;
    }

    void AnimatorUpdate()
    {
        animator.SetBool("OnGround", onGround);
        animator.SetBool("Move", moving);
        animator.SetBool("Run", running);
        animator.SetBool("Jump", jumping);
    }

    void Drag()
    {
        if (onGround || dashing)
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
        if (jumpCount == 0 && (onGround || onWall))
        {
            Vector3 upForce = jumpForce * transform.up;
            jumping = true;
            if (onWall)
            {
                float forward = verticalInput == 0 ? 0f : 30f;
                upForce *= 0.5f;
                upForce = rotation * Quaternion.Euler(forward, 0f, wallSide * 45f) * upForce;
            }
            if (running)
                rigidbody.AddForce(1.2f * upForce, ForceMode.Impulse);
            else
                rigidbody.AddForce(upForce, ForceMode.Impulse);
            jumpCount++;
        }
        else if (jumpCount < 10 && jumping)
        {
            jumpCount++;
        }
        else if (jumpCount < 20 && jumping)
        {
            rigidbody.AddForce(0.05f * jumpForce * transform.up, ForceMode.Impulse);
            jumpCount++;
        }
        else
        {
            jumping = false;
        }
        if ((onGround || onWall) && !jumping)
        {
            Invoke("JumpReset", 0.005f);
        }
    }

    void JumpReset()
    {
        if (onGround || onWall)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        }
        jumpCount = 0;
        jumping = false;
    }

    void Crouch()
    {
        crouching = true;
        capsuleCollider.height = colliderHeight / 2;
        capsuleCollider.center = colliderHeight / 4 * Vector3.up;
        crouchStartTime = Time.time;
    }

    void Slide()
    {
        sliding = true;
        transform.rotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 0f);
        crouchStartTime = Time.time;
    }

    void CrouchReset()
    {
        if (!topBlock)
        {
            crouching = false;
            capsuleCollider.height = colliderHeight;
            capsuleCollider.center = colliderHeight / 2 * Vector3.up;
        }
    }

    void SlideReset()
    {
        if (!topBlock)
        {
            sliding = false;
            transform.rotation = rotation;
        }
    }

    void Dash()
    {
        Camera cam = Camera.main;
        ignoreWall = detectWall;
        canDash = false;
        dashing = true;
        running = true;
        Quaternion camDir = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
        if (inputDirection != Vector3.zero)
        {
            dashDirection = camDir * inputDirection;
        }
        else
        {
            dashDirection = camDir * Vector3.forward;
        }
        dashDirection = dashDirection.normalized;
        dashStartTime = Time.time;
    }

    void Restart()
    {
        topBlock = false;
        rigidbody.velocity = Vector3.zero;
        transform.position = savePoint;
        transform.rotation = Quaternion.identity;
        running = false;
        onWall = false;
        detectWall = null;
        ignoreWall = null;
        if (jumping)
        {
            JumpReset();
        }
        if (crouching)
        {
            CrouchReset();
        }
        if (sliding)
        {
            SlideReset();
        }
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
     /***PORTAL***********************************************************************************/
     /* old code
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        //改變位置 因為是玩家所以要改變相機位置與角度
        isTransport = true;
        //transform.position = pos;                        //改變傳送者的位置(無論現在傳送者是玩家或非玩家)
        //CASE : 玩家
        if(isPlayer){
            //因為玩家的視角切換是根據相機去切換的 所以要改變相機的角度

            Camera cam = Camera.main;
            Matrix4x4 camM  = toPortal.transform.localToWorldMatrix * fromPortal.transform.worldToLocalMatrix * cam.transform.localToWorldMatrix;
            float yRotation = cam.GetComponent<PlayerCam>().yRotation;
            //cam.transform.position = camM.GetColumn (3); //改變相機位置
            Vector3 eulerRot = camM.rotation.eulerAngles;
            float delta = Mathf.DeltaAngle(yRotation, eulerRot.y);
            cam.GetComponent<PlayerCam>().yRotation += delta;
            cam.transform.eulerAngles = Vector3.up * cam.GetComponent<PlayerCam>().yRotation;
            Debug.Log("玩家傳送" + transform.position);
            cam.transform.localPosition = transform.position  + transform.up * 1.9f;
            transform.position = pos;   
        }
        //CASE : 非玩家 (之後再加)
        else{
            
            Vector3 eulerRot = rot.eulerAngles; //轉換成歐拉角
            float delta = Mathf.DeltaAngle (yaw, eulerRot.y); //計算旋轉後跟yaw的角度差
            yaw += delta;
            smoothYaw += delta;
            transform.eulerAngles = Vector3.up * yaw;
            velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
            Physics.SyncTransforms (); //同步所有child
            
        }
    }
    ***************************************************************************************************/
    /*問題
    camera 跟 body兩個在PlayerCam.cs互相連動有一個計算的方式讓玩家帶相機移動，相機帶玩家轉動。
    但我為了要在傳送的當下同步這兩個我就不能等到PlayCam的code那邊去做，
    因為他那邊的運算是在update。但是傳送門處理是在LateUpdate 這樣等於我要去等到下一次Update才能進行。
    所以我自己在LateUpdate那邊計算好了，但是等到Update時PlayCam的code還會再算一次，而這兩個計算方式不同。
    感覺是因為這樣才有誤差上下跳動的關係。

    解決方法 :
    Step 1. 將計算量降低，矩陣相乘 
    Step 2. collider變超小，加寬portal 移動的距離 詳見Portal.cs 

    */
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot){// Transform travellerT,Camera linkedcamera) {
        //改變位置 因為是玩家所以要改變相機位置與角度
        isTransport = true;
        //transform.position = pos;                        //改變傳送者的位置(無論現在傳送者是玩家或非玩家)
        
            Vector3 eulerRot = rot.eulerAngles;
            float delta = Mathf.DeltaAngle(cam.GetComponent<PlayerCam>().yRotation, eulerRot.y);
            cam.GetComponent<PlayerCam>().yRotation += delta;
            //cam.transform.eulerAngles = Vector3.up * cam.GetComponent<PlayerCam>().yRotation;
                                      //先改變相機位置
        //CASE : 玩家
        if(isPlayer){
            //因為玩家的視角切換是根據相機去切換的 所以要改變相機的角度
            //linkedcamera.enabled = true;
            //linkedcamera.targetDisplay = 1;
            //Camera.main.enabled = false;
            //set portal's child camera
            /*
            Camera cam = Camera.main;
            Matrix4x4 camM  = m * cam.transform.localToWorldMatrix;
            Vector3 eulerRot = camM.rotation.eulerAngles;
            float delta = Mathf.DeltaAngle(cam.GetComponent<PlayerCam>().yRotation, eulerRot.y);
            cam.GetComponent<PlayerCam>().yRotation += delta;
            cam.transform.eulerAngles = Vector3.up * cam.GetComponent<PlayerCam>().yRotation;
            cam.transform.position = camM.GetColumn(3);                           //先改變相機位置
            transform.position = m * transform.localToWorldMatrix.GetColumn(3);   //再改變玩家位置
            Physics.SyncTransforms ();*/
            //Camera.main.enabled = true;
            //set portal's child camera
            //linkedcamera.enabled = false;
        }
        //CASE : 非玩家 (之後再加)
        else{
            /*
            Vector3 eulerRot = rot.eulerAngles; //轉換成歐拉角
            float delta = Mathf.DeltaAngle (yaw, eulerRot.y); //計算旋轉後跟yaw的角度差
            yaw += delta;
            smoothYaw += delta;
            transform.eulerAngles = Vector3.up * yaw;
            velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
            Physics.SyncTransforms (); //同步所有child
            */
        }
        isTransport = false;
    }
    /***PORTAL***********************************************************************************/
}
