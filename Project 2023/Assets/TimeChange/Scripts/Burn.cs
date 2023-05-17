using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public int maxHitTimes;
    public int currentHitTimes;
    public GameObject timeline;
    public GameObject Plant;
    public ParticleSystem fire;


    // Start is called before the first frame update
    void Start()
    {
        currentHitTimes = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHitTimes >= maxHitTimes)
        {
            timeline.SetActive(true);
            currentHitTimes = 0;
        }
    }


    public void DestroyPlant()
    {
        var e = fire.emission;
        e.enabled = false;
        foreach (Transform child in fire.transform)
        {
            e = child.GetComponent<ParticleSystem>().emission;
            e.enabled = false;
        }

        foreach (Transform child in Plant.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
