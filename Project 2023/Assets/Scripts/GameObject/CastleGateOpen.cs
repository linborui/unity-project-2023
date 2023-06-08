using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleGateOpen : MonoBehaviour
{
    public GameObject[] circles;

    public void detectMagicCircle() {
        int cnt = 0;
        foreach (GameObject circle in circles) {
            if (circle.activeSelf) {
                cnt++;
            }
        }
        if (cnt > 1) return;
        GetComponent<Animator>().SetBool("open", true);
    }
}
