using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class ControlUI : MonoBehaviour
{
    public GameObject map;
    public GameObject menu;
    public GameObject hint;
    public GameObject instruction;
    public Player_interface playerinterface;
    public GameObject playerbody;
    public Transform startpoint;
    bool first = true;
    bool second = false;
    bool mapIsShow = false;
    // Update is called once per frame
    void Update()
    {
        if (first && InputManager.GetButtonDown("Next"))
        {
            first = false;
            second = true;
            showInstructions();
            showMap();
        }
        else if(second && InputManager.GetButtonDown("Next")){
            startGame();
        }

        if(playerinterface.HP <= 0){
            playerbody.transform.position = startpoint.transform.position;
            playerinterface.dead = false;
            playerinterface.HP = playerinterface.MaxHP;
            first = true;
            menu.SetActive(true);
        }
    }
    public void startGame() { 
        instruction.SetActive(false);
        second = false;
    }
    public void showInstructions(){
        first = false;
        menu.SetActive(false);
        instruction.SetActive(true);
        second = true;
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
