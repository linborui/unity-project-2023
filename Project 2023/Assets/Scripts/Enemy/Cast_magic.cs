using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast_magic : MonoBehaviour
{
    public GameObject cast;
    public Transform body;
    public float delay = 0;
    public bool set = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(delay == 0 && set == true){
            Instantiate(cast, body.position, Quaternion.Euler(body.eulerAngles.x,body.eulerAngles.y,body.eulerAngles.z));
            delay = 2f;
            set = false;
        }
        delay = Mathf.Max(0, delay - Time.deltaTime);
    }
}
