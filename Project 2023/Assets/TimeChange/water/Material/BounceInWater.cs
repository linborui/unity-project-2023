using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class BounceInWater : MonoBehaviour
{
    public Transform[] floaters;
    public float underWaterDrag = 0.3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;
    public bool OnWater;

    Rigidbody float_rb;
    WaveManager waveManager;
    int floatersUnderWater;

    bool underWater;

    public Transform Spot;
    private Vector3 prePos;
    public bool playerenter;
    private Transform PlayerPos;
    public GameObject ReSpawnTime;
    private Quaternion BoatRot;

    // Start is called before the first frame update
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        float_rb = GetComponent<Rigidbody>();
        BoatRot = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (OnWater)
        {
            floatersUnderWater = 0;
            for (int i = 0; i < floaters.Length; i++)
            {
                float difference = floaters[i].position.y - waveManager.waterHeightAtPos(floaters[i].position);

                if (difference < 0)
                {
                    float_rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);
                    floatersUnderWater += 1;

                    if (!underWater)
                    {
                        underWater = true;
                        SwitchState(true);
                    }
                }
            }

            if (underWater && floatersUnderWater == 0)
            {
                underWater = false;
                SwitchState(false);
            }

        }
        else
        {
            float_rb.drag = airDrag;
            float_rb.angularDrag = airAngularDrag;
        }

             Vector3 diff = transform.position - prePos;
        
             if (playerenter)
             {
                 //Vector3 diff = transform.position - prePos;
                 PlayerPos.position += diff;
             }
        
             prePos = transform.position;


    }

    void SwitchState(bool isUnderWater)
    {
        if (isUnderWater)
        {
            float_rb.drag = underWaterDrag; 
            float_rb.angularDrag = underWaterAngularDrag; 
        }
        else
        {
            float_rb.drag = airDrag;
            float_rb.angularDrag = airAngularDrag;
        }
    }
    private float prePower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && other.gameObject.tag == "Player")
        {
            playerenter = true;
            prePower = floatingPower;
            floatingPower = prePower * 1.5f;
        
           PlayerPos = other.transform.parent;
           Debug.Log("Player: "+ PlayerPos.gameObject);
        }
    }


    private void OnTriggerExit(Collider other)
    {
     
        if (other.gameObject.layer == 3 && other.gameObject.tag == "Player")
         {  
            playerenter = false;
            floatingPower = prePower;
        }
        if (other.CompareTag("DeadZone"))
        {
            this.gameObject.SetActive(false);
            transform.rotation = BoatRot;
            transform.position = Spot.position;
            ReSpawnTime.SetActive(true);
        }
    }





}
