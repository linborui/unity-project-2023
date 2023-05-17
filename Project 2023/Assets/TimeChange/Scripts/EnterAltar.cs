using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using DG.Tweening;
using TMPro;

public class EnterAltar : MonoBehaviour
{
    public CanvasGroup BlackCanvas;

    [Header("Particles")]
  //  public GameObject InsideParticles;
    public ParticleSystem Side;
 //   public ParticleSystem runningParticle;
    [Space]
    [Header("Weapon")]
    public GameObject PickBlue;
    public Transform weaponHandler;
    public Crossbow RocketLauncher;
    public GameObject FireBullet;
    public Material RocketFireMat;
    public GameObject timeline;


    private bool put= false;
    private bool enter = false;
    private bool fadeOut = false;

    [Header("PickUpDialogue")]
    public CanvasGroup PickUpCanvas;
    public float fadeTime = 0.5f;

    private TMP_Text Canvas_text;


    private void Start()
    {
        PickBlue.SetActive(false);
        Canvas_text = PickUpCanvas.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if(BlackCanvas.alpha == 1f && !fadeOut && put)
        {
            PanelFadeOut(BlackCanvas, 1f);
            timeline.SetActive(true);
            fadeOut = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3 &&!enter)
        {
            //       InsideParticles.SetActive(true);
            enter = true;
 
            Canvas_text.text = "press F to put the RocketLauncher at the middle of Altar";
            PanelFadeIn(PickUpCanvas, fadeTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(enter && other.gameObject.layer == 3)
        {
            if (InputManager.GetButtonDown("Pick") && !put)
            {           //     EnableItem.GetComponent<PickUp>().enabled = false;
           //     EnableItem.GetComponent<SphereCollider>().enabled = false;

                PickBlue.SetActive(true);
                RocketLauncher.gameObject.SetActive(false);
                RocketLauncher.transform.SetParent(weaponHandler.transform.parent);

                Canvas_text.text = "Danger! Please get away from the Altar";
                weaponHandler.GetComponent<WeaponSwitching>().addWeapon = true;
                put = true;
            }

        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3 && enter)
        {
            enter = false;
            PanelFadeOut(PickUpCanvas, fadeTime);

            if (put)
            {
                PanelFadeIn(BlackCanvas, 1f);
            //    CircleMat = InsideParticles.GetComponent<Renderer>().material.color;
            //    var emission = Side.emission;
            //    emission.enabled = false;
            //    runningParticle.gameObject.SetActive(true);
            //    Side.GetComponent<SphereCollider>().enabled = true;
            //    StartCoroutine(ExampleCoroutine());
            }


        }
    }

  //  IEnumerator ExampleCoroutine()
  //  {
  //      CircleMat.a = Mathf.Lerp(CircleMat.a,0,0.5f);
  //
  //      //yield on a new YieldInstruction that waits for 5 seconds.
  //      yield return new WaitForSeconds(9);
  //      DestroyTheAltar();
  //  }
  //
      public void DestroyTheAltar()
      {
         RocketLauncher.ArrowPrefab = FireBullet;
         RocketLauncher.GetComponentInChildren<Renderer>().material = RocketFireMat;
         
         PanelFadeOut(BlackCanvas, 1f);
        Destroy(timeline);
         Destroy(this.gameObject);
        //   EnableItem.GetComponent<SphereCollider>().enabled = true;     //   InsideParticles.SetActive(false);
        //   EnableItem.GetComponent<PickUp>().enabled = true;
        //   EnableItem.GetComponentInChildren<Renderer>().material = RocketFireMat;
        //   Side.GetComponent<SphereCollider>().enabled = false;
        //   var emission = Side.emission;
        //   emission.enabled = true;
    }



    private void PanelFadeIn(CanvasGroup canvasGroup,float fadeTime)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeTime);
    }

    private void PanelFadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, fadeTime);
    }

    public void changeRocket()
    {
        PickBlue.GetComponentInChildren<Renderer>().material = RocketFireMat;
        PickBlue.GetComponent<PickUp>().enabled = true;
        PickBlue.GetComponent<SphereCollider>().enabled = true;

    }


}
