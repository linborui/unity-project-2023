using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using DG.Tweening;
using TMPro;

public class PickUp : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioManager audioManager;
    public GameObject EnableItem;
    public Transform WeaponHandler;
    public bool IsWeapon;

    [Header("PickUpDialogue")]
    public CanvasGroup PickUpCanvas;
    public string Text= "«ö¤UFÁä¬B°_";
    public float fadeTime = 0.5f;

    [Header("ItemDialogue")]
    public ItemDialogueShow IDS;
    [Space]
    public string Name;  //setting the item
    public ItemManager ItemM;

    private TMP_Text Canvas_text;

    void Start()
    {
        EnableItem.SetActive(false);
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
                IDS.SetItemDialogue(Name);
                PanelFadeOut();
                if (IsWeapon) {
                    EnableItem.transform.SetParent(WeaponHandler);
                    WeaponHandler.GetComponent<WeaponSwitching>().addWeapon = true;
                }
                else
                    EnableItem.SetActive(true);

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
