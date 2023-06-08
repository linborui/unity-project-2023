using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class UIShowing : MonoBehaviour
{
    void Start()
    {
        PlayerMovement.disableMovement = true;
    }

    void Update()
    {
        if (InputManager.GetButtonDown("Interact"))
            Hide();
        else if (InputManager.GetButtonDown("Submit"))
            Hide();
        else if (InputManager.GetButtonDown("Cancel"))
            Hide();
    }

    public void Hide()
    {
        PlayerMovement.disableMovement = false;
        gameObject.SetActive(false);
    }
}
