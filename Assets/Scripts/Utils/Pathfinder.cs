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

    public static List<Vector2Int> FindPath(Vector2Int source, Vector2Int destination, int maxNodes = 800)
    {
        if (source == destination) return new List<Vector2Int>();

        // A* with Manhattan heuristic
        MinHeap openList = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gScore = new();

        gScore[source] = 0f;
        openList.Push(Heuristic(source, destination), source);
        cameFrom[source] = source;

        int nodesVisited = 0;

        while (openList.Count > 0)
        {
            Vector2Int current = openList.Pop();

            if (current == destination)
                return ReconstructPath(cameFrom, source, destination);

            nodesVisited++;
            if (nodesVisited >= maxNodes) break;

            float currentG = gScore[current];

            for (int i = 0; i < Directions.Length; i++)
            {
                Vector2Int neighbor = current + Directions[i];

                if (neighbor != destination && !TileMapManager.Instance.Walkable(neighbor))
                    continue;

                float tentativeG = currentG + 1f;

                if (gScore.TryGetValue(neighbor, out float existingG) && tentativeG >= existingG)
                    continue;

                gScore[neighbor] = tentativeG;
                cameFrom[neighbor] = current;
                openList.Push(tentativeG + Heuristic(neighbor, destination), neighbor);
            }
        }

        return new List<Vector2Int>();
    }

    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int source, Vector2Int destination)
    {
        List<Vector2Int> path = new();
        Vector2Int current = destination;
        while (current != source)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}
