using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]

public class SceneFade : MonoBehaviour
{
    public float fadeTime = 3f;
    CanvasGroup canvasFade;

    void Start()
    {
        canvasFade = GetComponent<CanvasGroup>();
        PanelFadeOut();
    }

    public void PanelFadeIn()
    {
        canvasFade.alpha = 0f;
        canvasFade.DOFade(1f, fadeTime);
    }

    public void PanelFadeOut()
    {
        canvasFade.alpha = 1f;
        canvasFade.DOFade(0f, fadeTime);
    }
}
