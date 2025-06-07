using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public bool[,] grid;  // 网格数据，true 表示可走

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new bool[width, height];

        // 默认全部可走
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = true;
            }
        }

        // 示例：添加障碍
        // grid[3, 3] = false;
        // grid[4, 3] = false;
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0, gridPos.y * cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
        );
    }

    public bool IsWalkable(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x < width && pos.y < height &&
               grid[pos.x, pos.y];
    }
}
