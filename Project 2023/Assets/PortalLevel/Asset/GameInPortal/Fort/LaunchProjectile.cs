using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public StoneManagement countStone;
    private float launchVelocity = 100f;
    int countSec = 4;
    void OnEnable(){
        InvokeRepeating("Counting",1f,0.3f);
    }
    void Counting(){
        countSec--;
    }
    void Update()
    {
        if(countSec<0 && countStone.stoneNumGet() > 0){
            countStone.stoneNumDec();
            var _projectile = Instantiate(projectile, launchPoint.position,launchPoint.rotation);
            _projectile.GetComponent<Rigidbody>().velocity = launchPoint.up * launchVelocity;
            countSec = 2;
        }    
    }
}
