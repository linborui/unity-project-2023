using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSnakeStatue : MonoBehaviour
{
    public GameObject player;
    public int gravenum = 0;
    public Animator snake;
    public GameObject graveSide;
    public AudioManager ad;

    // Update is called once per frame
    void Update()
    {
        if(gravenum == 2)
        {
            graveSide.SetActive(false);
            snake.Play("Open");
            ad.PlayAudio("StatueOpen");
            StageAudio stA = FindObjectOfType<StageAudio>();
            stA.stage = 1;
        }
    }
}
