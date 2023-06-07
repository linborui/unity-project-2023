using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class life_time : MonoBehaviour
{
    public float life_timer = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("a");
        if (life_timer <= 0)
            Object.Destroy(this.gameObject);
        life_timer -= Time.deltaTime;
    }
}
