using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DStarLite3d
{
    private readonly GameObject timeManager;
    private const float DefaultUnit = 1f;
    private const int DefaultPartition = 30;
    private const float DefaultMax = 50f;
    private readonly float unit;
    private readonly float maxDistance;
    private readonly Vector3 objectSize;
    private readonly GameObject controlObject;

    private readonly Dictionary<Vector3, float> gScore;
    private readonly Dictionary<Vector3, float> rhsScore;
    private readonly KeyPriorityQueue<Vector3> openSet;
    private readonly HashSet<Vector3> closedSet;
    private readonly List<Vector3> directions;

    private Vector3 start;
    private Vector3 goal;
    private Vector3 current;
    private Vector3 middleDis;

    private readonly int pastlayer;
    private readonly int presentlayer;

    private readonly Func<Vector3, Vector3, bool> IsNodeValid;

    public DStarLite3d(GameObject gameObject, float unit = DefaultUnit, int partition = DefaultPartition, float maxDistance = DefaultMax)
    {
        timeManager = GameObject.FindGameObjectWithTag("TimeManager");
        this.controlObject = gameObject;
        this.objectSize = gameObject.GetComponent<Renderer>().bounds.size;
        this.unit = Mathf.Max(unit, MathF.Round(maxDistance / partition, 2));
        this.maxDistance = maxDistance;
        gScore = new Dictionary<Vector3, float>();
        rhsScore = new Dictionary<Vector3, float>();
        openSet = new KeyPriorityQueue<Vector3>();
        closedSet = new HashSet<Vector3>();

        if (this.unit <= Mathf.Min(objectSize.x, objectSize.y, objectSize.z))
            IsNodeValid = (origin, dir) => { return IsNodeValidCheckBox(origin + dir); };
        else
            IsNodeValid = (origin, dir) => { return IsNodeValidRaycast(origin, dir); };

        directions = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0)
                        directions.Add(new Vector3(x, y, z) * this.unit);
                }

        pastlayer = LayerMask.NameToLayer("Past");
        presentlayer = LayerMask.NameToLayer("Present");

        //Debug.Log("DStarLite: unit: " + this.unit + " size: " + objectSize);
    }

    public Queue<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        middleDis = start - controlObject.transform.position;
        this.start = Vector3Round(start);
        this.goal = Vector3Round(goal);
        UpdateDirections();

        //Debug.Log("DStarLite: Start: " + this.start.ToString() + " Goal: " + this.goal.ToString());

        current = goal;

        Initialize();

        int getPathCnt = 0;

        while (openSet.Count > 0 && (openSet.PeekKeys() < CalculateKeys(this.start) || rhsScore[this.start] != gScore[this.start]))
        {
            var u = openSet.Peek();
            if (closedSet.Contains(u))
            {
                openSet.Dequeue();
                continue;
            }
            if (++getPathCnt > 200)
                break;
            //Debug.Log("DStarLite: point: " + u.ToString());

            if ((u - this.start).magnitude < unit * 0.87f)
                this.start = u;
            var kOld = openSet.PeekKeys();
            var kNew = CalculateKeys(u);
            if (kOld < kNew)
            {
                openSet.UpdateKeys(u, kNew);
            }
            else if (gScore[u] > rhsScore[u])
            {
                gScore[u] = rhsScore[u];
                openSet.Remove(u);
                closedSet.Add(u);
                foreach (var v in GetNeighbors(u))
                {
                    if (v != goal)
                        rhsScore[v] = Mathf.Min(rhsScore[v], GetCost(v, u) + gScore[u]);
                    UpdateVertex(v);
                }
            }
            else
            {
                float g = gScore[u];
                gScore[u] = float.PositiveInfinity;
                var neighbors = GetNeighbors(u);
                neighbors.Add(u);
                foreach (var v in neighbors)
                {
                    if (v != goal && rhsScore[v] == GetCost(v, u) + g)
                    {
                        rhsScore[v] = float.PositiveInfinity;
                        foreach (var neighbor in GetNeighbors(v))
                        {
                            var score = gScore[neighbor] + GetCost(v, neighbor);
                            if (score < rhsScore[v])
                                rhsScore[v] = score;
                        }
                    }
                    UpdateVertex(v);
                }
            }
        }

        //Debug.Log("DStarLite: Start: " + this.start.ToString() + " rhs " + rhsScore[this.start] + " g: " + gScore[this.start]);

        if (float.IsPositiveInfinity(gScore[this.start]))
            return null;

        closedSet.Clear();

        current = this.start;
        var path = new Queue<Vector3>();
        int pathCnt = 0;
        path.Enqueue(current);

        while ((current - this.goal).magnitude >= unit)
        {
            var minGScore = float.PositiveInfinity;
            Vector3 next = default;

            foreach (var neighbor in GetNeighbors(current))
            {
                if (gScore[neighbor] < minGScore)
                {
                    minGScore = gScore[neighbor];
                    next = neighbor;
                }
            }

            if (float.IsPositiveInfinity(minGScore))
                return null;

            current = next;
            path.Enqueue(current);

            if (++pathCnt > getPathCnt)
                return null;
        }

        return path;
    }

    private Vector3 Vector3Round(Vector3 node)
    {
        return new Vector3((float)Math.Round(node.x, 3), (float)Math.Round(node.y, 3), (float)Math.Round(node.z, 3));
    }

    private void Initialize()
    {
        rhsScore[start] = float.PositiveInfinity;
        gScore[start] = float.PositiveInfinity;

        rhsScore[goal] = 0f;
        gScore[goal] = float.PositiveInfinity;

        openSet.Enqueue(goal, CalculateKeys(goal));
        closedSet.Clear();
    }

    private void UpdateVertex(Vector3 node)
    {
        if (gScore[node] != rhsScore[node])
        {
            if (openSet.Contains(node))
                openSet.UpdateKeys(node, CalculateKeys(node));
            else
            {
                var keys = CalculateKeys(node);
                if (keys <= new Key(maxDistance, maxDistance))
                    openSet.Enqueue(node, CalculateKeys(node));
            }
        }
        else if (openSet.Contains(node))
        {
            openSet.Remove(node);
            closedSet.Add(node);
        }
    }

    private List<Vector3> GetNeighbors(Vector3 node)
    {
        var neighbors = new List<Vector3>();

        foreach (var dir in directions)
        {
            if (IsNodeValid(node, dir) || (node + dir - start).magnitude < unit * 0.87f)
            {
                var neighbor = Vector3Round(node + dir);
                if (!closedSet.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                    if (!gScore.ContainsKey(neighbor))
                        gScore[neighbor] = float.PositiveInfinity;
                    if (!rhsScore.ContainsKey(neighbor))
                        rhsScore[neighbor] = float.PositiveInfinity;
                }
            }
        }

        return neighbors;
    }

    private float GetCost(Vector3 node1, Vector3 node2)
    {
        var diff = node2 - node1;
        return diff.magnitude;
    }

    private float Heuristic(Vector3 node1, Vector3 node2)
    {
        var diff = node2 - node1;
        return diff.magnitude;
    }

    private Key CalculateKeys(Vector3 node)
    {
        float min = Mathf.Min(gScore[node], rhsScore[node]);
        return new Key(min + Heuristic(node, start), min);
    }

    private bool IsNodeValidCheckBox(Vector3 node)
    {
        return !Physics.CheckBox(node, objectSize / 2f, Quaternion.identity, GetIgnoreLayer(), QueryTriggerInteraction.Ignore);
    }

    private bool IsNodeValidRaycast(Vector3 origin, Vector3 dir)
    {
        int ignoreLayer = GetIgnoreLayer();
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, dir.magnitude, ignoreLayer, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        for (int x = -1; x < 2; x += 2)
            for (int y = -1; y < 2; y += 2)
                for (int z = -1; z < 2; z += 2)
                {
                    var d = new Vector3(x, y, z);
                    var vertex = Vector3.Scale(objectSize, d) * 0.5f - d * 0.02f;
                    if (Physics.Raycast(origin + vertex, dir, out hit, dir.magnitude, ignoreLayer, QueryTriggerInteraction.Ignore))
                    {
                        return false;
                    }
                }
        return true;
    }

    private void UpdateDirections()
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, (goal - start).normalized).normalized;
        for (int i = 0; i < directions.Count; i++)
        {
            directions[i] = rotation * directions[i];
        }
    }

    private int GetIgnoreLayer()
    {
        int pastBool = timeManager.GetComponent<TimeShiftingController>().PastBool;
        int ignoreLayer = (1 << 2) | (1 << 3) | (1 << 8);
        if (pastBool == 0)
            ignoreLayer |= (1 << pastlayer);
        else if (pastBool == 3)
            ignoreLayer |= (1 << presentlayer);
        return ~ignoreLayer;
    }

}

