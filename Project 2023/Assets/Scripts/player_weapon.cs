using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Luminosity.IO;

public class Element
{
    public bool             skinned, face;
    public Mesh             mesh, bakedmesh;
    public List<int>        triangles;
    public List<Vector3>    vertices;
    public List<Vector3>    normals;
    public List<Vector2>    uvs;
    public List<BoneWeight> bws;
    public Dictionary<int, Dictionary<int, bool>> edges;
    public Dictionary<int, Vector3>    planevertex;

    public Element(){
        skinned     = false;
        mesh        = new Mesh();
        bakedmesh   = new Mesh();
        triangles   = new List<int>();
        vertices    = new List<Vector3>();
        normals     = new List<Vector3>();
        uvs         = new List<Vector2>();
        bws         = new List<BoneWeight>();
        edges       = new Dictionary<int, Dictionary<int, bool>>();
        planevertex = new Dictionary<int, Vector3>();
    }
}

public class Group
{
    public Vector3[]        vertices;
    public Vector3[]        normals;
    public Vector2[]        uvs;
    public BoneWeight[]     bws;
}

public class player_weapon : MonoBehaviour
{
    public GameObject particle;
    public GameObject Base, Tip;
    private Delauny_Triangulation DT;
    private Vector3 _base, _tip;
    private Vector3 pos;
    private Vector3 moveVel;
    private float prevX, prevY;
    private bool sweaping = false, swapeDone = false;
    private Quaternion desDeg;
    private Vector3 sweapNormal;
    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<sliceable>() == null || other.GetComponentInParent<sliceable>().life_time < 1 || !other.GetComponentInParent<sliceable>().act) return;
        if(!InputManager.GetButton("Slash")) return;

        _tip = Tip.transform.position;
        _base = Base.transform.position;
    }
    public void OnTriggerExit(Collider other)
    {
        if(other.GetComponentInParent<sliceable>() == null || other.GetComponentInParent<sliceable>().life_time < 1 || !other.GetComponentInParent<sliceable>().act) return; 
        if(!InputManager.GetButton("Slash")) return;

        GameObject Parent = other.GetComponentInParent<sliceable>().gameObject;
        Vector3 slide1 = Tip.transform.position - _base;
        Vector3 slide2 = Base.transform.position - _tip;
        Vector3 sweapNormal = Vector3.Cross(slide1, slide2).normalized;
        Vector3 transformedNormal = ((Vector3)(Parent.transform.localToWorldMatrix.transpose * sweapNormal)).normalized;
        Vector3 otherCutPoint = Parent.transform.InverseTransformPoint(_tip);
        Parent.GetComponent<sliceable>().sleep();
        otherCutPoint = new Vector3(otherCutPoint.x, otherCutPoint.y, otherCutPoint.z);
        //otherCutPoint = new Vector3(otherCutPoint.x / size.x, otherCutPoint.y / size.y, otherCutPoint.z / size.z);
        
        Plane slicePlane;
        /*if(other.GetComponentInParent<SkinnedMeshRenderer>()){
            Transform Child = other.GetComponentInParent<SkinnedMeshRenderer>().rootBone;
            slicePlane = new Plane(Quaternion.Inverse(Quaternion.Euler(0, Child.transform.localEulerAngles.y, 0)) * sweapNormal, otherCutPoint);
        }else{
            slicePlane = new Plane(Quaternion.Inverse(Parent.transform.rotation) * sweapNormal, otherCutPoint);
        }*/
        slicePlane = new Plane(Quaternion.Inverse(Parent.transform.rotation) * sweapNormal, otherCutPoint);
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

    private int hash(Vector3 a){
        int x = (int)(a.x*1000),y = (int)(a.y*1000),z = (int)(a.z*1000);
        return ((x << 10) ^ (y << 5) ^ z);
    }

    //can't use disjoint set to count number of group that are unconnect
    private int find_boss(ref List<int> boss, int x){
        if(boss[x] == x) return x;
        return boss[x] = find_boss(ref boss, boss[x]);
    }

    private int disjointSet_split(Element e, List<Element> objs, Dictionary<int, Dictionary<int, bool>> edges, bool face){
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
            if(!group.ContainsKey(boss[i])) {
                group.Add(boss[i], new Element());
                group[boss[i]].skinned = e.skinned;
            }
        
        if(group.Count == 1){
            for(int i = 0; i < e.vertices.Count; ++i){
                Vector3 vertex = e.vertices[i];
                int key = hash(vertex);
                if(edges.ContainsKey(key) && !e.planevertex.ContainsKey(key)) e.planevertex.Add(key, vertex);
            }
            e.edges = edges;
            e.face = face;
            objs.Add(e);
        }else{
            for(int i = 0 ; i < e.triangles.Count; i += 3){
                int key = boss[e.triangles[i]];
                List<Vector2> uvs       = new List<Vector2>();
                List<Vector3> vertices  = new List<Vector3>();
                List<Vector3> normals   = new List<Vector3>();
                List<BoneWeight> bws    = new List<BoneWeight>();
                for(int j = 0; j < 3; ++j){
                    int index = e.triangles[i + j];
                    Vector3 vertex = e.vertices[index];
                    int planeKey = hash(vertex);

                    vertices.Add(vertex);
                    uvs.Add(e.uvs[index]);
                    normals.Add(e.normals[index]);
                    if(e.skinned) bws.Add(e.bws[index]);
                    if(edges.ContainsKey(planeKey) && !group[key].planevertex.ContainsKey(planeKey)) group[key].planevertex.Add(planeKey, vertex);
                }

                Group g = new Group();
                g.vertices = vertices.ToArray();
                g.normals  = normals.ToArray();
                g.uvs      = uvs.ToArray();
                g.bws      = bws.ToArray();
                add_mesh(group[key], g, true);
            }
            foreach(KeyValuePair<int, Element> it in group){
                foreach(KeyValuePair<int, Vector3> itt in it.Value.planevertex)
                    it.Value.edges.Add(itt.Key, edges[itt.Key]);
                it.Value.skinned = e.skinned;
                it.Value.face = face;
                objs.Add(it.Value);
            }
        }
        return group.Count;
    }

    private void fillGap(Element e, Element e1, Plane plane){
        if(e.planevertex.Count < 3)
            return;
        //List<Vector3> triangles = DT.bowyer_watson(e.planevertex, plane);
        List<Vector3> triangles = DT.sweep_line(e.planevertex, e.edges, plane);
        
        //Debug.Log("caculate mesh num = " + triangles.Count / 3);

        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector3 normal = computeNormal(triangles[i], triangles[i + 2], triangles[i + 1]);
            normal.Normalize();

            var direction = Vector3.Dot(normal, plane.normal);
            Vector3[] vertex1   = {triangles[i], triangles[i + 1], triangles[i + 2]}, vertex2   = {triangles[i], triangles[i + 2], triangles[i + 1]};
            Vector3[] normal1   = {-normal, -normal, -normal}, normal2   = {normal, normal, normal};
            Vector2[] uv        = {Vector2.zero, Vector2.zero, Vector2.zero};
            BoneWeight[] bw     = new BoneWeight[3];
            if(e.skinned) {
                int ind1 = e.vertices.IndexOf(triangles[i]), ind2 = e.vertices.IndexOf(triangles[i  + 1]), ind3 = e.vertices.IndexOf(triangles[i + 2]);
                bw[0] = e.bws[ind1];
                bw[1] = e.bws[ind2];
                bw[2] = e.bws[ind3];
            }
            Group g1 = new Group(), g2 = new Group();
            g1.uvs = uv; g2.uvs = uv;
            g1.bws = bw; g2.bws = bw;
            g1.vertices = vertex1; g1.normals = normal2;
            g2.vertices = vertex2; g2.normals = normal1;
            
            if(direction > 0 && e.face || direction < 0 && !e.face){
                add_mesh(e, g1, false);
                add_mesh(e1, g2, false);
            }else{
                add_mesh(e, g2, false);
                add_mesh(e1, g1, false);  
            }
        }
    }

    private void add_meshSide(bool side, Element pos, Element neg, Group g, bool shareVertices){
        if(side)    add_mesh(pos, g, shareVertices);
        else        add_mesh(neg, g, shareVertices);
    }

    private void add_mesh(Element e, Group g, bool shareVertices){
        List<Vector3>   vertices    = e.vertices, normals = e.normals;
        List<Vector2>   uvs         = e.uvs;
        List<int>       triangles   = e.triangles;
        Vector3[]       vertex      = g.vertices;
        Vector2[]       uv          = g.uvs;
        Vector3[]       normal      = g.normals;

        for(int i = 0; i < 3; ++i){
            int ind = vertices.IndexOf(vertex[i]);
            
            if(ind >= 0)
                triangles.Add(ind);
            else{
                if(normal[i] == Vector3.zero) normal[i] = computeNormal(vertex[i], vertex[(1 + i)%3], vertex[(2 + i)%3]);
                if(e.skinned) add_all(e, vertex[i], uv[i], normal[i], g.bws[i]);
                else add_all(e, vertex[i], uv[i], normal[i]);
            }
        }
    }
    private void add_all(Element e, Vector3 vertex, Vector2 uv, Vector3 normal){
        List<Vector3> vertices = e.vertices;
        List<Vector3> normals = e.normals;
        List<Vector2> uvs = e.uvs;
        List<int>     triangles = e.triangles;

        vertices.Add(vertex);
        uvs.Add(uv);
        normals.Add(normal);
        triangles.Add(vertices.IndexOf(vertex));
    }
    private void add_all(Element e, Vector3 vertex, Vector2 uv, Vector3 normal, BoneWeight bw){
        List<Vector3> vertices = e.vertices;
        List<Vector3> normals = e.normals;
        List<Vector2> uvs = e.uvs;
        List<int>     triangles = e.triangles;
        List<BoneWeight> bws = e.bws;

        vertices.Add(vertex);
        uvs.Add(uv);
        normals.Add(normal);
        bws.Add(bw);
        triangles.Add(vertices.IndexOf(vertex));
    }

    private void set_all(Element e){
        e.mesh.vertices    = e.vertices.ToArray();
        e.mesh.uv          = e.uvs.ToArray();
        e.mesh.normals     = e.normals.ToArray();
        e.mesh.triangles   = e.triangles.ToArray();
        if(e.skinned) e.mesh.boneWeights = e.bws.ToArray();
        e.mesh.RecalculateNormals();
        e.mesh.RecalculateBounds();
    }
    private void createObject(GameObject origin, Element element, Vector3 transNormal, bool set){
        Rigidbody rigBody;
        Vector3 size = origin.GetComponentInParent<sliceable>().scale;
        Mesh mesh;
        if(set){
            for(int j = 0; size.x != 1f && j < element.vertices.Count; ++j) element.vertices[j] *= size.x;
            set_all(element);
            mesh = element.mesh;
            GameObject obj = new GameObject();
            
            obj.AddComponent<sliceable>();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            Material[] origin_met;
            
            if(origin.GetComponent<MeshRenderer>() != null) origin_met = origin.GetComponentInChildren<MeshRenderer>().materials;
            else origin_met = origin.GetComponentInParent<SkinnedMeshRenderer>().materials;

            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<MeshRenderer>().sharedMaterials = origin_met;
            obj.GetComponent<sliceable>().life_time = origin.GetComponentInParent<sliceable>().life_time - 1;
            
            MeshCollider collider;
            
            if(mesh.triangles.Length < 12){
                Destroy(obj);
                return;
            }else{
                collider = obj.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
            }

            var rig = obj.AddComponent<Rigidbody>();
            rig.useGravity = true;
            
            obj.transform.localScale = origin.transform.localScale;
            obj.transform.rotation = origin.transform.rotation;
            obj.transform.position = origin.transform.position;
            obj.transform.tag = origin.tag;

            rigBody = obj.GetComponent<Rigidbody>();
        }else{
            if(origin.GetComponentInParent<SkinnedMeshRenderer>()) element.skinned = true;

            set_all(element);
            mesh = element.mesh;

            if(origin.GetComponentInParent<SkinnedMeshRenderer>()){
                GameObject parent = origin.GetComponentInParent<SkinnedMeshRenderer>().gameObject;
                SkinnedMeshRenderer old = parent.GetComponent<SkinnedMeshRenderer>();
                mesh.bindposes = old.sharedMesh.bindposes;
                old.sharedMesh = mesh;
            }
            else{
                origin.GetComponent<MeshFilter>().mesh = mesh;
                MeshCollider collider = origin.GetComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
            }

            //set cooldown for origin
            origin.GetComponentInParent<sliceable>().Sleep();

            rigBody = origin.GetComponent<Rigidbody>();
        }
        Vector3 newNormal = (Quaternion.FromToRotation(Vector3.up, transNormal) * transform.rotation).eulerAngles * 0.02f;
        rigBody.AddForce(newNormal, ForceMode.Impulse);
    }

    public void slice(GameObject a, Plane plane, Vector3 transNormal){
        Mesh mesh, sharedMesh;
        Element positive = new Element(), negative = new Element();
        bool skin = false;

        if(a.GetComponent<MeshFilter>() != null) {
            mesh = a.GetComponent<MeshFilter>().mesh;
            sharedMesh = mesh;
        }
        else{
            sharedMesh = a.GetComponentInParent<SkinnedMeshRenderer>().sharedMesh;
            sharedMesh.UploadMeshData(false);
            mesh = new Mesh();
            a.GetComponentInParent<SkinnedMeshRenderer>().BakeMesh(mesh);
            skin = true;
            positive.skinned = true;
            negative.skinned = true;
        }

        int[]           meshTriangles   = mesh.triangles;
        Vector3[]       vertices        = mesh.vertices;
        Vector3[]       vertices1       = sharedMesh.vertices;
        Vector3[]       normals         = mesh.normals;
        Vector2[]       UVs             = mesh.uv;
        BoneWeight[]    BWs;

        if(skin) BWs = sharedMesh.boneWeights;
        else BWs = new BoneWeight[0];
        //Debug.Log("bone weight " + BWs.Length + " " + vertices.Length);
        List<Vector3>   vertexOnPlane = new List<Vector3>();
        Dictionary<int, Dictionary<int, bool>> edges = new Dictionary<int, Dictionary<int, bool>>();

        for(int i = 0; i < meshTriangles.Length; i += 3){
            //在這邊面是由三角形組成, 三角形又是由三個點組成的,所以說
            Vector3[]       vertice   = new Vector3[3];
            Vector3[]       vertice1  = new Vector3[3];
            Vector3[]       normal    = new Vector3[3];
            Vector2[]       uv        = new Vector2[3];
            BoneWeight[]    bw        = new BoneWeight[3];
            bool[]          vSide     = new bool[3];

            for(int j = 0; j < 3; ++j){
                int index   = meshTriangles[i + j];
                vertice[j]  = vertices[index];
                vertice1[j] = vertices1[index];
                normal[j]   = normals[index];
                uv[j]       = UVs[index];
                if(skin)    bw[j] = BWs[index];
                vSide[j]    = plane.GetSide(vertice[j]);
            }
            Group g = new Group();
            if(!skin) g.vertices = vertice;
            else      g.vertices = vertice1;
            g.normals = normal;
            g.uvs = uv;
            if(skin) g.bws = bw;

            if(vSide[0] == vSide[1] && vSide[1] == vSide[2]){ //3 vertex at the same side
                add_meshSide(vSide[0], positive, negative, g, true);
            }else{
                Vector3[] intersectionPoint = new Vector3[4];
                Vector2[] intersectionUV    = new Vector2[2];

                for(int j = 0; j < 3; ++j){
                    int v0 = (0 - j < 0 ? 3 - j : 0), v1 = (1 - j < 0 ? 2 : 1 - j), v2 = 2 - j;
                            
                    if(vSide[v0] == vSide[v1]){
                        float d1,d2;
                        intersectionPoint[0] = getIntersectionVertexOnPlane(plane, vertice[v1], vertice[v2], out d1);
                        intersectionPoint[1] = getIntersectionVertexOnPlane(plane, vertice[v2], vertice[v0], out d2);
                        d1 = MathF.Abs(d1 / (vertice[v1] - vertice[v2]).magnitude);
                        d2 = MathF.Abs(d2 / (vertice[v2] - vertice[v0]).magnitude);
                        intersectionUV[0] = Vector2.Lerp(uv[v1], uv[v2], d1);
                        intersectionUV[1] = Vector2.Lerp(uv[v2], uv[v0], d2);
                        BoneWeight[] bw1, bw2, bw3;
                        Group g1 = new Group(), g2 = new Group(), g3 = new Group();

                        if(skin){
                            intersectionPoint[2] = Vector3.Lerp(vertice1[v1], vertice1[v2], d1);
                            intersectionPoint[3] = Vector3.Lerp(vertice1[v2], vertice1[v0], d2);
                            intersectionPoint[0] = intersectionPoint[2];
                            intersectionPoint[1] = intersectionPoint[3];
                            vertice = vertice1;
                            bw1 = new BoneWeight[3]{bw[v0], bw[v1], bw[v2]};
                            bw2 = new BoneWeight[3]{bw[v0], bw[v2], bw[v1]};
                            bw3 = new BoneWeight[3]{bw[v1], bw[v2], bw[v0]};
                            g1.bws = bw1;
                            g2.bws = bw2;
                            g3.bws = bw3;
                        }

                        Vector3[] vert1   = {vertice[v0], vertice[v1], intersectionPoint[0]},   vert2   = {vertice[v0], intersectionPoint[0], intersectionPoint[1]},    vert3   = {intersectionPoint[0], vertice[v2], intersectionPoint[1]};
                        Vector3[] nor1    = {Vector3.zero, Vector3.zero, Vector3.zero},         nor2    = {Vector3.zero, Vector3.zero, Vector3.zero},                   nor3 = {Vector3.zero, Vector3.zero, Vector3.zero};
                        Vector2[] uv1     = {uv[v0], uv[v1], intersectionUV[0]},                uv2     = {uv[v0], intersectionUV[0], intersectionUV[1]},               uv3 = {intersectionUV[0], uv[v2], intersectionUV[1]};
                        g1.vertices = vert1; g1.normals = nor1; g1.uvs = uv1;
                        g2.vertices = vert2; g2.normals = nor2; g2.uvs = uv2;
                        g3.vertices = vert3; g3.normals = nor3; g3.uvs = uv3;

                        add_meshSide(vSide[v0],  positive, negative, g1, true);
                        add_meshSide(vSide[v1],  positive, negative, g2, true);
                        add_meshSide(vSide[v2],  positive, negative, g3, true);
                    }
                }
                int key,key1;
                key = hash(intersectionPoint[0]);
                key1 = hash(intersectionPoint[1]);
                if(!edges.ContainsKey(key)) edges.Add(key, new Dictionary<int, bool>());
                if(!edges.ContainsKey(key1)) edges.Add(key1, new Dictionary<int, bool>());
                if(!edges[key].ContainsKey(key1) && key != key1) edges[key].Add(key1, false);
                if(!edges[key1].ContainsKey(key) && key != key1) edges[key1].Add(key, false);
            }
        }
        List<Element> objs = new List<Element>();
        
        int positiveNum = 0, negativeNum = 0;

        positiveNum = disjointSet_split(positive, objs, edges, true);
        negativeNum = disjointSet_split(negative, objs, edges, false);

        //Debug.Log("positive and negative :" + positiveNum + " " + negativeNum);

        objs.Sort((a, b) => {
            int aPVN = a.planevertex.Count, aVN = a.vertices.Count;
            int bPVN = b.planevertex.Count, bVN = b.vertices.Count;
            if(aPVN > bPVN)         return -1;
            else if(aPVN < bPVN)    return 1;
            else{
                if(aVN > bVN) return -1;
                else return 1;
            }
        });
        for(int i = objs.Count - 1; i > 0; --i){
            fillGap(objs[i], objs[0], plane);
            createObject(a, objs[i], transNormal, true);
        }
        createObject(a, objs[0], transNormal, false);
    }
    // Start is called before the first frame update
    void Start()
    {
        particle.SetActive(false);
        DT = transform.gameObject.AddComponent<Delauny_Triangulation>();
    }

    // Update is called once per frame
    void Update()
    {
        pos     = InputManager.mousePosition;
        pos.z   = 0.98f;
        pos     = Camera.main.ScreenToWorldPoint(pos);
        
        SwapeSword();
    }
}