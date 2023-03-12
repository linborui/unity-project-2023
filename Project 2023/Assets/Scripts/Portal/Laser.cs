using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	//private Transform target;
	[Header("General")]
	public float range = 15f;
	[Header("Use Laser")]
	public bool useLaser = false;
	public GameObject body;
	public LineRenderer lineRenderer;
	public ParticleSystem impactEffect;
	public Light impactLight;
	public Light Light;
	public GameObject Effect;
	[Header("Unity Setup Fields")]

	public Transform partToRotate;
	public float turnSpeed = 10f;

	public Transform firePoint;
	public Material RedMaterial;
	Vector3 target;
	bool add;
	public void UpdateTarget (Vector3 targetPos,bool add) 
	{
		target = targetPos;
		this.add = add;
		float shortestDistance = Mathf.Infinity;
	
		float distanceToEnemy = Vector3.Distance(transform.position, targetPos);
		if (distanceToEnemy < shortestDistance)
		{
			shortestDistance = distanceToEnemy;
		}
		Shoot(targetPos);
	}
	
	void Update () {
		/*
		if (target == null)
		{
			if (useLaser)
			{
				if (lineRenderer.enabled)
				{
					lineRenderer.enabled = false;
					impactEffect.Stop();
					impactLight.enabled = false;
				}
			}
			return;
		}
		*/
		//LockOnTarget();
		useLaser = true;
		if (useLaser)
		{
			Shoot(target);
		} 
	}

	void LockOnTarget (Vector3 targetPos)
	{
		Vector3 dir = targetPos - this.transform.position;
		Quaternion lookRotation = Quaternion.LookRotation(dir);
		Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
		partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
	}

    [System.Obsolete]
    void Shoot (Vector3 targetPos)
	{
		if (!lineRenderer.enabled)
		{
			lineRenderer.enabled = true;
			impactEffect.Play();
			impactLight.enabled = true;
			Light.enabled= true;
		}
		lineRenderer.SetPosition(0, body.transform.position);
		lineRenderer.SetPosition(1, targetPos);
		if(add)
		lineRenderer.SetColors(Color.green, Color.blue);
		if(!add)
		lineRenderer.SetColors(Color.yellow, Color.red);
		Vector3 dir = body.transform.position - targetPos;
		impactEffect.transform.position = targetPos + dir.normalized;
		Debug.LogWarning("impactEffect.transform.position = " + impactEffect.transform.position);
		impactEffect.transform.rotation = Quaternion.LookRotation(dir);
	}
}
