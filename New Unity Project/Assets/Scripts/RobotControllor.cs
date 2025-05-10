using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed = 2f;

    private Vector2Int start;
    private Vector2Int target;
    private Vector2Int gridPos;
    private Queue<Vector3> path;
    private bool taskCompleted = false;
    private float startTime;
    private float pathLength;
    private float elapsedTime;

    public float LastEfficiency { get; private set; }
    public bool TaskCompleted => taskCompleted;

    public QLearningAgent agent = new QLearningAgent();

    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public void Initialize(Vector2Int start, Vector2Int target)
    {
        this.start = start;
        this.target = target;
        this.gridPos = start;

        transform.position = GameController.Instance.GridToWorld(start);

        path = new Queue<Vector3>(AStarPathfinder.FindPath(start, target, GameController.Instance.grid));
        pathLength = path.Count;
        taskCompleted = false;
        startTime = Time.time;
    }

    void Update()
    {
        if (taskCompleted || path == null || path.Count == 0)
            return;

        Step();
    }

    void Step()
    {
        Vector3 nextPos = path.Peek();
        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos) < 0.01f)
        {
            path.Dequeue();
            gridPos = GameController.Instance.WorldToGrid(transform.position);

            if (gridPos == target)
            {
                taskCompleted = true;
                elapsedTime = Time.time - startTime;
                LastEfficiency = pathLength / Mathf.Max(elapsedTime, 0.01f);
                Debug.Log($"任务完成！效率: {LastEfficiency:F2}");

                return;
            }

            // 选择下一动作（局部 Q-learning 逻辑）
            Vector2Int moveDir = agent.ChooseAction(gridPos, target);
            Vector2Int newGridPos = gridPos + moveDir;

            if (GameController.Instance.IsWalkable(newGridPos))
            {
                int actionIndex = System.Array.IndexOf(directions, moveDir);
                float reward = -0.1f;
                if (newGridPos == target) reward = 1f;

                agent.UpdateQ(gridPos, target, actionIndex, reward, newGridPos);
                gridPos = newGridPos;

                transform.position = GameController.Instance.GridToWorld(gridPos);
            }
        }
    }

    private void OnDestroy()
    {
        // 可选：保存 Q 表
        agent.SaveQTable(Application.persistentDataPath + "/qtable.json");
    }
}
