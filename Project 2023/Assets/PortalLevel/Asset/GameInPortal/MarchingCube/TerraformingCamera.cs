using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class TerraformingCamera : MonoBehaviour
{
	Vector3 _hitPoint;
	Vector3 _hitPrevPoint;
	Camera _cam;

	public float BrushSize = 2f;
	RaycastHit prevhit;

    void Awake () {
		_cam = GetComponent<Camera>();
		_hitPrevPoint = _hitPoint = Vector3.zero;
    }

	private void Update() {
		if(InputManager.GetButtonDown("PortalIn")) {
			Terraform(true);
		}
		else if (InputManager.GetButtonDown("PortalOut")) {
			Terraform(false);
		}
	}

	private void Terraform(bool add) {
		RaycastHit hit;

		if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
			Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
			if(hitChunk == null)
				return;
			_hitPoint = hit.point;
			/*
			if(_hitPrevPoint == Vector3.zero)
				_hitPrevPoint = hit.point;
			else{
				_hitPoint = new Vector3( (_hitPoint.x + _hitPrevPoint.x)/2,( _hitPoint.y + _hitPrevPoint.y)/2, (_hitPoint.z + _hitPrevPoint.z)/2);
				_hitPrevPoint = hit.point;
			}*/
			hitChunk.EditWeights(_hitPoint, BrushSize, add);
		}
	}
	
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_hitPoint, BrushSize);
	}
}
