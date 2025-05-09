using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RobotController : MonoBehaviour
{
    public float moveInterval = 0.2f;

    private QLearningAgent agent;
    private Vector2Int currentPos, targetPos;
    private float timer;
    private float taskStartTime;
    private float totalDistance;
    private bool taskCompleted = false;

    private LineRenderer lineRenderer;
    private List<Vector3> pathPoints = new List<Vector3>();

    private string qTablePath => Application.persistentDataPath + "/qtable.json";

    public bool TaskCompleted => taskCompleted;
    public float LastEfficiency { get; private set; }


    public void Initialize(Vector2Int start, Vector2Int target)
    {
        currentPos = start;
        targetPos = target;

        agent = new QLearningAgent(GameController.Instance.width, GameController.Instance.height);
        agent.LoadQTable(qTablePath);

        transform.position = GameController.Instance.GridToWorld(start) + Vector3.up * 0.5f;
        totalDistance = Vector2Int.Distance(start, target) * GameController.Instance.cellSize;
        taskStartTime = Time.time;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.white;
    }

    void Update()
    {
        if (taskCompleted) return;

        timer += Time.deltaTime;
        if (timer >= moveInterval)
        {
            timer = 0f;
            Step();
        }
    }

    void Step()
    {
        var action = agent.ChooseAction(currentPos);
        var nextPos = agent.GetNextState(currentPos, action);

        float reward = -0.1f; // 默认小惩罚鼓励更快完成

        if (nextPos == targetPos)
        {
            reward = 10f;
            taskCompleted = true;
            float timeUsed = Time.time - taskStartTime;
            float efficiency = totalDistance / timeUsed;
            Debug.Log($"{name} finished! Time: {timeUsed:F2}s, Efficiency: {efficiency:F2}");
        }
        else if (Vector2Int.Distance(nextPos, targetPos) < Vector2Int.Distance(currentPos, targetPos))
        {
            reward = 0.5f;
        }
        else if (nextPos == currentPos)
        {
            reward = -1f; // 撞墙或越界
        }

        agent.UpdateQValue(currentPos, action, reward, nextPos);
        currentPos = nextPos;
        transform.position = GameController.Instance.GridToWorld(currentPos) + Vector3.up * 0.5f;

        Vector3 worldPos = GameController.Instance.GridToWorld(currentPos) + Vector3.up * 0.05f;
        pathPoints.Add(worldPos);
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 当前目标
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GameController.Instance.GridToWorld(targetPos) + Vector3.up * 0.2f, 0.3f);

        // 当前所在格子
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GameController.Instance.GridToWorld(currentPos) + Vector3.up * 0.2f, 0.3f);

        // 方向箭头（向所有可能动作）
        Gizmos.color = Color.yellow;
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int next = currentPos + dir;
            if (GameController.Instance.IsInsideGrid(next))
            {
                Vector3 from = GameController.Instance.GridToWorld(currentPos) + Vector3.up * 0.2f;
                Vector3 to = GameController.Instance.GridToWorld(next) + Vector3.up * 0.2f;
                Gizmos.DrawLine(from, to);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (agent != null)
        {
            agent.SaveQTable(qTablePath);
            

        }
    }
}
