using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class UIShowing : MonoBehaviour
{
    public bool isShowing;
    void Start()
    {
        isShowing = false;
    }

    void Update()
    {
        bool isButtonPressed = false;
        if (InputManager.GetButtonDown("Submit"))
        {
            isButtonPressed = true;
        }
        else if (InputManager.GetButtonDown("Cancel"))
        {
            isButtonPressed = true;
        }

        if (!isShowing) return;

        if (isButtonPressed)
        {
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isShowing = true;
        PlayerMovement.disableMovement = true;
    }

    public void Hide()
    {
        PlayerMovement.disableMovement = false;
        isShowing = false;
        gameObject.SetActive(false);
    }
}
