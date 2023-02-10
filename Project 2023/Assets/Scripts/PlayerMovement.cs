using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerMovement : MonoBehaviour
{
    public float gravity;
    public float moveSpeed;
    public float airSpeedMult;
    public float runSpeedMult;
    public static bool moving;
    public static bool running;

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
    int wallSide;
    bool detectWall;

    public static bool dashing;
    public float dashSpeed;
    public float dashTime;
    public float dashCD;
    bool canDash;
    float dashStartTime;
    Quaternion dashRotation;

    new Rigidbody rigidbody;
    Animator animator;
    CapsuleCollider capsuleCollider;

    Vector3 savePoint;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        colliderHeight = capsuleCollider.height;
        jumpCount = 0;
        detectWall = true;
        savePoint = transform.position;
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
            moveForce = dashRotation.normalized * Vector3.forward;
            moveForce *= dashSpeed;
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
            moveDirection = Vector3.forward * verticalInput + Vector3.right * horizontalInput;
            moveDirection = rotation * moveDirection;
            moveForce = moveDirection.normalized * moveSpeed;
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

        if (InputManager.GetButtonDown("Run") && onGround && moving && !crouching)
        {
            running = !running;
            if (sliding)
            {
                SlideReset();
                Crouch();
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
                detectWall = false;
            }
        }

        if (InputManager.GetButtonDown("Dash"))
        {
            if (Time.time> dashStartTime + dashCD && !onWall && !topBlock && canDash)
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

        if (InputManager.GetButtonDown("Restart"))
        {
            Restart();
        }

        if (onGround)
        {
            detectWall = true;
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
                    if (hit.transform.gameObject != gameObject)
                        return true;
                }
            }
        }
        return false;
    }

    bool WallDetect()
    {
        if (detectWall && !onGround)
        {
            Vector3 pos;
            float r, h;
            r = capsuleCollider.radius;
            h = colliderHeight / 2;
            for (int i = -1; i < 2; i++)
            {
                pos = transform.position + rotation * new Vector3(r - 0.1f, h + i * r, 0f);
                if (Physics.Raycast(pos, transform.right, 0.2f, -1))
                {
                    if (!onWall)
                        rigidbody.velocity = Vector3.zero;
                    wallSide = 1;
                    return true;
                }
            }
            for (int i = -1; i < 2; i++)
            {
                pos = transform.position + rotation * new Vector3(0.1f - r, h + i * r, 0f);
                if (Physics.Raycast(pos, -transform.right, 0.2f, -1))
                {
                    if (!onWall)
                        rigidbody.velocity = Vector3.zero;
                    wallSide = -1;
                    return true;
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
        canDash = false;
        dashing = true;
        dashRotation = rotation;
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
}
