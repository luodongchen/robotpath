using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Grid Settings")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 2f;
    public GameObject floorTilePrefab;

    [Header("Robot Settings")]
    public GameObject robotPrefab;
    public int robotCount = 3;

    private bool[,] walkableGrid;
    private List<float> taskEfficiencies = new List<float>();

    void Awake() => Instance = this;

    void Start()
    {
        GenerateGrid();
        SpawnMultipleRobots(robotCount);
        PositionCamera();
    }

    void GenerateGrid()
    {
        walkableGrid = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                walkableGrid[x, y] = true;
            }
        }
    }

    void SpawnMultipleRobots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int start = new Vector2Int(Random.Range(0, width / 2), Random.Range(0, height));
            Vector2Int end = new Vector2Int(Random.Range(width / 2, width), Random.Range(0, height));
            SpawnRobot(start, end);
        }
    }

    void SpawnRobot(Vector2Int startGrid, Vector2Int targetGrid)
    {
        Vector3 worldStart = GridToWorld(startGrid);
        GameObject robotObj = Instantiate(robotPrefab, worldStart + Vector3.up * 0.5f, Quaternion.identity);
        RobotController rc = robotObj.GetComponent<RobotController>();
        rc.Initialize(startGrid, targetGrid);
    }

    public Vector3 GridToWorld(Vector2Int gridPos) =>
        new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);

    public bool[,] GetWalkableGrid() => walkableGrid;

    public void ReportTaskEfficiency(float efficiency)
    {
        taskEfficiencies.Add(efficiency);
        Debug.Log($"任务完成效率: {efficiency:F2} （共{taskEfficiencies.Count}个任务）");
    }

    void PositionCamera()
    {
        if (Camera.main != null)
        {
            float midX = (width * cellSize) / 2f;
            float midZ = (height * cellSize) / 2f;
            float camHeight = Mathf.Max(width, height) * 1.5f;
            Camera.main.transform.position = new Vector3(midX, camHeight, midZ);
            Camera.main.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
