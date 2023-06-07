using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveSnakeStatue : MonoBehaviour
{
    public GameObject Grave;
    public CanvasGroup BlackCanvas;
    public int gravenum = 0;
    public Animator snake;
    public GameObject graveSide;
    public GameObject Audio;
    private StageAudio stA;
    private bool NotChange = true;

    private void Start()
    {
        stA = FindObjectOfType<StageAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Grave.activeInHierarchy && NotChange)
        {
            PanelFadeOut(BlackCanvas, 3f);
            NotChange = false;
        }
      
        if(gravenum == 2)
        {
            graveSide.SetActive(false);
            PanelFadeOut(BlackCanvas, 3f);
            Audio.SetActive(true);
            snake.Play("Open");
            stA.stage = 1;
        }
    }


    private void PanelFadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, fadeTime);
    }
}
