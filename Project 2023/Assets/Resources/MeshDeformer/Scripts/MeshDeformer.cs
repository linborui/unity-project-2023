using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshDeformer : MonoBehaviour
{
    class Vertex
    {
        public Vector3 tVertex;
        public Vector3 oVertex;
        public Vector3 normal;
        public List<int> indices;

        public Vertex()
        {
            indices = new List<int>();
            tVertex = new Vector3(0, 0, 0);
        }
        public Vertex(Vector3 vertex, Vector3 normal, int index)
        {
            this.tVertex = vertex;
            this.oVertex = vertex;
            this.normal = normal;

            indices = new List<int>();
            indices.Add(index);
        }
        public void AddIndex(int index, Vector3 normal)
        {
            indices.Add(index);
            this.normal = (this.normal + normal) / 2;
        }
        public override string ToString()
        {
            string text = "@ Vertex: ";
            text += tVertex + "\n";
            text += " Indices: ";
            foreach (int item in indices)
            {
                text += item + " ";
            }
            return text;
        }
    }
    class VertexList
    {
        public List<Vertex> vertices;
        public VertexList()
        {
            vertices = new List<Vertex>();
        }

        public void Add(Vertex vertex)
        {
            vertices.Add(vertex);
        }

        public void TryAddIndex(Vector3 vertex, Vector3 normal, int index)
        {
            foreach (Vertex item in vertices)
                if (item.tVertex == vertex)
                    item.AddIndex(index, normal);
        }

        public bool Contains(Vector3 vertex)
        {
            foreach (Vertex item in vertices)
                if (item.tVertex == vertex)
                    return true;

            return false;
        }

        public override string ToString()
        {
            string text = "Length " + vertices.Count + "  \n";
            foreach (Vertex item in vertices)
            {
                text += item.ToString();
            }
            return text;
        }
    }

    public enum DefromationType
    {
        Solid, Fractured
    }
    public enum DeformationDirection
    {
        Positive, Negative, Both
    }

    public DefromationType deformationType = DefromationType.Solid;
    public bool relaxMesh = false;

    public bool shuffleVertices = true;
    [Range(0.0f, 5.0f)]
    public float shuffleRate = 0.66f;

    public bool reactOnAttractor = false;
    [Range(0.0f, 10)]
    public float reactionDistance = 1.5f;

    [Header("Overall power of deformation:")]
    [Range(0.0f, 20)]
    public float power = 10.0f;
    [Header("Set deformation direction:")]
    public DeformationDirection direction = DeformationDirection.Positive;

    [Header("FFT influence:")]
    [Range(0.0f, 2)]
    public float fftPower = 1.0f;
    [Header("Random noise influence:")]
    [Range(0.0f, 10.0f)]
    public float randomPower = 0.3f;
    [Header("Speed of linear interpolation:")]
    [Range(0.0f, 25.0f)]
    public float speed = 7.0f;
    [Header("Update rate of animation:")]
    [Range(0.0f, 1.0f)]
    public float m_rate = 0.1f;

    private Vector3[] m_vertices;
    public Vector3[] Vertices {
        get { return m_vertices; }
    }

    private SimpleFFT m_fft;
    private Mesh m_mesh;
    private Vector3[] m_targetVertices;
    private Vector3[] m_oVertices;
    private Vertex[] m_unique;
    private VertexList m_uList;
    private float[] m_offsets;

    private float m_elapsed = 0;
    private float m_shuffleElapsed = 0;

    // used for attractor position dependend deformation
    [Header("Steady inflation: set FFT power and random power 0.")]
    [Header("Reaction distance uses approximator")]
    public Transform approximator;
    private Vector3 m_approxPosition;
    private float m_approxDistance;

    #region unity callbacks
    private void Start()
    {
        m_fft = FindObjectOfType<SimpleFFT>();
        ResetMesh();
    }
    private void Update()
    {
        UpdateFFT();
        if (shuffleVertices)
        {
            m_shuffleElapsed += Time.deltaTime;
            if (m_shuffleElapsed >= shuffleRate)
            {
                m_shuffleElapsed = 0;
                if (deformationType == DefromationType.Solid)
                {
                    m_unique = Shuffle(m_unique);
                }
                else
                {
                    m_oVertices = Shuffle(m_oVertices);
                }
            }
        }

        if (m_elapsed < m_rate)
        {
            m_elapsed += Time.deltaTime;
        }
        else
        {
            m_elapsed = 0;
            UpdateAttractorPosition();
            if (deformationType == DefromationType.Solid)
                UpdateMeshSolid();
            else
                UpdateMeshBreaking();
        }

        m_vertices = m_mesh.vertices;
        for (int i = 0; i < m_oVertices.Length; i++)
        {
            m_vertices[i] = Vector3.Lerp(m_vertices[i], m_targetVertices[i], Time.deltaTime * speed);
        }
        m_mesh.vertices = m_vertices;
        //mesh.RecalculateBounds();
    }
    #endregion

    #region public
    public void ResetMesh()
    {
        m_mesh = GetComponent<MeshFilter>().mesh;
        m_mesh.MarkDynamic();

        List<Vector3> rV = new List<Vector3>();
        rV.Add(m_mesh.vertices[0]);
        for (int i = 1; i < m_mesh.vertices.Length; i++)
        {
            if (!rV.Contains(m_mesh.vertices[i]))
            {
                rV.Add(m_mesh.vertices[i]);
            }
        }
        m_uList = new VertexList();
        for (int i = 0; i < m_mesh.vertexCount; i++)
        {
            if (!m_uList.Contains(m_mesh.vertices[i]))
            {
                m_uList.Add(new Vertex(m_mesh.vertices[i], m_mesh.normals[i], i));
            }
            else
            {
                m_uList.TryAddIndex(m_mesh.vertices[i], m_mesh.normals[i], i);
            }
        }
        m_unique = m_uList.vertices.ToArray();

        m_oVertices = m_mesh.vertices;
        m_targetVertices = m_mesh.vertices;
        m_offsets = new float[] { 0 };
    }
    #endregion

    #region private
    private void UpdateFFT()
    {
        if (m_fft != null)
        {
            m_offsets = m_fft.Spectrum;
        }
    }

    private void UpdateMeshBreaking()
    {
        for (int i = 0; i < m_oVertices.Length; i++)
        {
            float deformFactor = GetRamdomValue() *
                power *
                GetApproximationDeformation(m_oVertices[i]) *
                GetFFTDeformationValue(i);

            m_targetVertices[i] = m_oVertices[i] + m_mesh.normals[i] * ScaleDeformation(deformFactor);
        }
    }

    private void UpdateMeshSolid()
    {
        int i = 0;
        foreach (Vertex item in m_unique)
        {
            float deformFactor =
                GetRamdomValue() *
                power *
                GetApproximationDeformation(item.oVertex) *
                GetFFTDeformationValue(i++);

            // apply deformation
            item.tVertex = item.oVertex + item.normal * ScaleDeformation(deformFactor);

            if (reactOnAttractor)
            { // vertices react on attractor position
                if (m_approxDistance < reactionDistance)
                {
                    UpdateTargetVertices(item, false);
                }
                else
                { // reset mesh if attractor walks away
                    UpdateTargetVertices(item, true);
                }
            }
            else
            { // attractor position check is deactivated
                UpdateTargetVertices(item, relaxMesh);
            }
        }
    }

    // calculates deforamtion factor for the given vertex index based on fft values
    private float GetFFTDeformationValue(int i)
    {
        return (Time.deltaTime * Mathf.Clamp01(power) + m_offsets[i % m_offsets.Length] * fftPower);
    }

    private float ScaleDeformation(float value)
    {
        if (direction == DeformationDirection.Both)
            return value;
        else if (direction == DeformationDirection.Negative)
            return -Mathf.Abs(value);
        else
            return Mathf.Abs(value);
    }

    private float GetRamdomValue()
    {
        return 1.0f + UnityEngine.Random.Range(-randomPower, randomPower);
    }

    private float GetApproximationDeformation(Vector3 v)
    {
        if (reactOnAttractor && approximator != null && reactionDistance > 0)
        {
            m_approxDistance = Vector3.Distance(m_approxPosition, v);
            if (m_approxDistance == 0.0f)
            {
                m_approxDistance = float.MinValue;
            }

            if (m_approxDistance > reactionDistance)
                return 0;

            return (reactionDistance / m_approxDistance);
        }

        return 1;
    }

    private void UpdateAttractorPosition()
    {
        if (!reactOnAttractor)
        {
            m_approxDistance = 1.0f;
        }

        // transfrom attractor position in mesh(local) space
        if (approximator != null)
            m_approxPosition = transform.InverseTransformPoint(approximator.transform.position);
    }

    // use this to deactivate the mesh deformer delayed
    // so the mesh can relax when the attractor is gone
    private IEnumerator DeactivateDelayed()
    {
        yield return new WaitForSeconds(1.0f);
        // set mesh deformer inactive
        enabled = false;
    }

    // apply new position to all target vertices
    private void UpdateTargetVertices(Vertex vertex, bool relax)
    {
        if (relax && reactOnAttractor)
        {
            foreach (int index in vertex.indices)
                m_targetVertices[index] = vertex.oVertex;
        }
        else
        {
            foreach (int index in vertex.indices)
                m_targetVertices[index] = vertex.tVertex;
        }
    }

    // shuffle vertex indices
    private Vertex[] Shuffle(Vertex[] list)
    {
        System.Random shuffleRnd = new System.Random();
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = shuffleRnd.Next(n + 1);
            Vertex value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
    private Vector3[] Shuffle(Vector3[] list)
    {
        System.Random shuffleRnd = new System.Random();
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = shuffleRnd.Next(n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
    #endregion
}