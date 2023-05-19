using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public GameObject enableObj;
    public int Stage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) {
            if(enableObj != null)
                enableObj.SetActive(true);
            if (Stage != 0)
            {
                StageAudio stA = FindObjectOfType<StageAudio>();
                stA.stage = Stage;
            }
        }
    }
}
