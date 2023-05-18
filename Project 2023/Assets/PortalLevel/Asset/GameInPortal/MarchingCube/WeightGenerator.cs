using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightGenerator : MonoBehaviour
{
    ComputeBuffer _weightsBuffer;
    public ComputeShader NoiseShader;
    
    public float size;
    public bool circle;
    private void Awake() {
        CreateBuffers();
    }

    private void OnDestroy() {
        ReleaseBuffers();
    }

    public float[] GetNoise() {
        float[] noiseValues = new float[GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk];

        NoiseShader.SetBuffer(0, "_Weights", _weightsBuffer);
        NoiseShader.SetFloat("_Size",size);
        NoiseShader.SetBool("_Circle",circle);
        NoiseShader.SetInt("_ChunkSize", GridMetrics.PointsPerChunk);
        NoiseShader.Dispatch( 0, GridMetrics.PointsPerChunk / GridMetrics.NumThreads, 
                                GridMetrics.PointsPerChunk / GridMetrics.NumThreads, 
                                GridMetrics.PointsPerChunk / GridMetrics.NumThreads);
        _weightsBuffer.GetData(noiseValues);

        return noiseValues;
    }
    void Update(){
        NoiseShader.SetFloat("_Size",size);
    }
    void CreateBuffers() {
        _weightsBuffer = new ComputeBuffer( GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk * GridMetrics.PointsPerChunk, sizeof(float));
    }

    void ReleaseBuffers() {
        _weightsBuffer.Release();
    }
}
