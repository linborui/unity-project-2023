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
    bool mapIsShow = false;
    // Update is called once per frame
    void Update()
    {
        if (first && InputManager.GetButtonDown("Next"))
        {
            first = false;
            startGame();
            showMap();
        }
    }
    public void startGame() { 
        first = false;
        menu.SetActive(false);
    }
    public void showMap()
    {
        if (mapIsShow == false)
        {
            hint.SetActive(true);
            map.SetActive(true);
        }
        else
        {
            hint.SetActive(false);
            map.SetActive(false);
        }
        mapIsShow = !mapIsShow;
    }
}
