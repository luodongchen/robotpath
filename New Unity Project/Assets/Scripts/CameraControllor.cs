using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int gridWidth = 100;
    public int gridHeight = 100;
    public float cellSize = 1f;

    void Start()
    {
        PositionCamera();
    }

    public void Initialize(int width, int height, float cell)
    {
        gridWidth = width;
        gridHeight = height;
        cellSize = cell;
        PositionCamera();
    }

    void PositionCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float midX = (gridWidth* cellSize) / 2f;
        float midZ = (gridHeight * cellSize) / 2f;

        float size = Mathf.Max(gridWidth, gridHeight) * cellSize * 0.6f;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = size;
        Camera.main.transform.position = new Vector3(midX, 100f, midZ);
        Camera.main.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
