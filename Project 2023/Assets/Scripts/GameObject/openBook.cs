using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class openBook : MonoBehaviour
{
    public GameObject interactMessage;
    public GameObject content;

    void PressInteract()
    {
        interactMessage.SetActive(true);
        if (InputManager.GetButtonDown("Interact"))
        {
            interactMessage.SetActive(false);
            content.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PressInteract();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PressInteract();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        interactMessage.SetActive(false);
    }
}
