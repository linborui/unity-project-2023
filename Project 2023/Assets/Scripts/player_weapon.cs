using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class Element
{
    public Mesh            mesh;
    public List<Vector3>   vertices;
    public List<Vector3>   normals;
    public List<Vector2>   uvs;
    public List<int>       triangles;
    public Element(){
        mesh        = new Mesh();
        vertices    = new List<Vector3>();
        normals     = new List<Vector3>();
        uvs         = new List<Vector2>();
        triangles   = new List<int>();
        
    }
}

public class player_weapon : MonoBehaviour
{
    public GameObject particle;
    public GameObject Base, Tip;
    private Vector3 _base, _tip;
    private Vector3 pos;
    private Vector3 moveVel;
    private float prevX, prevY;
    private bool sweaping = false, swapeDone = false;
    private Quaternion desDeg;
    private Vector3 sweapNormal;
    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<sliceable>() == null || !other.GetComponent<sliceable>().act) return; 

        _tip = Tip.transform.position;
        _base = Base.transform.position;
    }
    public void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<sliceable>() == null || !other.GetComponent<sliceable>().act) return; 
        if(!InputManager.GetButton("Slash")) return;

        Vector3 slide1 = Tip.transform.position - _base;
        Vector3 slide2 = Base.transform.position - _tip;
        Vector3 sweapNormal = Vector3.Cross(slide1, slide2).normalized;
        Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * sweapNormal)).normalized;
        Vector3 otherCutPoint = other.gameObject.transform.InverseTransformPoint(_tip);
        
        Plane slicePlane = new Plane(Quaternion.Inverse(other.transform.rotation) * sweapNormal, otherCutPoint);
        var direction = Vector3.Dot(Vector3.up, transformedNormal);
        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
            slicePlane = slicePlane.flipped;

        slice(other.gameObject, slicePlane, transformedNormal);
    }

    void SwapeSword() { 
        float xAxis = InputManager.mousePosition.x;
        float yAxis = InputManager.mousePosition.y;
        if(InputManager.GetButton("Slash")){
            transform.localPosition = new Vector3(xAxis / Screen.width - 0.5f, yAxis / Screen.height - 0.5f, 0.9f);
            if(!swapeDone){
                float vx = xAxis - prevX, vy = yAxis - prevY;
                float v = Mathf.Sqrt(vx * vx + vy * vy) * Time.deltaTime;
                if(v > 0.3f){
                    sweapNormal = Vector3.Cross(new Vector3( vx, vy, 0), new Vector3( 0, 0, 1)).normalized;
                    transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(vx, -vy) * 180 / Mathf.PI);
                    desDeg = Quaternion.AngleAxis(-179, sweapNormal) * transform.localRotation;
                    sweaping = true;
                }else if(sweaping) swapeDone = true;
                
            }
        }
        if(swapeDone) {
            //Debug.DrawRay(transform.position, sweapNormal, Color.blue);
            //Debug.Log(desDeg.eulerAngles);
            if(transform.localRotation != desDeg){
                transform.localRotation = Quaternion.Slerp(transform.localRotation, desDeg, 25f * Time.deltaTime);
                //Debug.Log("rotating");
                particle.SetActive(true);
            }else{
                swapeDone = false;
                sweaping = false;
                //Debug.Log("rotate off");
                particle.SetActive(false);
            }
        }
        else if(!InputManager.GetButton("Slash"))
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0.5f, -0.1f, 0.9f),ref moveVel, 0.1f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 6f * Time.deltaTime);
        }
        prevX = xAxis;
        prevY = yAxis;
    }
    // Start is called before the first frame update
    void Start()
    {
        particle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pos     = InputManager.mousePosition;
        pos.z   = 0.98f;
        pos     = Camera.main.ScreenToWorldPoint(pos);
        
        SwapeSword();
    }
    
    //要切割模型的話, 需要對最基本的面(三角形)進行切割那我們需要求的其邊上跟平面的 intersection
    private Vector3 getIntersectionVertexOnPlane(Plane plane, Vector3 v1, Vector3 v2, out float dis){
        Ray ray = new Ray(v1, v1 - v2);
        plane.Raycast(ray, out dis);
        Vector3 intersectionPoint = ray.GetPoint(dis);
        
        return intersectionPoint;
    }

    private Vector3 computeNormal(Vector3 v1, Vector3 v2, Vector3 v3){
        Vector3 side1 = v2 - v1, side2 = v3 - v1;

        return Vector3.Cross(side1, side2);
    }

    private void add_meshSide(bool side, ref Element pos, ref Element neg, Vector3[] vertex, Vector2[] uv, Vector3[] normal, bool shareVetices, bool addFirst){
        if(side)    add_mesh(ref pos.vertices, ref pos.uvs, ref pos.triangles, ref pos.normals, vertex, uv, normal, shareVetices, addFirst);
        else        add_mesh(ref neg.vertices, ref neg.uvs, ref neg.triangles, ref neg.normals, vertex, uv, normal, shareVetices, addFirst);
    }

    private void add_mesh(ref List<Vector3> vertices, ref List<Vector2> uvs, ref List<int> triangles, ref List<Vector3> normals, Vector3[] vertex, Vector2[] uv, Vector3[] normal, bool shareVetices, bool addFirst){
        if(addFirst)
            for(int i = 0; i < triangles.Count; ++i)
                triangles[i] += 3;

        for(int i = 0; i < 3; ++i){
            int ind = vertices.IndexOf(vertex[i]);

            if(ind >= 0 && shareVetices)
                triangles.Add(ind);
            else{
                int j = 0;
                
                if(normal[i] == Vector3.zero) normal[i] = computeNormal(vertex[i], vertex[(1 + i)%3], vertex[(2 + i)%3]);
                if(addFirst) j = 1;
                
                add_all(ref vertices, ref uvs, ref triangles, ref normals, vertex[i], uv[i], normal[i], j);
            }
        }
    }
 
    private Vector3 GetHalfwayPoint(List<Vector3> vertexOnPlane, out float dis){
        if(vertexOnPlane.Count > 0) {
            Vector3 firstPoint = vertexOnPlane[0], furthestPoint = Vector3.zero;
            dis = 0;

            foreach(Vector3 it in vertexOnPlane){
                float nowDis = Vector3.Distance(firstPoint, it);
                if(nowDis > dis){
                    dis = nowDis;
                    furthestPoint = it;
                }
            }

            return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
        }else{
            dis = 0;
            return Vector3.zero;
        }
    }

    private void fillGap(Element e, Element e1, Plane plane, bool face, List<Vector3> vertexOnPlane){
        List<Vector3> newVertexOnPlane = vertexOnPlane.Where(x => e.vertices.Contains(x)).ToList();;
        //List<Vector3> newVertexOnPlane = vertexOnPlane;
        //List<Vector3> newVertexOnPlane = vertexOnPlane.Intersect(e.vertices).ToList(); 

        Vector3 beg = GetHalfwayPoint(newVertexOnPlane, out float distance);;

        for (int i = 0; i < newVertexOnPlane.Count; i += 2)
        {
            Vector3 firstVertex, secondVertex;

            firstVertex = newVertexOnPlane[i];
            secondVertex = newVertexOnPlane[(i + 1) % newVertexOnPlane.Count];

            Vector3 normal = computeNormal(beg, secondVertex, firstVertex);
            normal.Normalize();

            var direction = Vector3.Dot(normal, plane.normal);
            Vector3[] vertex1   = {beg, firstVertex, secondVertex}, vertex2   = {beg, secondVertex, firstVertex};
            Vector3[] normal1   = {-normal, -normal, -normal}, normal2   = {normal, normal, normal};
            Vector2[] uv        = {Vector2.zero, Vector2.zero, Vector2.zero};
            if(face && direction > 0 || !face && direction < 0){
                add_mesh(ref e.vertices, ref e.uvs, ref  e.triangles, ref e.normals, vertex1, uv, normal2, false, false);
                add_mesh(ref e1.vertices, ref e1.uvs, ref  e1.triangles, ref e1.normals, vertex2, uv, normal1, false, false);
            }else{
                add_mesh(ref e.vertices, ref e.uvs, ref  e.triangles, ref e.normals, vertex2, uv, normal1, false, false);
                add_mesh(ref e1.vertices, ref e1.uvs, ref  e1.triangles, ref e1.normals, vertex1, uv, normal2, false, false);  
            }         
        }
    }

    //can't use disjoint set to count number of group that are unconnect
    private int find_boss(ref List<int> boss, int x){
        if(boss[x] == x) return x;
        return boss[x] = find_boss(ref boss, boss[x]);
    }

    private int disjointSet_split(ref Element e, ref List<Element> objs){
        //Debug.Log(e.vertices.Count);
        List<int> boss = new List<int>(new int[e.vertices.Count]);
        Dictionary<int, Element> group = new Dictionary<int, Element>();
        Dictionary<Vector3, int> samePosPoint = new Dictionary<Vector3, int>();
        //if model have multiple vertex with same position, we need define there have same boss
        for(int i = 0; i < boss.Count; ++i){
            if(samePosPoint.ContainsKey(e.vertices[i])){
                int v = 0;
                samePosPoint.TryGetValue(e.vertices[i], out v);
                boss[i] = v;
            }else{
                samePosPoint.Add(e.vertices[i], i);
                boss[i] = i;
            }
        }
        //union
        for(int i = 0; i < e.triangles.Count; i += 3){
            if(find_boss(ref boss, e.triangles[i + 1]) != find_boss(ref boss, e.triangles[i])) boss[find_boss(ref boss, e.triangles[i + 1])] = find_boss(ref boss, e.triangles[i]);
            if(find_boss(ref boss, e.triangles[i + 2]) != find_boss(ref boss, e.triangles[i + 1])) boss[find_boss(ref boss, e.triangles[i + 2])] = find_boss(ref boss, e.triangles[i + 1]);
        }
        for(int j = 0; j < e.vertices.Count; ++j)
            find_boss(ref boss, j);
        
        for(int i = 0; i < e.vertices.Count; ++i)
            if(!group.ContainsKey(boss[i])) group.Add(boss[i], new Element());
        if(group.Count == 1){
            objs.Add(e);
        }else{
            Element obj = new Element();
            for(int j = 0 ; j < e.triangles.Count; j += 3){
                int key = boss[e.triangles[j]];
                List<Vector2> uvs = new List<Vector2>();
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                for(int i = 0; i < 3; ++i){
                    vertices.Add(e.vertices[e.triangles[j + i]]);
                    uvs.Add(e.uvs[e.triangles[j + i]]);
                    normals.Add(e.normals[e.triangles[j + i]]);
                }
                add_mesh(ref group[key].vertices, ref group[key].uvs, ref group[key].triangles, ref group[key].normals, vertices.ToArray(), uvs.ToArray(), normals.ToArray(), false, false);
            }
            foreach(KeyValuePair<int, Element> it in group)
                objs.Add(it.Value);
        }
        return group.Count;
    }

    private void add_all(ref List<Vector3> vertices, ref List<Vector2> uvs, ref List<int> triangles, ref List<Vector3> normals, Vector3 vertex, Vector2 uv, Vector3 normal, int index){
        if(index == 0){
            vertices.Add(vertex);
            uvs.Add(uv);
            normals.Add(normal);
            triangles.Add(vertices.IndexOf(vertex));
        }else{
            int i = (int)index;
            vertices.Insert(i, vertex);
            uvs.Insert(i, uv);
            normals.Insert(i, normal);
            triangles.Insert(i, i);
        }
    }
    private void set_all(Element e){
        e.mesh.vertices    = e.vertices.ToArray();
        e.mesh.uv          = e.uvs.ToArray();
        e.mesh.normals     = e.normals.ToArray();
        e.mesh.triangles   = e.triangles.ToArray();
        e.mesh.RecalculateNormals();
        e.mesh.RecalculateBounds();
    }
    private void createObject(GameObject origin, Mesh mesh, Vector3 transNormal, bool set){
        Rigidbody rigBody;
        if(set){
            GameObject obj = new GameObject();
            MeshCollider collider = obj.AddComponent<MeshCollider>();
            obj.transform.localScale = origin.transform.localScale;
            obj.AddComponent<sliceable>();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            Material[] origin_met;
            
            
            if(origin.GetComponent<MeshRenderer>() != null) origin_met = origin.GetComponentInChildren<MeshRenderer>().materials;
            else origin_met = origin.GetComponentInChildren<SkinnedMeshRenderer>().materials;
            
            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshRenderer>().materials = origin_met;
            collider.sharedMesh = mesh;
            try{
                collider.convex = true;
            }catch(Exception e){
                Debug.LogError(e.Message);
                Destroy(obj);
                return;
            }
            var rig = obj.AddComponent<Rigidbody>();
            rig.useGravity = true;
            
            obj.transform.localScale = origin.transform.localScale;
            obj.transform.rotation = origin.transform.rotation;
            obj.transform.position = origin.transform.position;
            obj.transform.tag = origin.tag;

            rigBody = obj.GetComponent<Rigidbody>();
        }else{
            if(origin.GetComponentInChildren<SkinnedMeshRenderer>() != null) origin.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh;
            else origin.GetComponent<MeshFilter>().mesh = mesh;
            //hide the box collider from origin object
            if(origin.GetComponentInChildren<BoxCollider>() != null) origin.GetComponentInChildren<BoxCollider>().enabled = false; 
            if(origin.GetComponent<MeshCollider>() == null) origin.AddComponent<MeshCollider>();
            //set cooldown for origin
            origin.GetComponent<sliceable>().Sleep();

            MeshCollider collider = origin.GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            collider.convex = true;
            rigBody = origin.GetComponent<Rigidbody>();
        }
        Vector3 newNormal = (Quaternion.FromToRotation(Vector3.up, transNormal) * transform.rotation).eulerAngles * 0.02f;
        rigBody.AddForce(newNormal, ForceMode.Impulse);
    }

    public void slice(GameObject a, Plane plane, Vector3 transNormal){
        Mesh mesh;
        if(a.GetComponentInChildren<MeshFilter>() != null)
            mesh   = a.GetComponentInChildren<MeshFilter>().mesh;
        else{
            a.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.UploadMeshData(false);
            mesh = a.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        }
        Element     positive = new Element(), negative = new Element();

        int[]       meshTriangles   = mesh.triangles;
        Vector3[]   vertices        = mesh.vertices;
        Vector3[]   normals         = mesh.normals;
        Vector2[]   UVs             = mesh.uv;
        List<Vector3> vertexOnPlane = new List<Vector3>();
        for(int i = 0; i < meshTriangles.Length; i += 3){
            //在這邊面是由三角形組成, 三角形又是由三個點組成的,所以說
            Vector3[] vertice   = new Vector3[3];
            Vector3[] normal    = new Vector3[3];
            Vector2[] uv        = new Vector2[3];
            bool[]    vSide     = new bool[3];

            for(int j = 0; j < 3; ++j){
                int index   = meshTriangles[i + j];
                vertice[j]  = vertices[index];
                normal[j]   = normals[index];
                uv[j]       = UVs[index];
                vSide[j]    = plane.GetSide(vertice[j]);
            }

            if(vSide[0] == vSide[1] && vSide[1] == vSide[2]){ //3 vertex at the same side
                add_meshSide(vSide[0], ref positive, ref negative, vertice, uv, normal, true, false);
            }else{
                Vector3[] intersectionPoint = new Vector3[2];
                Vector2[] intersectionUV    = new Vector2[2];

                for(int j = 0; j < 3; ++j){
                    int v0 = (0 - j < 0 ? 3 - j : 0), v1 = (1 - j < 0 ? 2 : 1 - j), v2 = 2 - j;
                            
                    if(vSide[v0] == vSide[v1]){
                        float d1,d2;
                        intersectionPoint[0] = getIntersectionVertexOnPlane(plane, vertice[v1], vertice[v2], out d1);
                        intersectionPoint[1] = getIntersectionVertexOnPlane(plane, vertice[v2], vertice[v0], out d2);
                        intersectionUV[0] = Vector2.Lerp(uv[v1], uv[v2], d1);
                        intersectionUV[1] = Vector2.Lerp(uv[v2], uv[v0], d2);

                        Vector3[] vert1   = {vertice[v0], vertice[v1], intersectionPoint[0]},   vert2   = {vertice[v0], intersectionPoint[0], intersectionPoint[1]},    vert3   = {intersectionPoint[0], vertice[v2], intersectionPoint[1]};
                        Vector3[] nor1    = {Vector3.zero, Vector3.zero, Vector3.zero},         nor2    = {Vector3.zero, Vector3.zero, Vector3.zero},                   nor3 = {Vector3.zero, Vector3.zero, Vector3.zero};
                        Vector2[] uv1     = {uv[v0], uv[v1], intersectionUV[0]},                uv2     = {uv[v0], intersectionUV[0], intersectionUV[1]},               uv3 = {intersectionUV[0], uv[v2], intersectionUV[1]};
                        add_meshSide(vSide[v0],  ref positive, ref negative, vert1, uv1, nor1, false, false);
                        add_meshSide(vSide[v1],  ref positive, ref negative, vert2, uv2, nor2, false, false);
                        add_meshSide(vSide[v2],  ref positive, ref negative, vert3, uv3, nor3, false, false);
                    }
                }
                vertexOnPlane.Add(intersectionPoint[0]);
                vertexOnPlane.Add(intersectionPoint[1]);
            }
        }
        List<Element> objs = new List<Element>();
        int positiveNum = 0, negativeNum = 0;

        positiveNum = disjointSet_split(ref positive, ref objs);
        negativeNum = disjointSet_split(ref negative, ref objs);
        
        for(int i = 0; i < objs.Count; ++i){
            if(positiveNum >= negativeNum) {
                if(i != objs.Count - 1) fillGap(objs[i], objs[objs.Count - 1], plane, true,vertexOnPlane);
                if(i == 0) {
                    set_all(objs[0]);
                    createObject(a, objs[i].mesh, transNormal, false);
                }
            }
            else{
                if(i != 0) fillGap(objs[i], objs[0], plane, false,vertexOnPlane);
                if(i == objs.Count - 1){
                    set_all(objs[0]);
                    createObject(a, objs[0].mesh, transNormal, false);
                }
            }
            if(i != 0){
                set_all(objs[i]);
                createObject(a, objs[i].mesh, transNormal, true);
            }
        }
        //Destroy(a);
    }
}
