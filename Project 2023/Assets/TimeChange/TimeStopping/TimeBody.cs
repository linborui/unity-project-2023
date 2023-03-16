using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Luminosity.IO;

public class TimeBody : MonoBehaviour
{
    public float TimeBeforeAffected; //The time after the object spawns until it will be affected by the timestop(for projectiles etc)
    public float flyingtime;
    public Vector3 recordedVelocity;
    public float recordedMagnitude;
    public bool IsStopped;
    public Vector3 addingForceNormal;

    private DesaturateController desaturateController;
    private Rigidbody rb;

    private float TimeBeforeAffectedTimer;
    private bool CanBeAffected;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        desaturateController = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<DesaturateController>();
        TimeBeforeAffectedTimer = TimeBeforeAffected;
        addingForceNormal = Vector3.zero;
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
                rb.isKinematic = false;
                TimeBeforeAffectedTimer = 0.15f;      //丟出去後先飛0.15秒
                addingForceNormal = Vector3.zero;
            }

        }


        if(CanBeAffected && desaturateController.TimeIsStopped && !IsStopped  )
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
        rb.AddForce(addingForceNormal, ForceMode.Impulse);
    }
}


//廢
// private bool grabbing; private float timer;
// grabbing = true;
//   else if (grabbing)
//   {
//       timer = 0.01f;
//       grabbing = false;
//       rb.isKinematic = false;
//   }
//
//   if (timer != 0.0f)
//   {
//       if (timer < flyingtime)
//       {
//           float deltaTime = Time.deltaTime;
//           timer += deltaTime;
//       }
//       else
//       {
//           timer = 0;
//       }
//   }
//&& timer == 0.0f

