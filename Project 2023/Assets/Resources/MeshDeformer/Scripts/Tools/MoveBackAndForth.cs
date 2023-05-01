using UnityEngine;

public class MoveBackAndForth : MonoBehaviour {

    public Vector3 direction;
    public float speed = 0.5f;
    public float  randomSpeed = 1.0f;
    private float t, totalSpeed;
    private Vector3 initPos;

	private void Start () {
        initPos = transform.position;
        InvokeRepeating("ChangeRandomValues", 0.0f, 3.0f);
        totalSpeed = Random.Range(-randomSpeed, randomSpeed) * speed;
    }

    private void ChangeRandomValues() {
        totalSpeed = Random.Range(0, randomSpeed) + speed;
    }

    private void Update () {
        transform.localPosition = initPos + direction * Mathf.Cos(t += Time.deltaTime * totalSpeed);
	}
}
