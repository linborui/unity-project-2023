using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectile;
    public CountStone countStone;
    private float launchVelocity = 100f;
    int count = 4;
    void OnEnable(){
        InvokeRepeating("Counting",1f,0.3f);
    }
    void Counting(){
        count--;
    }
    void Update()
    {
        if(count<0 && countStone.stonenum > 0){
            countStone.stonenum--;
            countStone.stoneNum.text = countStone.stonenum.ToString();
            var _projectile = Instantiate(projectile, 
                                        launchPoint.position,
                                        launchPoint.rotation);
            _projectile.GetComponent<Rigidbody>().velocity = launchPoint.up * launchVelocity;
            count = 2;
        }    
    }
}
