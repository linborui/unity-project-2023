using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class ControlUI : MonoBehaviour
{
    public GameObject map;
    public GameObject menu;
    public GameObject hint;
    bool first = true;
    // Update is called once per frame
    void Update()
    {
        if (first && InputManager.GetButtonDown("Next"))
        {
            first = false;
            menu.SetActive(false);
            hint.SetActive(true);
            map.SetActive(true);
        }
    }
}
