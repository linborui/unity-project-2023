using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliceable : MonoBehaviour
{
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    public bool act = true;
    public int life_time = 3;
    
    public IEnumerator sleep(){
        act = false;
        yield return new WaitForSeconds(0.75f);
        act = true;
    }
    public void Sleep(){
        StartCoroutine(sleep());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
