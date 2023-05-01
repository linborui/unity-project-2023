using UnityEngine;

/// <summary>
/// A simple component which enables viewport orientation to a 3D camera
/// zoom, move up, down, left, right is applied locally
/// rotation is applied in worldspace
/// </summary>
public class InGameViewportCamera : MonoBehaviour {
    // key binding
    KeyCode upKey = KeyCode.W;
    KeyCode downKey = KeyCode.S;
    KeyCode leftKey = KeyCode.A;
    KeyCode rightKey = KeyCode.D;
    KeyCode reset = KeyCode.Escape;

    float moveSpeed = 0.2f;
    
    float roationSpeed = 3.0f;
    float scaleSpeed = 5.0f;

    Vector3 startPosition;
    Quaternion startRotation;

    Quaternion qRotation = Quaternion.identity;

    private void Start() {
        // initialize with current rotation
        qRotation = transform.rotation;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update() {
        Vector3 translation = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        // zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            translation += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * scaleSpeed);
        }
        // translate
        if (Input.GetKey(upKey)) {
            translation += new Vector3(0, 0, moveSpeed);
        }
        else if (Input.GetKey(downKey)) {
            translation += new Vector3(0, 0, -moveSpeed);
        }
        if (Input.GetKey(leftKey)) {
            translation += new Vector3(-moveSpeed, 0, 0);
        }
        else if (Input.GetKey(rightKey)) {
            translation += new Vector3(moveSpeed, 0, 0);
        } else if (Input.GetKey(reset)) {
            transform.position = startPosition;
            transform.rotation = startRotation;

            qRotation = startRotation;
        }
        // drag
        if (Input.GetMouseButton(0)) {
            translation = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0) * moveSpeed;
        }
        // apply translation
        transform.Translate(translation);

        // rotate
        if (Input.GetMouseButton(1)) {
            rotation = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * roationSpeed;

            // apply translation
            qRotation.eulerAngles += rotation;
            transform.rotation = qRotation;
        }
    }
}