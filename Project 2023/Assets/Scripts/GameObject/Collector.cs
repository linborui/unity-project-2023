using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public static Dictionary<string, int> count = new Dictionary<string, int>();
    string[] items = { "vase" };

    void Start()
    {
        foreach (var item in items)
        {
            count[item] = 0;
        }
    }

    void Update()
    {
        
    }
}
