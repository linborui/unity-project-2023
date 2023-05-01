using UnityEngine;

public class Rotate : MonoBehaviour {

    public float x, y, z;
    public bool useRandom;
    private float m_rotationMultiplier = 0;

    private void Start()
    {
        if (useRandom)
        {
            m_rotationMultiplier = Random.Range(-2.0f, 2.0f);
        }
        else
        {
            m_rotationMultiplier = 1.0f;
        }
    }

    private void Update () {
        transform.Rotate(
            x * m_rotationMultiplier * Time.deltaTime,
            y * m_rotationMultiplier * Time.deltaTime, 
            z * m_rotationMultiplier * Time.deltaTime);
	}
}