using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Crossbow : MonoBehaviour
{
    public GameObject ArrowPrefab;
    public Transform ArrowLaunch;
    public float ArrowSpeed;
    public float FireRate;
    private float firetimer;


    private Vector3 destination;
    private Camera cam;
   // private Animator anim;
    void Start()
    {
        cam = GetComponentInParent<Camera>();
        ArrowLaunch.rotation = Quaternion.LookRotation(Vector3.forward);
        //    anim = GetComponent<Animator>();
    }

    void Update()
    {
        firetimer -= Time.deltaTime;                                                                 //minus 1 per second

        if(InputManager.GetButtonDown("Fire") && firetimer <=0f)                                            //if left click and fire timer less than zero
        {
           // Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
           // RaycastHit hit;
           // if (Physics.Raycast(ray, out hit))
           //     destination = hit.point;
           // else
           //     destination = ray.GetPoint(1000);
           // ShootProjectile();

            Vector3 middleofScreen = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 80f));          //Find the middle of the screen with z offset of 100f (fakes shooting to the middle)
            ArrowLaunch.LookAt(middleofScreen);                                                       //makes the launchtransform look at it
            GameObject arrow = Instantiate(ArrowPrefab, ArrowLaunch.position, ArrowLaunch.rotation); //Instantiate the arrow
                                                                                                  
            arrow.GetComponent<Rigidbody>().velocity = ArrowLaunch.transform.forward * ArrowSpeed;        //Set the velocity of the arrow
            firetimer = FireRate;                                                    // Makes the firetimer go back to the default firerate;     
       //     anim.Play("Shoot");           //Play Shoot Animation
        }
    }

  //  void ShootProjectile()
  //  {
  //      var projectileObj = Instantiate(ArrowPrefab, ArrowLaunch.position, Quaternion.identity) as GameObject;
  //      projectileObj.GetComponent<Rigidbody>().velocity = (destination - ArrowLaunch.position).normalized * ArrowSpeed;
  //  }


}
