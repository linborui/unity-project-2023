using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStart : MonoBehaviour
{
    public GameObject DialogManager;
    public GameObject NPCDialog;
    public GameObject PlayerDialog;
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnEnterTrigger");
        if(other.gameObject.tag == "Player")
        {
            DialogManager.SetActive(true);
            DialogManager.GetComponent<DialogSystem>().enabled = true;
            NPCDialog.SetActive(true);
            PlayerDialog.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {          
            DialogManager.SetActive(false);
            NPCDialog.SetActive(false);
            PlayerDialog.SetActive(false);
        }
    }
}
