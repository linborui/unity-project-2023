using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class half : MonoBehaviour
{
    public SkinnedMeshRenderer skin;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = skin.sharedMesh;
        Mesh mesh2 = new Mesh();
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int>     triangles = new List<int>();
        List<BoneWeight> bws = new List<BoneWeight>();
        List<Matrix4x4>  bps = new List<Matrix4x4>();

        Debug.Log("bws size : " + mesh.boneWeights.Length);
        Debug.Log("bps size : " + mesh.bindposes.Length);

        for(int i = 0; i < mesh.triangles.Length * 3 / 4; i += 3){
            for(int j = 0; j < 3; ++j){
                int ind = mesh.triangles[i + j];
                vertices.Add(mesh.vertices[ind]);
                normals.Add(mesh.normals[ind]);
                uvs.Add(mesh.uv[ind]);
                triangles.Add(mesh.triangles[i + j]);
                bws.Add(mesh.boneWeights[ind]);
                //bps.Add(mesh.bindposes[ind]);
            }
        }
        mesh2.vertices = vertices.ToArray();
        mesh2.normals = normals.ToArray();
        mesh2.uv =   uvs.ToArray();
        mesh2.triangles = triangles.ToArray();
        mesh2.boneWeights = bws.ToArray();
        mesh2.bindposes = mesh.bindposes;
        skin.sharedMesh = mesh2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
