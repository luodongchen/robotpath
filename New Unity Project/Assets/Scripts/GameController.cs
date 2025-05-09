using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public GameObject robotPrefab;

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
        SpawnRobot();
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

    // 将网格坐标转换为世界坐标
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0f, gridPos.y * cellSize);
    }

    // 判断坐标是否在网格范围内
    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    // 生成一个机器人并初始化其路径任务
    void SpawnRobot()
    {
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int target = new Vector2Int(width - 1, height - 1);

        GameObject robot = Instantiate(robotPrefab);
        robot.name = "Robot";
        robot.GetComponent<RobotController>().Initialize(start, target);
    }
}
