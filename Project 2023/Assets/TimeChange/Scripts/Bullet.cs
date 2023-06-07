using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitPrefab;
    public GameObject firePrefab;

    private bool collided;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag!= "Bullet" && collision.gameObject.layer != 3 && !collided)
        {
            ContactPoint contactPoint = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
            Vector3 pos = contactPoint.point;
            if(hitPrefab!=null)
            {
                Instantiate(hitPrefab, pos,rot);
            }

            if (firePrefab!=null)
            {
                if (collision.gameObject.tag == "Burn"){
                    GameObject fire = Instantiate(firePrefab, pos, Quaternion.identity);
                    fire.transform.SetParent(collision.transform);
                    collision.gameObject.GetComponentInParent<Burn>().currentHitTimes += 1;
                }
                else if(collision.gameObject.tag == "Grave")
                {
                    if(collision.gameObject.GetComponent<GraveBoss>().attack)
                    {
                        GameObject fire = Instantiate(firePrefab, pos, Quaternion.identity);
                        fire.transform.SetParent(collision.transform);

                        collision.gameObject.GetComponent<GraveBoss>().currentHitTimes += 1;
                    }

                }

 
            }

            collided = true;
            Destroy(gameObject);
        }
    }
}
