using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class DesaturateController : MonoBehaviour {

    [SerializeField] private UniversalRendererData rendererData = null;
    [SerializeField] private string featureName = null;
    [SerializeField] private float transitionPeriod = 1;

    private bool transitioning;
    private float startTime;
    ScriptableRendererFeature feature;
    Material mat;
    public bool TimeIsStopped;

    private void Start()
    {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        var blitFeature = feature as BlitMaterialFeature;
        mat = blitFeature.Material;
        mat.SetFloat("_Saturation", 1);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E) && !transitioning) {
            StartTransition();
            StopTime();
        }
        if(transitioning) {
            if(Time.timeSinceLevelLoad >= startTime + transitionPeriod) {
                ContinueTime();
                ResetTransition();
            } else {
                UpdateTransition();
            }
        }
    }


    //GrayScale Screen
    private void StartTransition() {
        startTime = Time.timeSinceLevelLoad;
        transitioning = true;
    }
    private void UpdateTransition() {
            float saturation = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / transitionPeriod);
            mat.SetFloat("_Saturation", saturation);
    }
    private void ResetTransition() {
            mat.SetFloat("_Saturation", 1);          
            transitioning = false;
    }


    //for stopping objects
    public void ContinueTime()      //timestop finish
    {
        TimeIsStopped = false;

        var objects = FindObjectsOfType<TimeBody>();  //Find Every object with the Timebody Component
        for (var i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<TimeBody>().ContinueTime(); //continue time in each of them
        }

    }
    public void StopTime()
    {
        TimeIsStopped = true;
    }

}
