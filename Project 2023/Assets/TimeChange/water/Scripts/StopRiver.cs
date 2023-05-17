using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRiver : MonoBehaviour
{
    public Material mat;
    private float pretime;
    public DesaturateController dc;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (dc.TimeIsStopped)
        {
            mat.SetInt("_Stopped", 1);
            mat.SetFloat("_StopTime", pretime);
        }
        else
        {
            mat.SetInt("_Stopped", 0);
            pretime = Time.time;
        }

    }
}
