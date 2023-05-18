using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasures : MonoBehaviour
{
    public enum ThingsStates {Note, Shield, Coin,Times,Heart}
    public ThingsStates state = 0;
    public int Factor(){
        int returnValue;
        switch(state){
            case ThingsStates.Note:
                returnValue = 6;
                break;
            case ThingsStates.Shield:
                returnValue = 4;      
                break;
            case ThingsStates.Coin:
                returnValue = 12;
                break;
            case ThingsStates.Times:
                returnValue = 8;
                break;
            case ThingsStates.Heart:
                returnValue = 10;
                break; 
            default:
                returnValue = 1;
                break;
        }
        return returnValue;
    }
}
