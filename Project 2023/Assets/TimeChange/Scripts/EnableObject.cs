using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public GameObject enableObj;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) {
            enableObj.SetActive(true);

        }
    }
}
