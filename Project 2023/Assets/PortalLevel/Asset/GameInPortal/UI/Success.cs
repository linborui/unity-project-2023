using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;
public class Success : MonoBehaviour
{
    public GameObject SuccessDescription;
    public GameObject SuccessDescription2;
    public GameObject Reward;
    public int state;
    public PortalPlaceSet setting;
    // Start is called before the first frame update
    void Close()
    {
        SuccessDescription.SetActive(false);
        Reward.SetActive(false);
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            if(state == 0){
                setting.UpdateState(1);
                SuccessDescription.SetActive(true);
                Reward.SetActive(true);
                Invoke("Close",5f);
            }
            if(state == 2){
                setting.UpdateState(4);
                //SuccessDescription.GetComponent<TextMeshProUGUI>().text = setting.GetDescription();
                SuccessDescription2.SetActive(true);
            }
        }
    }
}
