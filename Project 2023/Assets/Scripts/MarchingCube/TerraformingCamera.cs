using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformingCamera : MonoBehaviour
{
	Vector3 _hitPoint;
	Vector3 _hitPrevPoint;
	Camera _cam;

	public float BrushSize = 2f;
	RaycastHit prevhit;
	Portal[] portals;
	int times;
    void Awake () {
		_cam = GetComponent<Camera>();
        portals = FindObjectsOfType<Portal> ();
		_hitPrevPoint = _hitPoint = Vector3.zero;
		times = 0;
    }

	private void LateUpdate() {
		if (Input.GetMouseButton(0)) {
			Terraform(true);
		}
		else if (Input.GetMouseButton(1)) {
			Terraform(false);
		}
		times ++ ;
	}

	private void Terraform(bool add) {
		RaycastHit hit;

		if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
			Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
			if(hitChunk == null)
				return;
			_hitPoint = hit.point;
			float mouseX = Input.mousePosition.x;
			float mouseY = Input.mousePosition.y;
			Debug.Log("MOUSE" + mouseX + " " + mouseY);
			
			if(_hitPrevPoint == Vector3.zero)
				_hitPrevPoint = hit.point;
			else{
				_hitPoint = new Vector3( (_hitPoint.x + _hitPrevPoint.x)/2,( _hitPoint.y + _hitPrevPoint.y)/2, (_hitPoint.z + _hitPrevPoint.z)/2);
				_hitPrevPoint = hit.point;
			}
			hitChunk.EditWeights(_hitPoint, BrushSize, add);
		}
	}
	
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_hitPoint, BrushSize);
	}
}
