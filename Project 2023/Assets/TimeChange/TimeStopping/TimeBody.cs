using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Luminosity.IO;

public class TimeBody : MonoBehaviour
{
   // public float range;
   // public Transform player;
    public float TimeBeforeAffected; //The time after the object spawns until it will be affected by the timestop(for projectiles etc)
    public Vector3 recordedVelocity;
    public float flyingtime;

    private DesaturateController desaturateController;
    private Rigidbody rb;
    public float recordedMagnitude;
    public bool IsStopped;

    private float TimeBeforeAffectedTimer;
    private bool CanBeAffected;
    private bool grabbing;
    private float timer;


  //  private bool Grabbing;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        desaturateController = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<DesaturateController>();
        TimeBeforeAffectedTimer = TimeBeforeAffected;
        grabbing = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeBeforeAffectedTimer -= Time.deltaTime; // minus 1 per second
        if(TimeBeforeAffectedTimer <= 0f)
        {
            CanBeAffected = true; // Will be affected by timestop
        }


        if (desaturateController.TimeIsStopped)
        {

            if (ObjectControl.controledObject == this.gameObject)
            {
                CanBeAffected = false;
                IsStopped = false;
                grabbing = true;
            }
            else if (grabbing)
            {
                timer = 0.01f;
                grabbing = false;
                rb.isKinematic = false;
            }

            if (timer != 0.0f)
            {
                if (timer < flyingtime)
                {
                    float deltaTime = Time.deltaTime;
                    timer += deltaTime;
                }
                else
                {
                    timer = 0;
                }
            }
        }


        if(CanBeAffected && desaturateController.TimeIsStopped && !IsStopped && timer == 0.0f )//
        {

            if (rb.velocity.magnitude >= 0f) //If Object is moving
            {
                recordedVelocity = rb.velocity.normalized; //records direction of movement
                recordedMagnitude = rb.velocity.magnitude; // records magitude of movement
     
                rb.velocity = Vector3.zero; //makes the rigidbody stop moving
                rb.isKinematic = true; //not affected by forces
                IsStopped = true; // prevents this from looping
            }
        }
    
    }


    public void ContinueTime()
    {
        rb.isKinematic = false;
        IsStopped = false;
        rb.velocity = recordedVelocity * recordedMagnitude; //Adds back the recorded velocity when time continues
    }
}
