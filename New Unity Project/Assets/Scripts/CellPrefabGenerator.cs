using UnityEngine;

public static class CellPrefabGenerator
{
    public static GameObject CreateCell(float size)
    {
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cell.name = "GridCell";
        cell.transform.localScale = new Vector3(size, size, 1f);
        cell.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 把Quad立起来当地面
        cell.GetComponent<Renderer>().material = GenerateMaterial();

        return cell;
    }

    private static Material GenerateMaterial()
    {
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = new Color(0.85f, 0.85f, 0.85f); // 淡灰色
        return mat;
    }
}
