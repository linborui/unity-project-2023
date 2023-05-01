using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    private Camera cam;

    [Header("Prevent negative ")]
    public bool keepAboveFloor = true;

    [Header("Rate on which position get created")]
    [Range(0.1f, 13.0f)]
    public float updateRate = 3.37f;
    [Header("Add random spread to update rate")]
    [Range(0.0f, 5.0f)]
    public float randomSpread = 1.3f;

    [Header("Interpolation speed of camera")]
    [Range(0.01f, 13.0f)]
    public float cameraSpeed = 3.37f;

    [Range(0.01f, 20.0f)]
    public float minDistance = 3.1f;
    [Range(0.01f, 50.0f)]
    public float maxDistance = 17.37f;

    private Vector3 nextPosition;

    #region unity callbacks
    private void Start () {
        cam = Camera.main;
        nextPosition = transform.position;

        StartCoroutine(GeneratePositions());
	} 
	private void Update () {

        UpdatePosition();

        cam.transform.LookAt(transform);
	}
    #endregion

    #region private
    private IEnumerator GeneratePositions() {
        while (this.enabled) {
            yield return new WaitForSeconds(updateRate);
            nextPosition = Random.insideUnitSphere * Random.Range(minDistance, maxDistance);

            if (keepAboveFloor && nextPosition.y < 0)
                nextPosition.y = 0;
        }
    }
    private void UpdatePosition() {
        cam.transform.position = Vector3.Lerp(cam.transform.position, nextPosition, cameraSpeed * Time.deltaTime);
    }
    #endregion
}