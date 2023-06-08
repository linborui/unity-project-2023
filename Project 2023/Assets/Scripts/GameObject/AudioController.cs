using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public string audioName;
    AudioManager audioManager;

    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioManager.PlayAudio(audioName);
    }

    public void changeAudio(string newAudioName) {
        // Debug.Log("change audio to " + newAudioName);
        if (audioName == newAudioName) return;
        audioManager.StopAudio(audioName);
        audioName = newAudioName;
        audioManager.PlayAudio(audioName);
    }
}
