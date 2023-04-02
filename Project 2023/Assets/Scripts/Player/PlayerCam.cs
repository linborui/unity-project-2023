using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using System;
using UnityEngine.Rendering;
public class PlayerCam : MonoBehaviour
{
    public int state;
    public float xSensitivity;
    public float ySensitivity;
    public float heightSpeed;
    public float distanceSpeed;

    public GameObject crosshair;

    float cameraHeight;
    float cameraDistance;
    float currHeight;
    float currDistance;

    public GameObject body;
    Transform tf;

    public float xRotation; //pitch set public for transformer to handle portal 葉惟欣 
    public float yRotation; //yaw set public for transformer to handle portal 葉惟欣 給portal access

    /* for marching cube*/


    Camera cam;
    Vector3 _hitPoint;
    Vector3 _hitPrevPoint;
    public float BrushSize = 2f;
    public GameObject laser;
    public Laser laserObject;
    /* for marching cube*/
    void Start()
    {
        laser.SetActive(false);
        cam = GetComponent<Camera>();
        tf = body.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraHeight = body.GetComponent<CapsuleCollider>().height - 0.1f;
        if (state == 0)
        {
            cameraDistance = 2.0f;
        }
        else if (state == 1)
        {
            cameraDistance = 0;//0.3f;
        }
        currHeight = cameraHeight;
        currDistance = cameraDistance;
    }

    Portal[] portals;

    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
        RenderPipelineManager.beginCameraRendering += RenderPortal;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= RenderPortal;
    }

    void RenderPortal(ScriptableRenderContext context, Camera camera)
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender(context);
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render(context);
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender(context);
        }

    }

    void Update()
    {
        MouseLockState();
        MouseControl();
        MoveCam();
        MoveCrosshair();
    }

    void MouseLockState()
    {
        if (!InputManager.GetButton("Slash"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    void MouseControl()
    {
        if (!InputManager.GetButton("Slash") && !PlayerMovement.isTransport)
        {
            float deltaTime = (Time.deltaTime < 0.1f) ? Time.deltaTime : 0.1f;
            float mouseX = InputManager.GetAxisRaw("Mouse X") * deltaTime * xSensitivity;
            float mouseY = InputManager.GetAxisRaw("Mouse Y") * deltaTime * ySensitivity;
            float xMax = PlayerMovement.sliding ? 15f : 75f;

            xRotation -= mouseY; //(pitch)
            xRotation = Mathf.Clamp(xRotation, -60f, xMax);
            yRotation += mouseX; //(yaw)

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f); //相機的rotation
            if (!PlayerMovement.onWall)
                tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, yRotation, tf.rotation.eulerAngles.z); //人物的rotation
        }
        else
        {
            Physics.SyncTransforms();
        }
    }

    void MoveCam()
    {
        Action<float, float> UpdateCurrLoc = (distance, height) =>
        {

            float dis = distanceSpeed * Time.deltaTime;
            float hei = heightSpeed * Time.deltaTime;
            if (currDistance < distance)
            {
                currDistance += dis;
                if (currDistance > distance)
                    currDistance = distance;
            }
            else if (currDistance > distance)
            {
                currDistance -= dis;
                if (currDistance < distance)
                    currDistance = distance;
            }
            if (currHeight < height)
            {
                currHeight += hei;
                if (currHeight > height)
                    currHeight = height;
            }
            else if (currHeight > height)
            {
                currHeight -= hei;
                if (currHeight < height)
                    currHeight = height;
            }
        };

        if (state == 0) //第三人稱
        {
            float distance = cameraDistance;
            float height = cameraHeight;
            if (PlayerMovement.sliding)
            {
                distance += body.GetComponent<CapsuleCollider>().height;
                height *= 0.5f;
            }
            else if (PlayerMovement.crouching)
            {
                height *= 0.5f;
            }
            UpdateCurrLoc(distance, height);
            transform.localPosition = tf.localPosition - transform.forward * currDistance + Vector3.up * currHeight;
        }
        else if (state == 1) //第一人稱
        {
            //只有在未傳送狀態下才會在這裡移動相機(Update)，其他都在PlayerMoverment.cs transport傳送(雖然那裏是LateUpdate)
            if (!PlayerMovement.isTransport)
            {
                float distance = cameraDistance;
                cameraHeight = body.GetComponent<CapsuleCollider>().height - 0.1f;

                if (PlayerMovement.sliding)
                {
                    distance *= 2f;
                }
                UpdateCurrLoc(distance, cameraHeight);
                transform.localPosition = tf.localPosition + tf.forward * currDistance + tf.up * currHeight;
            }
            else
            {
                Physics.SyncTransforms();
            }
        }
    }

    void MoveCrosshair()
    {
        if (InputManager.GetButton("Slash"))
        {
            float border = 20f;
            float pos_x = Mathf.Clamp(InputManager.mousePosition.x, border, Screen.width - border);
            float pos_y = Mathf.Clamp(InputManager.mousePosition.y, border, Screen.height - border);
            crosshair.transform.position = new Vector3(pos_x, pos_y, 0f);
        }
        else
        {
            crosshair.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        }
    }
    //Control Marching Cubes
    private void LateUpdate()
    {
        if (InputManager.GetButton("Grow") && false)
        {
            laser.SetActive(true);
            Terraform(true);
        }
        else if (InputManager.GetButton("Eclipse"))
        {
            laser.SetActive(true);
            Terraform(false);
        }
        else
        {
            laser.SetActive(false);
        }
    }

    private void Terraform(bool add)
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 1000))
        {
            Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
            if (hitChunk == null)
                return;
            _hitPoint = hit.point;
            laserObject.UpdateTarget(hit.point, add);
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;
            if (_hitPrevPoint == Vector3.zero)
                _hitPrevPoint = hit.point;
            else
            {
                _hitPoint = new Vector3((_hitPoint.x + _hitPrevPoint.x) / 2, (_hitPoint.y + _hitPrevPoint.y) / 2, (_hitPoint.z + _hitPrevPoint.z) / 2);
                _hitPrevPoint = hit.point;
            }
            hitChunk.EditWeights(_hitPoint, BrushSize, add);
        }
    }
}