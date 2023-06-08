using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class openBook : MonoBehaviour
{
    public GameObject content;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            content.SetActive(true);
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
