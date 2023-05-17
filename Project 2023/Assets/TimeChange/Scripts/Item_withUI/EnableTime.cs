using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class EnableTime : MonoBehaviour
{
    private TimeShiftingController timeShiftingController;
    private DesaturateController desaturateController;
    public AudioManager audioManager;
    [Header("PickUpDialogue")]
    public CanvasGroup PickUpCanvas;
    public float fadeTime = 0.5f;
    public string Text = "«ö¤UFÁä¬B°_";

    [Header("ItemDialogue")]
    public ItemDialogueShow IDS;
    [Space]
    public string Name;  //setting the item
    public ItemManager ItemM;

    private TMP_Text Canvas_text;

    // Start is called before the first frame update
    void Start()
    {
        desaturateController = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<DesaturateController>();
        timeShiftingController = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<TimeShiftingController>();
        Canvas_text = PickUpCanvas.GetComponentInChildren<TMP_Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Canvas_text.text = Text;
            PanelFadeIn();
        }

    }
    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            if (InputManager.GetButtonDown("Pick"))
            {
                audioManager.PlayAudio("Pick");
                desaturateController.CanStop = true;
                timeShiftingController.CanChange = true;
                IDS.SetItemDialogue(Name);
                PanelFadeOut();
     
                IDS.show = true;
                this.gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PanelFadeOut();
    }

    private void PanelFadeIn()
    {
        PickUpCanvas.alpha = 0f;
        PickUpCanvas.DOFade(1f, fadeTime);
    }

    private void PanelFadeOut()
    {
        PickUpCanvas.alpha = 1f;
        PickUpCanvas.DOFade(0f, fadeTime);
    }

}
