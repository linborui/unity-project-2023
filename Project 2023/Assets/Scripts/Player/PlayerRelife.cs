using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class PlayerRelife : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData = null;
    [SerializeField] private string featureName = "Desaturate";
    [SerializeField] private float DarkPeriod = 0.5f;
    [SerializeField] private float StopPeriod = 1f;
    [SerializeField] private float BackPeriod = 1f;

    ScriptableRendererFeature feature;
    Material mat;

    [Header("DeadEffect")]
    [ColorUsageAttribute(true, true)]
    public Color frameColor;
    public CanvasGroup DeadCanvas;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 1f;

    private DesaturateController TimeStopController;
    private Player_interface playerInterface;
    private PlayerMovement playerMovement;

    [ColorUsageAttribute(true, true)]
    private Color o_Color;

    private bool transitioning = false;
    private bool Dark = false;
    private bool Stop = false;
    public bool Back = false;
    private float timer = 0;

    public void OnTriggerEnter(Collider other) {
        if(other.tag == "Dead") playerInterface.dead = true;
    }
    private void Start()
    {
        TimeStopController = FindObjectOfType<DesaturateController>();
        playerInterface = this.GetComponent<Player_interface>();
        playerMovement = this.GetComponent<PlayerMovement>();

        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        var blitFeature = feature as BlitMaterialFeature;
        mat = blitFeature.Material;
        mat.SetFloat("_Saturation", 1);
        mat.SetFloat("_FullScreenIntensity", 1);
        o_Color = mat.GetColor("_FrameColor");
    }

    public void Update()
    {
        if (!transitioning && playerInterface.dead)
        {
            ResumingTime();         //resumeTime if Stopped , set the FrameColor and Intensity
            transitioning = true;   //Start transition
            Dark = true;
            timer = 0;
            mat.SetColor("_FrameColor", frameColor);
            PanelFadeIn();
        }

        if (transitioning)
        {
            timer += Time.deltaTime;
            if (Dark)            //Scene Get Dark
            {
                if (timer > DarkPeriod)
                {
                    timer = 0;
                    Dark = false;
                    Stop = true;
                }
                else
                {
                    SceneGetDark();
                }
            }
            else if(Stop)       //Stop for a while
            {
                if (timer > StopPeriod)
                {
                    Relife();
                    timer = 0;
                    Stop = false;
                    Back = true;
                    PanelFadeOut();
                    ResetEnemy();
                }
            }
            else if (Back)      //Scene Back
            {
                if (timer > BackPeriod)
                {
                    timer = 0;
                    Back = false;
                    FinishTransition();
                }
                else
                {
                    SceneBack();
                }
            }

        }
    }

    private void ResumingTime()
    {
        if (TimeStopController != null)
        {
            if (TimeStopController.TimeIsStopped)
            {
                TimeStopController.ResumeTime();
            }


            TimeStopController.CanStop = false;
        }
    }

    public void Relife()
    {
        playerMovement.Restart();
        playerMovement.enabled = false;
    }
    private void SceneGetDark()
    {
        float intensity = 1 - Mathf.Clamp01(timer / DarkPeriod);
        mat.SetFloat("_FullScreenIntensity", intensity);
    }
    private void SceneBack()
    {
        float intensity = Mathf.Clamp01(timer / BackPeriod);
        mat.SetFloat("_FullScreenIntensity", intensity);
    }

    private void FinishTransition()
    {
        if (TimeStopController != null)
        {
                TimeStopController.CanStop = true;
        }
        mat.SetFloat("_FullScreenIntensity", 1);
        mat.SetColor("_FrameColor", o_Color);

        playerInterface.HP = playerInterface.MaxHP;
        playerInterface.Stamina = playerInterface.MaxStamina;
        playerInterface.dead = false;

        playerMovement.enabled = true;
        transitioning = false;
    }

    private void PanelFadeIn()
    {
        DeadCanvas.alpha = 0f;
        DeadCanvas.DOFade(1f, fadeInTime);
    }

    private void PanelFadeOut()
    {
        DeadCanvas.alpha = 1f;
        DeadCanvas.DOFade(0f, fadeOutTime);
    }

    void ResetEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<AI>().Reset();
        }
    }


}
