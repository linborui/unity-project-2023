using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class GhostGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject GhostPrefab;
    private Transform _transform;

    private ObjectPool<GhostController> GhostPool;
    private List<GhostController>  updateGhostList;


    // Start is called before the first frame update
    void Start()
    {
        GhostPool = ObjectPool<GhostController>.instance;
        GhostPool.InitPool(GhostPrefab);

        updateGhostList = new List<GhostController>();

        int warmUpCount = 10;
        List<GhostController> warmUpList = new List<GhostController>();
        for (int i = 0; i < warmUpCount; i++)
        {
            GhostController t = GhostPool.Spawn(Vector3.zero, Quaternion.identity);
            t.timer = 10;
            updateGhostList.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetButtonDown("Ghost"))
        {
            int d = 10;
            //float d_angle = 360 / d;
            //float d_radian = 360 / d * Mathf.PI / 180;
            for (int i = 0; i < d; i++)
            {
                // GameObject g = Instantiate(bulletPrefab, _transform.position, Quaternion.identity);
                // BulletController b = g.GetComponent<BulletController>();

                // 透過物件池產生。
                GhostController b = GhostPool.Spawn(this.transform.position, Quaternion.identity);
                Debug.Log("after V:"+b.gameObject.active);
                
                // b.radian = d_radian * i;
                // 重置上一次這個物件執行的某些參數。
                b.Reset();
                updateGhostList.Add(b);
            }
        }
    }
}
