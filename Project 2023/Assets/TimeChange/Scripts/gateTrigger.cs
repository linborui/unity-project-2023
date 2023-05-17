using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateTrigger : MonoBehaviour
{
    public Animator RDoor;
    public Animator LDoor;
    private DesaturateController desaturateController;

    private float preSpeedR;
    private float preSpeedL;

    private bool HasStopped;

    private void Start()
    {
        desaturateController = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<DesaturateController>();

        HasStopped = false;
    }

  // private void Update()
  // {
  //     if (desaturateController.TimeIsStopped && LDoor.speed!=0 && RDoor.speed!=0)
  //     {
  //         preSpeedL = LDoor.speed;
  //         preSpeedR = RDoor.speed;
  //
  //         LDoor.speed = 0;
  //         RDoor.speed = 0;
  //         HasStopped = true;
  //     }
  //     else if (!desaturateController.TimeIsStopped && LDoor.speed != 0 && RDoor.speed != 0) {
  //         LDoor.speed = preSpeedL;
  //         RDoor.speed = preSpeedR;
  //         HasStopped = false;
  //     }
  // }
    private void OnTriggerEnter(Collider other)
    {
        if (!desaturateController.TimeIsStopped)
        {
            if (other.gameObject.layer == 3)
            {
                RDoor.Play("rightDoorClose");
                LDoor.Play("leftDoorClose");
            }
        }
    }


      private void OnTriggerExit(Collider other)
      {

        if (!desaturateController.TimeIsStopped)
        {
            if (other.gameObject.layer == 3)
            {
                RDoor.Play("rightDoorOpen");
                LDoor.Play("leftDoorOpen");
            }
        }
      }
}
