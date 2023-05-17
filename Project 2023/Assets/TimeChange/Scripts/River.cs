using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class River : MonoBehaviour
{
        public float riverForce = 5f;
        public float boatForce = 2f;
        private bool isPlayerInRiver = false;
        private Rigidbody playerRigidbody;
    //public AudioSource underWaterSound;

        private List<Rigidbody> BoatsRB;

    private void Start()
    {
        BoatsRB = new List<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isPlayerInRiver = true;
                playerRigidbody = other.GetComponent<Rigidbody>();
                this.GetComponent<AudioSource>().Play();
            }
            else if (other.gameObject.CompareTag("Boat"))
             {
                other.GetComponent<BounceInWater>().OnWater = true;
                Rigidbody rb = other.GetComponent<Rigidbody>();
                BoatsRB.Add(other.GetComponent<Rigidbody>());
             }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isPlayerInRiver = false;
                this.GetComponent<AudioSource>().Stop();
            }
            else if (other.gameObject.CompareTag("Boat"))
            {

            foreach (Rigidbody boatrb in BoatsRB)
            {
                if (boatrb == other.GetComponent<Rigidbody>())
                {
                    boatrb.GetComponent<BounceInWater>().OnWater = false;
                    BoatsRB.Remove(boatrb);
                    break;
                }
            }
            }
        }

        private void FixedUpdate()
        {
            if (isPlayerInRiver)
            {
                 // 水流方向
                 Vector3 riverDirection = transform.forward;

                playerRigidbody.AddForce(riverDirection * riverForce, ForceMode.Acceleration);
            }
        if (BoatsRB.Count != 0)
        {
            Vector3 riverDirection = transform.forward;

            foreach (Rigidbody boatrb in BoatsRB)
            { 
                if (boatrb.GetComponent<BounceInWater>().playerenter)
                {
                    boatrb.AddForce(riverDirection * boatForce *2f, ForceMode.Acceleration);
                }
                else
                    boatrb.AddForce(riverDirection * boatForce, ForceMode.Acceleration);
            }
        }
        }
}
