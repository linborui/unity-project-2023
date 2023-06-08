using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class openBook : MonoBehaviour
{
    public GameObject content;
    bool Switch = false;
    public void OnTriggerEnter(Collider other)
    {
        Switch = false;
        if (other.gameObject.CompareTag("Player"))
        {
            content.SetActive(true);
        }
    }

    public void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player"))
            if(InputManager.GetButtonDown("Interact")){
                content.SetActive(Switch);
                Switch = !Switch;
            }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            content.SetActive(false);
        }
    }
}
