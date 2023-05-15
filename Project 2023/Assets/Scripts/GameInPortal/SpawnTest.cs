using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectile;
    int count = 1;
    void OnEnable(){
        InvokeRepeating("Counting",1f,1f);
    }
    void Counting(){
        count--;
    }
    void Update()
    {
        if(count<0){

            var _projectile = Instantiate(projectile, 
                                        transform.position,
                                        transform.rotation);
            count = 2;
        }    
    }
}
