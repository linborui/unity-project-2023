using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delauny_Triangulation : MonoBehaviour
{
    public class Vertex
    {
        public Vector2 val;
        public string ind;
        public Vertex(Vector2 a,string index){
            val = a;
            ind = index;
        }
    }
    public class Pair
    {
        public bool x;
        public Vector2 y;
        public Pair(bool a, Vector2 b){
            x = a;
            y = b;
        }
    }
    public class Circle
    {
        public Vector2 center;
        float   radious;
        public Circle(){
            center = new Vector2();
        }
        public void caculate(Vector2 A, Vector2 B, Vector2 C){
            float x = 0, y = 0;
            float a, b, c, a1, b1, c1;
            a = 2 * (A.x - B.x); b = 2 * (A.y - B.y); c = A.x * A.x + A.y * A.y - B.x * B.x - B.y * B.y;
            a1 = 2 * (B.x - C.x); b1 = 2 * (B.y - C.y); c1 = B.x * B.x + B.y * B.y - C.x * C.x - C.y * C.y;

            x = (b1 * c - b * c1) / (a * b1 - a1 * b);
            y = (a1 * c - a * c1) / (a1 * b - a * b1);

            center.x = x;
            center.y = y;
            radious = Mathf.Abs((A - center).magnitude);

            //Debug.Log("circles x: " + x + " , y: " + y + ", r: " + radious);
        }
        public bool vertexInCircle(Vector2 a){
            float distance = Mathf.Abs((a - center).magnitude);
            if(distance <= radious) return true;
            return false;
        }
        public bool vertexAtCircleRside(Vector2 a){
            if(a.x - center.x > radious) return true;
            return false;
        }
        public bool vertexAtCircleUpside(Vector2 a){
            if(a.y - center.y > radious) return true;
            return false;
        }
    }
    public class Triangle
    {
        public Vertex[] vertices;
        public Circle    circle;
        public Triangle(Vertex a, Vertex b, Vertex c){
            vertices = new Vertex[3];
            circle   = new Circle();

            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            circle.caculate(a.val, b.val, c.val);
        }
        public Triangle(Vector2 a, Vector2 b, Vector2 c, string ind1, string ind2, string ind3){
            vertices = new Vertex[3];
            circle   = new Circle();

            vertices[0] = new Vertex(a, ind1);
            vertices[1] = new Vertex(b, ind2);
            vertices[2] = new Vertex(c, ind3);
            circle.caculate(a, b, c);
        }
    }

    private int hash(Vector2 a, Vector2 b){
        int x =(int)(1000 * a.x),y =(int)(1000 * a.y),z =(int)(1000 * b.x), w = (int)(1000 * b.y);
        return ((x << 15) ^ (y << 10) ^ (z << 5) ^ w);
    }

    public Dictionary<string, bool> visited = new Dictionary<string, bool>();
    public void dfs(string now, Dictionary<string, Pair> active, Dictionary<string, Dictionary<string, bool>> edges){
        //Debug.Log(active[now].y);
        visited[now] = true;

        foreach(KeyValuePair<string, bool> next in edges[now]){
            if(visited[next.Key]) continue;
            dfs(next.Key, active, edges);
        }
    }

    public List<Vector3> sweep_line(Dictionary<string, Vector3> vertices, Dictionary<string, HashSet<string>> edges, Plane plane){
        List<Vector3> ans = new List<Vector3>();
        List<Vertex> vertices_2d = new List<Vertex>();
        List<Triangle> triangles = new List<Triangle>(), ans_triangles = new List<Triangle>();
        Dictionary<string, Pair> active = new Dictionary<string, Pair>(); 
        //string startPoint;

        foreach(KeyValuePair<string, Vector3> it in vertices){
            Vector3 point = it.Value;
            Vector2 point2d;
            Vertex vertex;
            string index = it.Key;
            point = Quaternion.Inverse(Quaternion.LookRotation(plane.normal)) * point;
            point2d = new Vector2(point.x, point.y);
            vertex = new Vertex(point2d, it.Key);
            
            vertices_2d.Add(vertex);
            active.Add(index, new Pair(false, point2d));
            //visited.Add(index, false);
            //startPoint = index;
        }

        //dfs(startPoint, active, edges);

        vertices_2d.Sort((a, b) => {
            if(a.val.x < b.val.x) return -1;
            else if(a.val.x > b.val.x) return 1;
            else{
                if(a.val.y < b.val.y) return -1;
                else return 1;
            }
        });
        
        for(int i = 0; i < vertices_2d.Count; ++i){
            active[vertices_2d[i].ind].x = true;
            while(true){
                //find the connected point
                Vertex a = vertices_2d[i], b = null, c = null;
                foreach(string ind in edges[a.ind])
                    if(active[ind].x && ind != a.ind) b = new Vertex(active[ind].y, ind);
                if(b == null) break;
                foreach(string ind in edges[b.ind])
                    if(active[ind].x && ind != a.ind && ind != b.ind) c = new Vertex(active[ind].y, ind);
                if(c == null) break;

                bool split = false;
                float degree;
                Vector2 v;
                v = a.val - c.val;
                
                if(b.val.x >= c.val.x) split = true;

                degree = Mathf.Atan2(v.x, v.y);
                if(split) degree = -degree;
                if(degree >= Mathf.PI) break;

                edges[a.ind].Remove(b.ind);
                edges[b.ind].Remove(a.ind);
                edges[b.ind].Remove(c.ind);
                edges[c.ind].Remove(b.ind);
                if(!edges[a.ind].Contains(c.ind)) edges[a.ind].Add(c.ind);
                if(!edges[c.ind].Contains(a.ind)) edges[c.ind].Add(a.ind);
                triangles.Add(new Triangle(a, b, c));
                active[b.ind].x = false;
            }
        }

        foreach(Triangle it in triangles)
            ans_triangles.Add(it);

        //Debug.Log(" vertices: " + vertices_2d.Count);
        //Debug.Log(" triangles: " + ans_triangles.Count);

        foreach(Triangle it in ans_triangles){
            ans.Add(vertices[it.vertices[0].ind]);
            ans.Add(vertices[it.vertices[1].ind]);
            ans.Add(vertices[it.vertices[2].ind]);
        }

        return ans;
    }
    public List<Vector3> bowyer_watson(Dictionary<string, Vector3> vertices, Plane plane){
        float factor = 1f;
        List<Vector3> ans = new List<Vector3>();
        List<Vertex> vertices_2d = new List<Vertex>();
        List<Triangle> triangles = new List<Triangle>(), ans_triangles = new List<Triangle>();

        //Debug.Log("vertices num " + vertices.Count);
        //transform vertices from 3 Dimesion to 2 Dimesion, because they are point on the same plane.
        foreach(KeyValuePair<string, Vector3> it in vertices){
            Vector3 vertex = it.Value;
            vertex = Quaternion.Inverse(Quaternion.LookRotation(plane.normal)) * vertex;
            Vertex V = new Vertex(vertex, it.Key);
            factor = Mathf.Max(V.val.y, Mathf.Max(factor, V.val.x));

            vertices_2d.Add(V);
        }

        //generate a superTriangle that can overlap all the vertices on plane
        Vector2 a = new Vector2(100 * factor, 0),b = new Vector2(-100 * factor, 100),c = new Vector2(-100 * factor, -100  * factor);
        Triangle superTriangle = new Triangle(a, b, c, "-", "-", "-");
        triangles.Add(superTriangle);

        //Important, that will speed up the bowyer_watson algo. then bowyer_watson will imporve from O(n^2) to O(nlog(n)), but worst case still be O(n^2)
        vertices_2d.Sort((a, b) => a.val.x.CompareTo(b.val.x));

        //insert a point in plane and split super triangle to delauny triangles
        foreach(Vertex it in vertices_2d){
            List<Triangle> del_triangles = new List<Triangle>();
            Dictionary<int, Vertex[]> edges = new Dictionary<int, Vertex[]>();
            foreach(Triangle t in triangles){
                //detect if the vertice in the triangle, then that triangle is a part of break triangles.
                if(t.circle.vertexInCircle(it.val)){
                    for(int i = 0; i < 3; ++i){
                        int j = (i + 1) % 3;
                        Vertex[] edge = new Vertex[2];
                        edge[0] = t.vertices[i];
                        edge[1] = t.vertices[j];
                        if(edge[0].ind.CompareTo(edge[1].ind) > 0 ? true : false){
                            Vertex temp = edge[0];
                            edge[0] = edge[1];
                            edge[1] = temp;
                        }
                        int key = hash(edge[0].val, edge[1].val);
                        if(edges.ContainsKey(key)) {
                            //Debug.Log("delete");
                            edges.Remove(key);
                        }
                        else edges.Add(key, edge);
                    }
                    del_triangles.Add(t);
                }
                else if(t.circle.vertexAtCircleRside(it.val) || it == vertices_2d[vertices_2d.Count - 1]) {
                    ans_triangles.Add(t);
                    del_triangles.Add(t);
                }
            }
            foreach(Triangle t in del_triangles)
                triangles.Remove(t);
            //Debug.Log("del triangles number: " + del_triangles.Count);
            //Debug.Log("-- Edge number : " + edges.Count);
            foreach(KeyValuePair<int, Vertex[]> itt in edges){
                //Debug.Log("edge: "+ itt.Value[0].val + " " + itt.Value[1].val);
                Triangle t = new Triangle(itt.Value[0].val, itt.Value[1].val, it.val, itt.Value[0].ind, itt.Value[1].ind, it.ind);
                //Debug.Log("triangle: " + t.vertices[0].val + " | " + t.vertices[1].val + " | " + t.vertices[2].val);
                if(it != vertices_2d[vertices_2d.Count - 1]) triangles.Add(t);
                else ans_triangles.Add(t);
            }
        }
        foreach(Triangle it in ans_triangles){
            bool f = false;
            //deleting triangles related to supertriangle
            for(int i = 0; i < 3; ++i)
                if(it.vertices[i].ind == "-") f = true;
            if(f) continue;
            //Debug.Log(it.vertices[0].val + " " + it.vertices[1].val + " " + it.vertices[2].val);
            //Debug.Log(vertices[it.vertices[0].ind] + " " + vertices[it.vertices[1].ind] + " " + vertices[it.vertices[2].ind]);
            ans.Add(vertices[it.vertices[0].ind]);
            ans.Add(vertices[it.vertices[1].ind]);
            ans.Add(vertices[it.vertices[2].ind]);
        }
        return ans;
    }
}
