using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Luminosity.IO;

public class ChangeSceneInteract : MonoBehaviour
{
    public string sceneName;

    public GameObject interactMessage;
    
    public Image radiusIndicator;

    float pressTime = 3f;
    float indicatorTime;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactMessage.SetActive(true);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactMessage.SetActive(true);
            if (InputManager.GetButton("Interact"))
            {
                indicatorTime += Time.deltaTime;
                radiusIndicator.fillAmount = indicatorTime / pressTime;
                if (indicatorTime >= pressTime)
                {
                    interactMessage.SetActive(false);
                    SceneManager.LoadScene(sceneName);
                }
            }
            else
            {
                indicatorTime = 0f;
                radiusIndicator.fillAmount = 0f;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            interactMessage.SetActive(false);
        }
    }
}
