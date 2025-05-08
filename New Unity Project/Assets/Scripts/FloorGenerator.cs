using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public int width = 200;
    public int height = 200;
    public float cellSize = 1f;
    public Material floorMaterial;

    void Start()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.parent = transform;
        floor.transform.localScale = new Vector3(width * cellSize / 10f, 1f, height * cellSize / 10f);
        floor.transform.position = new Vector3((width * cellSize) / 2f - cellSize / 2f, 0f, (height * cellSize) / 2f - cellSize / 2f);

        if (floorMaterial != null)
        {
            floor.GetComponent<Renderer>().material = floorMaterial;
        }
    }
}
