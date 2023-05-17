using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Luminosity.IO;


public class ItemDialogueShow : MonoBehaviour
{
    public CanvasGroup ItemDiscription_D;   //the dialogue on canvas
    public TMP_Text[] ItemContext_D;
    public Image ItemPicture_D;

    public float fadeTime = 0.5f;
    public bool show = false;
    private ItemManager ItemM;

    private void Start()
    {
        ItemM = GetComponent<ItemManager>();
    }

    private void Update()
    {
        if (show && ItemDiscription_D.alpha == 0f)
        {
            PanelFadeIn();
        }

        if (InputManager.GetButtonDown("CloseDialogue") && show)
        {
            PanelFadeOut();
            show = false;
        }
    }

    private void PanelFadeIn()
    {
        ItemDiscription_D.alpha = 0f;
        ItemDiscription_D.DOFade(1f, fadeTime);
    }

    private void PanelFadeOut()
    {
        ItemDiscription_D.alpha = 1f;
        ItemDiscription_D.DOFade(0f, fadeTime);
    }

    public void SetItemDialogue(string Name)
    {
        for (int i = 0; i < ItemM.ItemList.Count; i++)
        {
            if (ItemM.ItemList[i].ItemName == Name)
            {
                ItemPicture_D = ItemM.ItemList[i].ItemImage;
                for (int j = 0; j < 4; j++)
                {
                string str =ItemM.ItemList[i].ItemDescriptionLines[j].Replace("\\n", "\n");
                    ItemContext_D[j].text = str;
                }
                return;
            }
        }
    }

}
