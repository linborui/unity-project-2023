using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float timer;
    public float existTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        timer += deltaTime;

        if (timer >= existTime)
        {
            // Destroy(this.gameObject);

            // 物件池回收這個物件。
            ObjectPool<GhostController>.instance.Recycle(this);
        }
    }


    public void Reset()
    {
        timer = 0;
        this.gameObject.layer = 9;  //present layer
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dead"))
        {
            Reset();
            ObjectPool<GhostController>.instance.Recycle(this);
        }
    }



}