class Key : IEquatable<Key>
{
    public float k1;
    public float k2;

    public Key(float k1, float k2)
    {
        this.k1 = k1;
        this.k2 = k2;
    }

    public override int GetHashCode()
    {
        return new Tuple<float, float>(k1, k2).GetHashCode();
    }

    public bool Equals(Key other)
    {
        return (k1 == other.k1 && k2 == other.k2);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(Key)) return false;
        return Equals((Key)obj);
    }

    public static bool operator ==(Key left, Key right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Key keys, Key right)
    {
        return !keys.Equals(right);
    }

    public static bool operator >(Key left, Key right)
    {
        if (left.k1 > right.k1) return true;
        else if (left.k1 < right.k1) return false;
        else if (left.k2 > right.k2) return true;
        return false;
    }

    public static bool operator <(Key left, Key right)
    {
        if (left.k1 < right.k1) return true;
        else if (left.k1 > right.k1) return false;
        else if (left.k2 < right.k2) return true;
        return false;
    }

    public static bool operator >=(Key left, Key right)
    {
        if (left.k1 > right.k1) return true;
        else if (left.k1 < right.k1) return false;
        else if (left.k2 >= right.k2) return true;
        return false;
    }

    public static bool operator <=(Key left, Key right)
    {
        if (left.k1 < right.k1) return true;
        else if (left.k1 > right.k1) return false;
        else if (left.k2 <= right.k2) return true;
        return false;
    }

    public static Key operator +(Key left, Key right)
    {
        return new Key(left.k1+ right.k1, left.k2 + right.k2);
    }

    public override string ToString()
    {
        return "(" + k1 + ", " + k2 + ")";
    }
}

