using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Material riverMat;
    Texture2D wavesDisplacement;
    public Transform riverTrans;

    Vector4 waveFrequency;
    Vector2 waveSpeed;
    float waveHeight;

    // Start is called before the first frame update
    void Start()
    {
        wavesDisplacement = (Texture2D)riverMat.GetTexture("_NormalMap");
        waveFrequency = riverMat.GetVector("_UV1");
        waveSpeed = riverMat.GetVector("_WaterSpeed");
        waveHeight = riverMat.GetFloat("_Wave");
    }

    public float waterHeightAtPos(Vector3 position)
    {
        return riverTrans.position.y + wavesDisplacement.GetPixelBilinear((position.x * waveFrequency.x + Time.time * waveSpeed.x) * riverTrans.localScale.x, (position.z * waveFrequency.z + Time.time * waveSpeed.y) * riverTrans.localScale.z).g * waveHeight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
