using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public List <GameObject> enableObj;
    public List <GameObject> disableObj;
    public int Stage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) {
            foreach (GameObject obj in enableObj)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in disableObj)
            {
                obj.SetActive(false);
            }

            StageAudio stA = FindObjectOfType<StageAudio>();
            if (Stage != stA.stage)
            {
                stA.stage = Stage;
            }
        }
    }
}