class KeyPriorityQueue<T>
{
    private readonly List<(Key keys, T item)> elements = new List<(Key keys, T item)>();

    public int Count => elements.Count;

    public void Enqueue(T item, Key keys)
    {
        elements.Add((keys, item));
        HeapifyUp(elements.Count - 1);
    }

    public T Dequeue()
    {
        var lastIndex = elements.Count - 1;
        var firstItem = elements[0].item;
        elements[0] = elements[lastIndex];
        elements.RemoveAt(lastIndex);
        HeapifyDown(0);
        return firstItem;
    }

    public T Peek()
    {
        if (elements.Count == 0)
            throw new InvalidOperationException("The priority queue is empty");
        return elements[0].item;
    }

    public Key PeekKeys()
    {
        if (elements.Count == 0)
            throw new InvalidOperationException("The priority queue is empty");
        return elements[0].keys;
    }

    public bool Contains(T item)
    {
        foreach (var element in elements)
        {
            if (element.item.Equals(item))
                return true;
        }
        return false;
    }

    public bool Remove(T item)
    {
        for (var i = 0; i < elements.Count; i++)
        {
            if (elements[i].item.Equals(item))
            {
                var lastElementIndex = elements.Count - 1;
                elements[i] = elements[lastElementIndex];
                elements.RemoveAt(lastElementIndex);
                if (Count > 0)
                {
                    int parentIndex = (i - 1) / 2;
                    if (i == 0 || elements[i].keys > elements[parentIndex].keys)
                        HeapifyDown(i);
                    else
                        HeapifyUp(i);
                }
                return true;
            }
        }
        return false;
    }

    public void UpdateKeys(T item, Key keys)
    {
        for (var i = 0; i < elements.Count; i++)
        {
            if (elements[i].item.Equals(item))
            {
                var oldKeys = elements[i].keys;
                elements[i] = (keys, item);
                if (keys > oldKeys)
                    HeapifyDown(i);
                else
                    HeapifyUp(i);
                return;
            }
        }
        Enqueue(item, keys);
    }

    private void Swap(int index1, int index2)
    {
        var temp = elements[index1];
        elements[index1] = elements[index2];
        elements[index2] = temp;
    }

    private void HeapifyUp(int index)
    {
        int parentIndex = (index - 1) / 2;

        while (index > 0 && elements[index].keys < elements[parentIndex].keys)
        {
            Swap(index, parentIndex);
            index = parentIndex;
            parentIndex = (index - 1) / 2;
        }
    }

    private void HeapifyDown(int index)
    {
        var leftChildIndex = index * 2 + 1;
        var rightChildIndex = index * 2 + 2;
        if (leftChildIndex >= elements.Count)
            return;
        var smallerChildIndex = leftChildIndex;
        if (rightChildIndex < elements.Count && elements[rightChildIndex].keys < elements[leftChildIndex].keys)
        {
            smallerChildIndex = rightChildIndex;
        }
        if (elements[index].keys <= elements[smallerChildIndex].keys)
            return;
        Swap(index, smallerChildIndex);
        HeapifyDown(smallerChildIndex);
    }
}
