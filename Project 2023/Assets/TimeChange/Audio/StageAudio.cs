using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class StageAudio : MonoBehaviour
{
    public int stage = 1;
    public AudioManager audioManager;
    public TimeShiftingController timeShiftingController;
   // public float fadeInTime;
   // public float fadeOutTime;

    private int PrePastbool;
    private int preStage;
    //private string pastOrpresent;
    public string preAudioName;
   // public Sound NowBGM;

  // private bool fadeInTransition;
  // private bool fadeOutTransition;
  // private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        PrePastbool = timeShiftingController.PastBool;
        preStage = stage;
      //  pastOrpresent = "Past";
        preAudioName = "Stage" + stage + "Past";
        //     preAudio = audioManager.FindSoundSource("Stage" + stage + pastOrpresent);
        audioManager.PlayAudio(preAudioName);
    }

    // Update is called once per frame
    void Update()
    {
        if ((PrePastbool != timeShiftingController.PastBool && (timeShiftingController.PastBool == 0 || timeShiftingController.PastBool == 2))
            || preStage != stage) {

            audioManager.StopAudio(preAudioName);

            if(preStage == stage)
                audioManager.PlayAudio("TimeShift");

            PrePastbool = timeShiftingController.PastBool;
            if (PrePastbool == 0) preAudioName = "Stage" + stage + "Present";                //pastOrpresent = "Present";
            else if (PrePastbool == 2) preAudioName = "Stage" + stage + "Past";          //pastOrpresent = "Past";

            preStage = stage;

            audioManager.PlayAudio(preAudioName);
        }

    }
}



//    if (fadeOutTransition)
//    {
//        Debug.Log("fadeOut" + preAudio.source.volume);
//        if (timer < fadeOutTime)
//        {
//            timer += Time.deltaTime;
//            audioManager.fadeAudio(preAudio, 0f, timer, fadeOutTime);
//        }
//        else
//        {
//            preAudio.source.volume = 0f;
//            preAudio.source.Stop();
//            fadeOutTransition = false;
//            fadeInTransition = true;
//            preAudio = audioManager.FindSoundSource(preAudioName);
//            preAudio.source.volume = 0f;
//            preAudio.source.Play();
//            timer = 0f;
//        }
//    }
//
//    if (fadeInTransition)
//    {
//        Debug.Log("fadeIn" + preAudio.source.volume);
//        if (timer < fadeInTime)
//        {
//            timer += Time.deltaTime;
//            audioManager.fadeAudio(preAudio, preAudio.volume, timer, fadeInTime);
//        }
//        else {
//            fadeInTransition = false;
//            timer = 0f;
//        }

//   }