using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public ComputeShader MarchingShader;

    public MeshFilter MeshFilter;
    public MeshCollider MeshCollider;

    ComputeBuffer _trianglesBuffer;
    ComputeBuffer _trianglesCountBuffer;
    ComputeBuffer _weightsBuffer;

    public NoiseGenerator NoiseGenerator;
    Mesh _mesh;
    bool up = false,down = false,left = false,right = false,forward = false,back = false;
    private void Awake() {
        NoiseGenerator = FindObjectOfType<NoiseGenerator>();
        CreateBuffers();
    }

    private void OnDestroy() {
        ReleaseBuffers();
    }

    struct Triangle {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public static int SizeOf => sizeof(float) * 3 * 3;
    }

    float[] _weights;

    void Start() {
        _weights = NoiseGenerator.GetNoise();
        _mesh = new Mesh();
        UpdateMesh();
    }

    void UpdateMesh() {
        Mesh mesh = ConstructMesh();
        MeshFilter.sharedMesh = mesh;
        MeshCollider.sharedMesh = mesh;
    }

    public void EditWeights(Vector3 hitPosition, float brushSize, bool add) {
        int kernel = MarchingShader.FindKernel("UpdateWeights");
        
        _weightsBuffer.SetData(_weights);
        MarchingShader.SetBuffer(kernel, "_Weights", _weightsBuffer);

        //MarchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk);
        Vector3 offset = new(GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2);
        hitPosition =  hitPosition -transform.position+ offset;//-transform.position;//-offset;
        MarchingShader.SetVector("_HitPosition", hitPosition);

        MarchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk);
        MarchingShader.SetFloat("_BrushSize", brushSize);
        MarchingShader.SetFloat("_TerraformStrength", add ? 1f : -1f);

        MarchingShader.Dispatch(kernel, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads);

        _weightsBuffer.GetData(_weights);

        UpdateMesh();
    }

    Mesh ConstructMesh() {
        int kernel = MarchingShader.FindKernel("March");

        MarchingShader.SetBuffer(kernel, "_Triangles", _trianglesBuffer);
        MarchingShader.SetBuffer(kernel, "_Weights", _weightsBuffer);

        MarchingShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk);
        MarchingShader.SetFloat("_IsoLevel", .5f);

        _weightsBuffer.SetData(_weights);
        _trianglesBuffer.SetCounterValue(0);

        MarchingShader.Dispatch(kernel, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, GridMetrics.PointsPerChunk / GridMetrics.NumThreads);

        Triangle[] triangles = new Triangle[ReadTriangleCount()];
        _trianglesBuffer.GetData(triangles);

        return CreateMeshFromTriangles(triangles);
    }

    int ReadTriangleCount() {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
        _trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    Mesh CreateMeshFromTriangles(Triangle[] triangles) {
        Vector3[] verts = new Vector3[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];
        for (int i = 0; i < triangles.Length; i++) {
            Vector3 offset = new(GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2);
            int startIndex = i * 3;
            verts[startIndex] = triangles[i].a - offset;
            verts[startIndex + 1] = triangles[i].b  - offset;
            verts[startIndex + 2] = triangles[i].c - offset;
            tris[startIndex] = startIndex;
            tris[startIndex + 1] = startIndex + 1;
            tris[startIndex + 2] = startIndex + 2;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
    
    private void OnDrawGizmos() {
        if (_weights == null || _weights.Length == 0) {
            return;
        }
        for (int x = 0; x < GridMetrics.PointsPerChunk; x++) {
            for (int y = 0; y < GridMetrics.PointsPerChunk; y++) {
                for (int z = 0; z < GridMetrics.PointsPerChunk; z++) {
                    int index = x + GridMetrics.PointsPerChunk * (y + GridMetrics.PointsPerChunk * z);
                    float noiseValue = _weights[index];
                    Gizmos.color = Color.Lerp(Color.black, Color.white, noiseValue);
                    Vector3 offset = new(GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2,GridMetrics.PointsPerChunk/2);
                    Gizmos.DrawCube(new Vector3(x , y, z) -offset , Vector3.one * .2f);
                }
            }
        }
    }
    
    void CreateBuffers() {
        _trianglesBuffer = new ComputeBuffer(5 * (GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk), Triangle.SizeOf, ComputeBufferType.Append);
        _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        _weightsBuffer = new ComputeBuffer(GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk, sizeof(float));
    }

    void ReleaseBuffers() {
        _trianglesBuffer.Release();
        _trianglesCountBuffer.Release();
        _weightsBuffer.Release();
    }
}
