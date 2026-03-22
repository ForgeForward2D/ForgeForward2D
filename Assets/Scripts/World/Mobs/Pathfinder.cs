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
        SortedList<float, Vector2Int> openList = new(new DuplicateKeyComparer());
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gScore = new();

        gScore[from] = 0f;
        openList.Add(Heuristic(from, to), from);
        cameFrom[from] = from;

        int nodesVisited = 0;

        while (openList.Count > 0)
        {
            Vector2Int current = openList.Values[0];
            openList.RemoveAt(0);

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
                openList.Add(tentativeG + Heuristic(neighbor, to), neighbor);
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

    // SortedList doesn't allow duplicate keys; this comparer permits them.
    private class DuplicateKeyComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            int result = x.CompareTo(y);
            return result == 0 ? 1 : result;
        }
    }
}
