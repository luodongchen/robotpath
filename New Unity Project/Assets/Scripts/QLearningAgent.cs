using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class QLearningAgent
{
    public Vector2Int[] Directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private float learningRate = 0.1f;
    private float discountFactor = 0.95f;
    private float epsilon = 0.2f;

    // Q 表: 状态-目标 对应的动作 Q 值
    private Dictionary<string, float[]> qTable = new Dictionary<string, float[]>();

    public Vector2Int ChooseAction(Vector2Int state, Vector2Int goal)
    {
        string key = GetStateKey(state, goal);

        if (!qTable.ContainsKey(key))
            qTable[key] = new float[Directions.Length];

        if (UnityEngine.Random.value < epsilon)
        {
            // 探索
            int rand = UnityEngine.Random.Range(0, Directions.Length);
            return Directions[rand];
        }
        else
        {
            // 利用
            float[] qValues = qTable[key];
            int bestIndex = 0;
            float bestValue = qValues[0];
            for (int i = 1; i < qValues.Length; i++)
            {
                if (qValues[i] > bestValue)
                {
                    bestValue = qValues[i];
                    bestIndex = i;
                }
            }
            return Directions[bestIndex];
        }
    }

    public void UpdateQ(Vector2Int state, Vector2Int goal, int actionIndex, float reward, Vector2Int nextState)
    {
        string key = GetStateKey(state, goal);
        string nextKey = GetStateKey(nextState, goal);

        if (!qTable.ContainsKey(key))
            qTable[key] = new float[Directions.Length];

        if (!qTable.ContainsKey(nextKey))
            qTable[nextKey] = new float[Directions.Length];

        float maxNextQ = Mathf.Max(qTable[nextKey]);
        float currentQ = qTable[key][actionIndex];

        qTable[key][actionIndex] = currentQ + learningRate * (reward + discountFactor * maxNextQ - currentQ);
    }

    private string GetStateKey(Vector2Int pos, Vector2Int goal)
    {
        return $"{pos.x},{pos.y}:{goal.x},{goal.y}";
    }

    // ===== 持久化方法 =====

    [Serializable]
    private class QTableWrapper
    {
        public List<string> keys = new();
        public List<float[]> values = new();
    }

    public void SaveQTable(string filePath)
    {
        QTableWrapper wrapper = new QTableWrapper();
        foreach (var kv in qTable)
        {
            wrapper.keys.Add(kv.Key);
            wrapper.values.Add(kv.Value);
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Q表已保存到：{filePath}");
    }

    public void LoadQTable(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Q表文件不存在：{filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        QTableWrapper wrapper = JsonUtility.FromJson<QTableWrapper>(json);

        qTable.Clear();
        for (int i = 0; i < wrapper.keys.Count; i++)
        {
            qTable[wrapper.keys[i]] = wrapper.values[i];
        }

        Debug.Log($"Q表已加载：{filePath}");
    }
}
