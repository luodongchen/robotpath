using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Vector3> FindPath(Vector2Int start, Vector2Int end, bool[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        HashSet<Vector2Int> closed = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> open = new PriorityQueue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        gScore[start] = 0;
        open.Enqueue(start, Heuristic(start, end));

        while (open.Count > 0)
        {
            Vector2Int current = open.Dequeue();
            if (current == end)
                return ReconstructPath(cameFrom, current);

            closed.Add(current);
            foreach (Vector2Int dir in Directions)
            {
                Vector2Int neighbor = current + dir;
                if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= width || neighbor.y >= height)
                    continue;
                if (!grid[neighbor.x, neighbor.y] || closed.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, end);
                    open.Enqueue(neighbor, f);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null;
    }

    static List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(GameController.Instance.GridToWorld(current));
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, GameController.Instance.GridToWorld(current));
        }
        return path;
    }

    static float Heuristic(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    static readonly List<Vector2Int> Directions = new List<Vector2Int> {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };
}
