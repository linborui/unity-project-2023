using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_object : MonoBehaviour
{
    public bool noBreak = false;
    public bool isRig = false;
    public float Vel = 10f;
    public float dmg = 30f;
    public float smoothTime = 0.001f;    
    private Transform Aim;
    private Vector3 targetAngle;
    private Quaternion angle;
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        Vector3 point = other.ClosestPoint(transform.position);
        other.GetComponentInParent<Player_interface>().takeDamage(dmg, point);
        if(!noBreak) Object.Destroy(this.gameObject);
    }

    void Start()
    {   
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in objectsWithTag)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                Aim = obj.transform;
            }
        }

        targetAngle = new Vector3(Aim.position.x, Aim.position.y + 1, Aim.position.z) - transform.position;
        transform.rotation = Quaternion.LookRotation(targetAngle);
        if(isRig) transform.GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward.normalized * Vel, ForceMode.Impulse);
        transform.rotation = angle;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isRig){
            targetAngle = new Vector3(Aim.position.x, Aim.position.y + 1, Aim.position.z) - transform.position;
            angle = Quaternion.LookRotation(targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, angle, smoothTime);
            transform.position += transform.rotation * Vector3.forward.normalized * Vel * Time.deltaTime;
        }
    }
}
