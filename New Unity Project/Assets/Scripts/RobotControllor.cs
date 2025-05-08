using UnityEngine;
using System.Collections.Generic;

public class RobotController : MonoBehaviour
{
    public float speed = 2f;
    private List<Vector3> path = new List<Vector3>();
    private int currentIndex = 0;
    private bool moving = false;
    private Vector2Int startGrid, targetGrid;
    private float taskStartTime;
    private LineRenderer lineRenderer;

    public void Initialize(Vector2Int start, Vector2Int target)
    {
        startGrid = start;
        targetGrid = target;

        path = AStarPathfinder.FindPath(startGrid, targetGrid, GameController.Instance.GetWalkableGrid());
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path not found");
            return;
        }

        transform.position = path[0] + Vector3.up * 0.5f;
        currentIndex = 1;
        moving = true;
        taskStartTime = Time.time;
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = path.Count;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i] + Vector3.up * 0.1f);
        }
    }

    void Update()
    {
        if (!moving || currentIndex >= path.Count)
            return;

        Vector3 target = path[currentIndex] + Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                moving = false;
                float timeUsed = Time.time - taskStartTime;
                float distance = (path.Count - 1) * GameController.Instance.cellSize;
                float efficiency = distance / timeUsed;
                Debug.Log($"{name} completed task. Distance: {distance:F2}, Time: {timeUsed:F2}, Efficiency: {efficiency:F2}");

                GameController.Instance.ReportTaskEfficiency(efficiency); // <-- 添加这行
            }

        }
    }
}
