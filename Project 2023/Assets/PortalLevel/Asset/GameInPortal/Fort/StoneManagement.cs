using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StoneManagement : MonoBehaviour
{
    public TextMeshProUGUI stoneNum;
    private int stonenum = 0;
    void Awake(){
        stoneNum.text = stonenum.ToString();
    }
    void Update(){
        stoneNum.text = stonenum.ToString();
    }
    public void stoneNumDec(){
        stonenum--;
        stoneNum.text = stonenum.ToString();
    }
    public void stoneNumSet(int value){
        stonenum = value;
        stoneNum.text = stonenum.ToString();
    }
    public int stoneNumGet(){
        return stonenum;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Moveable"))
        {
            if(other.gameObject.GetComponent<Treasures>() != null){
                stonenum *= other.gameObject.GetComponent<Treasures>().Factor();
            }
            else{
                stonenum+=1;
            }
        }
    }
}
