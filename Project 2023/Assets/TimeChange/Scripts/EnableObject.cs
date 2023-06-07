using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnableObject : MonoBehaviour
{
    public GameObject enableObj;
    public GameObject disableObj;
    public int Stage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) {
            if (enableObj != null)
                enableObj.SetActive(true);
            if (disableObj != null)
                disableObj.SetActive(false);

            StageAudio stA = FindObjectOfType<StageAudio>();
            if (Stage != stA.stage)
            {
                stA.stage = Stage;
            }
        }
    }

}
