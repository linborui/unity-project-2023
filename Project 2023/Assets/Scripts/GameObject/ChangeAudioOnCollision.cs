using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAudioOnCollision : MonoBehaviour
{
    public string audioName;
    public AudioController audioController;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioController.changeAudio(audioName);
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioController.changeAudio(audioName);
        }
    }
}