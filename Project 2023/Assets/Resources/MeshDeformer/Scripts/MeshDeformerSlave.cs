using UnityEngine;
using System.Collections;

/// <summary>
/// This class uses the deformation values of a master deformer
/// decreases cost by taking already calulated values
/// 
/// Only use if master has the same mesh (at least the same vertex count)
/// </summary>
public class MeshDeformerSlave : MonoBehaviour {

    public bool halfPower;
    public MeshDeformer m_master;

    private Mesh m_mesh;
    private Vector3[] m_oVertices, m_tVertices;
    private bool m_doUpdate = false;

    #region unity callbacks
    private void Start () {
        m_mesh = GetComponent<MeshFilter>().mesh;
        m_mesh.MarkDynamic();

        StartCoroutine(InitializeDelayed());
	}
	
	private void Update () {
        if (!m_doUpdate)
            return;

        if (halfPower)
        {
            for (int i = 0; i < m_mesh.vertices.Length; i++)
            {
                m_tVertices[i] = (m_oVertices[i] + m_master.Vertices[i]) / 2.0f;
            }
            m_mesh.vertices = m_tVertices;
        }
        else
        {
            m_mesh.vertices = m_master.Vertices;
        }

        m_mesh.RecalculateBounds();
    }
    #endregion

    #region private
    // Delay by one frame so master is ready
    private IEnumerator InitializeDelayed()
    {
        yield return 0;

        m_oVertices = m_mesh.vertices;
        m_tVertices = m_mesh.vertices;

        // only works if vertexcount is equal
        if (m_master != null && m_master.Vertices.Length == m_mesh.vertexCount)
        {
            m_doUpdate = true;
        }
    }
    #endregion
}
