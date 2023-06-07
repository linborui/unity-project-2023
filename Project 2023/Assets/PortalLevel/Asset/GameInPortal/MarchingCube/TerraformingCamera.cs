using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class TerraformingCamera : MonoBehaviour
{
	Vector3 _hitPoint;
	Camera _cam;

	public float BrushSize = 5f;
	RaycastHit prevhit;

    void Awake () {
		_cam = GetComponent<Camera>();
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

		if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
			Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
			if(hitChunk == null)
				return;
			_hitPoint = hit.point;
			hitChunk.EditWeights(_hitPoint, BrushSize, add);
		}
	}
	/*
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_hitPoint, BrushSize);
	}*/
}
