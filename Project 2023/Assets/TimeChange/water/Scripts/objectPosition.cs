using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPosition : MonoBehaviour
{

    public Material mat;
    private float pretime;
    public DesaturateController dc;
    // Start is called before the first frame update
    void Start()
    {
        mat.SetVector("_ObjectPosition", Vector4.zero);
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetVector("_ObjectPosition", new Vector4(this.transform.position.x, this.transform.position.y, this.transform.position.z, this.transform.localScale.x));
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
