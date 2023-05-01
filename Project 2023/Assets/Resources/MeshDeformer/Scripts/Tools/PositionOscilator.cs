using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOscilator : MonoBehaviour
{
    public Vector3 speed;
    public Vector3 amp;

    private Vector3 t;

    private Vector3 m_addPos, m_initPos;

    private void Start()
    {
        m_initPos = transform.localPosition;
    }

    private void Update()
    {
        t.x = (t.x + Time.deltaTime * speed.x) % 360.0f;
        t.y = (t.y + Time.deltaTime * speed.y) % 360.0f;
        t.z = (t.z + Time.deltaTime * speed.z) % 360.0f;

        m_addPos.x = Mathf.Sin(t.x) * amp.x;
        m_addPos.y = Mathf.Sin(t.y) * amp.y;
        m_addPos.z = Mathf.Sin(t.z) * amp.z;

        transform.localPosition = m_initPos + m_addPos;
    }
}
