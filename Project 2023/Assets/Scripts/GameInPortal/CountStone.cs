using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CountStone : MonoBehaviour
{
    public  TextMeshProUGUI stoneNum;
    public int stonenum = 0;
    void Awake(){
        stoneNum.text = stonenum.ToString();
    }
    void Update(){
        stoneNum.text = stonenum.ToString();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Moveable"))
        {
            if(other.gameObject.GetComponent<OtherThings>() != null){
                Debug.Log(other.gameObject.GetComponent<OtherThings>().name);
                switch(other.gameObject.GetComponent<OtherThings>().state)
                {
                    case OtherThings.ThingsStates.Note:
                        stonenum*=6;
                        break;
                    case OtherThings.ThingsStates.Shield:
                        stonenum*=4;      
                        break;
                    case OtherThings.ThingsStates.Coin:
                        stonenum*=12;
                        break;
                    case OtherThings.ThingsStates.Times:
                        stonenum*=8;
                        break;
                    case OtherThings.ThingsStates.Heart:
                        stonenum*=10;
                        break;
                }
            }
            else{
                stonenum+=1;
            }
        }
    }
}
