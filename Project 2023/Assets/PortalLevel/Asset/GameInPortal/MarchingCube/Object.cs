using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    int maximum = 7;
    bool ascend = true;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<WeightGenerator>().size = 2;
        InvokeRepeating("ChangeSize", 1f, 1f);
    }
    void ChangeSize()
    {
        if(!ascend) transform.GetComponent<WeightGenerator>().size--;
        if(ascend) transform.GetComponent<WeightGenerator>().size++;
        if(transform.GetComponent<WeightGenerator>().size > 7) ascend = false;
    }
}
