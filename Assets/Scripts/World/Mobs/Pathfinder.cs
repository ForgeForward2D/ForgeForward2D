using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    private static Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
    };

    public static List<Vector2Int> FindPath(Vector2Int from, Vector2Int to, int maxNodes = 800)
    {
        if (from == to) return new List<Vector2Int>();

        // A* with Manhattan heuristic
        MinHeap openList = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gScore = new();

        gScore[from] = 0f;
        openList.Push(Heuristic(from, to), from);
        cameFrom[from] = from;

        int nodesVisited = 0;

        while (openList.Count > 0)
        {
            Vector2Int current = openList.Pop();

            if (current == to)
                return ReconstructPath(cameFrom, from, to);

            nodesVisited++;
            if (nodesVisited >= maxNodes) break;

            float currentG = gScore[current];

            for (int i = 0; i < Directions.Length; i++)
            {
                Vector2Int neighbor = current + Directions[i];

                if (neighbor != to && !TileMapManager.Instance.Walkable(neighbor))
                    continue;

                float tentativeG = currentG + 1f;

                if (gScore.TryGetValue(neighbor, out float existingG) && tentativeG >= existingG)
                    continue;

                gScore[neighbor] = tentativeG;
                cameFrom[neighbor] = current;
                openList.Push(tentativeG + Heuristic(neighbor, to), neighbor);
            }
        }

        return new List<Vector2Int>();
    }

    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new();
        Vector2Int current = to;
        while (current != from)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }

    /// <summary>Binary min-heap keyed by float priority. Push/Pop are O(log n).</summary>
    private class MinHeap
    {
        private readonly List<(float priority, Vector2Int value)> _data = new();

        public int Count => _data.Count;

        public void Push(float priority, Vector2Int value)
        {
            _data.Add((priority, value));
            SiftUp(_data.Count - 1);
        }

        public Vector2Int Pop()
        {
            var top = _data[0].value;
            int last = _data.Count - 1;
            _data[0] = _data[last];
            _data.RemoveAt(last);
            if (_data.Count > 0) SiftDown(0);
            return top;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_data[i].priority < _data[parent].priority)
                {
                    (_data[i], _data[parent]) = (_data[parent], _data[i]);
                    i = parent;
                }
                else break;
            }
        }

        private void SiftDown(int i)
        {
            int count = _data.Count;
            while (true)
            {
                int smallest = i;
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                if (left < count && _data[left].priority < _data[smallest].priority)
                    smallest = left;
                if (right < count && _data[right].priority < _data[smallest].priority)
                    smallest = right;
                if (smallest == i) break;
                (_data[i], _data[smallest]) = (_data[smallest], _data[i]);
                i = smallest;
            }
        }
    }
}
