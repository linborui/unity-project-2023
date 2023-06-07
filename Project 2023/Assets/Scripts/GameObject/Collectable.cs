using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string itemName;

    float destroyTime;
    float magnifyDuration = 0.2f;
    float magnifySpeed = 1f;
    float microfySpeed = 2f;
    float minSize = 0.1f;


    void Start()
    {
        destroyTime = -1f;
    }

    void Update()
    {
        if (destroyTime < 0f)
            return;

        if (destroyTime + magnifyDuration > Time.time)
        {
            transform.localScale = transform.localScale * (1f + magnifySpeed * Time.deltaTime);
        }
        else if (transform.localScale.magnitude > minSize)
        {
            transform.localScale = transform.localScale / (1f + microfySpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyTime > 0f)
            return;

        string[] accTag = { "Player", "Weapon", "Moveable" };
        foreach (string tag in accTag)
        {
            if (other.tag.Equals(tag))
            {
                Collector.count[itemName]++;
                destroyTime = Time.time;
                return;
            }
        }
    }
}
