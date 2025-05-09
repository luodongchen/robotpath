using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public GameObject robotPrefab;
    public int robotCount = 1;

    private Transform robotParent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitCamera();
        SpawnRobots(5);
    }

    // 初始化摄像机
    void InitCamera()
    {
        CameraController camCtrl = Camera.main.GetComponent<CameraController>();
        if (camCtrl != null)
        {
            camCtrl.Initialize(width, height, cellSize);
        }
    }

    // 网格转世界坐标
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0f, gridPos.y * cellSize);
    }

    // 判断是否在网格内
    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    // 批量生成机器人
    void SpawnRobots(int count)
    {
        if (robotParent == null)
        {
            robotParent = new GameObject("Robots").transform;
        }

        for (int i = 0; i < count; i++)
        {
            Vector2Int start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            Vector2Int target = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            while (target == start)
            {
                target = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            }

            GameObject robot = Instantiate(robotPrefab, robotParent);
            robot.name = $"Robot_{i}";
            var controller = robot.GetComponent<RobotController>();
            controller.Initialize(start, target);
        }
    }
}
