using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float speed = 3f;
    public Vector2Int gridPos;
    public Vector2Int target;
    public bool TaskCompleted { get; private set; }
    public float LastEfficiency { get; private set; }

    public QLearningAgent agent = new QLearningAgent();
    private Vector2Int[] directions => agent.Directions;

    private float pathLength;
    private float startTime;

    public void Initialize(Vector2Int start, Vector2Int goal)
    {
        gridPos = start;
        target = goal;
        transform.position = GameController.Instance.GridToWorld(start);
        pathLength = 0;
        startTime = Time.time;
        TaskCompleted = false;
    }

    void Update()
    {
        if (TaskCompleted) return;

        Vector2Int move = agent.ChooseAction(gridPos, target);
        Vector2Int nextPos = gridPos + move;

        float reward = -0.1f;
        if (!GameController.Instance.IsWalkable(nextPos))
        {
            reward = -1f;
        }
        else
        {
            float oldDist = Vector2Int.Distance(gridPos, target);
            float newDist = Vector2Int.Distance(nextPos, target);
            reward += 0.05f * (oldDist - newDist);

            if (nextPos == target)
            {
                reward = 1f;
                TaskCompleted = true;
                LastEfficiency = 1f / (Time.time - startTime);
                TrainingManager.Instance.RecordEfficiency(LastEfficiency);
            }

            int actionIndex = System.Array.IndexOf(directions, move);
            agent.UpdateQ(gridPos, nextPos, actionIndex, reward);
            gridPos = nextPos;
            transform.position = GameController.Instance.GridToWorld(gridPos);
            pathLength++;
        }
    }
}
