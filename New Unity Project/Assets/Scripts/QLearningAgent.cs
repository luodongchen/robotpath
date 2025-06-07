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
    public float epsilon = 0.3f;  // 初始探索率

    private Dictionary<string, float[]> qTable = new Dictionary<string, float[]>();

    public Vector2Int ChooseAction(Vector2Int state, Vector2Int goal)
    {
        string key = GetStateKey(state);
        if (!qTable.ContainsKey(key))
            qTable[key] = new float[Directions.Length];

        if (UnityEngine.Random.value < epsilon)
            return Directions[UnityEngine.Random.Range(0, Directions.Length)];
        
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

    public void UpdateQ(Vector2Int state, Vector2Int nextState, int actionIndex, float reward)
    {
        string key = GetStateKey(state);
        string nextKey = GetStateKey(nextState);

        if (!qTable.ContainsKey(key))
            qTable[key] = new float[Directions.Length];
        if (!qTable.ContainsKey(nextKey))
            qTable[nextKey] = new float[Directions.Length];

        float maxNextQ = Mathf.Max(qTable[nextKey]);
        float currentQ = qTable[key][actionIndex];
        qTable[key][actionIndex] = currentQ + learningRate * (reward + discountFactor * maxNextQ - currentQ);
    }

    string GetStateKey(Vector2Int pos)
    {
        return $"{pos.x},{pos.y}";
    }

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
    }

    public void LoadQTable(string filePath)
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        QTableWrapper wrapper = JsonUtility.FromJson<QTableWrapper>(json);
        qTable.Clear();
        for (int i = 0; i < wrapper.keys.Count; i++)
        {
            qTable[wrapper.keys[i]] = wrapper.values[i];
        }
    }
}
