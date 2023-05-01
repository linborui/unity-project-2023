using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    private float launchVelocity = 100f;
    int count = 2;
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
                                        launchPoint.position,
                                        launchPoint.rotation);
            _projectile.GetComponent<Rigidbody>().velocity = launchPoint.up * launchVelocity;
            count = 1;
        }    
    }
}
