using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class UIShowing : MonoBehaviour
{
    public Player_interface player;
    float delayTime = 0.1f;
    float timer = 0;

    void Start()
    {
        PlayerMovement.disableMovement = true;
        timer = Time.time;
    }

    void Update()
    {
        if (Time.time - timer < delayTime)
            return;

        if (InputManager.GetButtonDown("Interact"))
            Hide();
        else if (InputManager.GetButtonDown("Submit"))
            Hide();
        else if (InputManager.GetButtonDown("Cancel"))
            Hide();
    }

    public void show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
