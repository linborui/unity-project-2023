using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using Luminosity.IO;

public class Death : MonoBehaviour
{

    [SerializeField] private UniversalRendererData rendererData = null;
    [SerializeField] private string featureName = null;
    [SerializeField] private float transitionPeriod_Dead = 1;
    [SerializeField] private float transitionPeriod_Relife = 1;
    public DesaturateController desaturateController;
    public TimeShiftingController timeShiftingController;
    public AudioManager audioManager;
    public StageAudio stageAudio;

    private bool transitioning_dead = false;
    private bool transitioning_relife = false;
    private float startTime;
    private bool picked = false;

    ScriptableRendererFeature feature;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        var blitFeature = feature as BlitMaterialFeature;
        mat = blitFeature.Material;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitioning_dead || transitioning_relife)
        {
            UpdateTransition();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioManager.StopAudio(stageAudio.preAudioName);
            audioManager.PlayAudio("Death");
            if (desaturateController.CanStop) picked = true;
            desaturateController.ContinueTime();
            desaturateController.ResetTransition();
            desaturateController.CanStop = false;
            timeShiftingController.CanChange = false;

            StartTransition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioManager.PlayAudio(stageAudio.preAudioName);
            StartTransition();
        }
    }
    private void StartTransition()
    {
        startTime = Time.timeSinceLevelLoad;

        if(!transitioning_dead)
            transitioning_dead = true;
        else
        {
            transitioning_dead = false;
            transitioning_relife = true;
        }

    }
    private void UpdateTransition()
    {
        float saturation = 1f;

        if (transitioning_dead)
        {
            saturation = 1 - Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / transitionPeriod_Dead);
        }
        else
        {
            if (Time.timeSinceLevelLoad >= startTime + transitionPeriod_Relife)
            {
                if (picked)
                {
                    timeShiftingController.CanChange = true;
                    desaturateController.CanStop = true;
                }
                transitioning_relife = false;
            }
            else
            {
                saturation = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / transitionPeriod_Relife);
            }
        }

        mat.SetFloat("_Saturation", saturation);
    }
}
