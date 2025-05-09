using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class QLearningAgent
{
    [Serializable]
    public struct StateActionKey
    {
        public int x, y;
        public string action;

        public StateActionKey(Vector2Int state, QLearningAgent.Action action)
        {
            this.x = state.x;
            this.y = state.y;
            this.action = action.ToString();
        }

        public Vector2Int ToVector2Int() => new Vector2Int(x, y);
        public QLearningAgent.Action ToAction() => (QLearningAgent.Action)Enum.Parse(typeof(QLearningAgent.Action), action);
    }

    [Serializable]
    public class QTableData
    {
        public List<StateActionKey> keys = new List<StateActionKey>();
        public List<float> values = new List<float>();
    }

    private Dictionary<StateActionKey, float> qTable = new Dictionary<StateActionKey, float>();
    private float learningRate = 0.1f;
    private float discountFactor = 0.9f;
    private float explorationRate = 0.2f;
    private int gridWidth, gridHeight;

    public enum Action { Up, Down, Left, Right }

    public QLearningAgent(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
    }

    public Vector2Int GetNextState(Vector2Int current, Action action)
    {
        Vector2Int next = current;
        switch (action)
        {
            case Action.Up: next += Vector2Int.up; break;
            case Action.Down: next += Vector2Int.down; break;
            case Action.Left: next += Vector2Int.left; break;
            case Action.Right: next += Vector2Int.right; break;
        }
        if (next.x < 0 || next.x >= gridWidth || next.y < 0 || next.y >= gridHeight)
            return current;
        return next;
    }

    public Action ChooseAction(Vector2Int state)
    {
        if (UnityEngine.Random.value < explorationRate)
            return (Action)UnityEngine.Random.Range(0, 4);

        float maxQ = float.MinValue;
        Action bestAction = Action.Up;

        foreach (Action a in Enum.GetValues(typeof(Action)))
        {
            float q = GetQValue(state, a);
            if (q > maxQ)
            {
                maxQ = q;
                bestAction = a;
            }
        }
        return bestAction;
    }

    public void UpdateQValue(Vector2Int state, Action action, float reward, Vector2Int nextState)
    {
        float currentQ = GetQValue(state, action);
        float maxNextQ = float.MinValue;

        foreach (Action nextAction in Enum.GetValues(typeof(Action)))
        {
            float nextQ = GetQValue(nextState, nextAction);
            if (nextQ > maxNextQ) maxNextQ = nextQ;
        }

        float newQ = currentQ + learningRate * (reward + discountFactor * maxNextQ - currentQ);
        SetQValue(state, action, newQ);
    }

    public float GetQValue(Vector2Int state, Action action)
    {
        var key = new StateActionKey(state, action);
        if (qTable.TryGetValue(key, out float value))
            return value;
        return 0f;
    }

    public void SetQValue(Vector2Int state, Action action, float value)
    {
        var key = new StateActionKey(state, action);
        qTable[key] = value;
    }

    public void SaveQTable(string filePath)
    {
        QTableData data = new QTableData();
        foreach (var kv in qTable)
        {
            data.keys.Add(kv.Key);
            data.values.Add(kv.Value);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Q-table saved to " + filePath);
    }

    public void LoadQTable(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Q-table file not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        QTableData data = JsonUtility.FromJson<QTableData>(json);

        qTable.Clear();
        for (int i = 0; i < data.keys.Count; i++)
        {
            qTable[data.keys[i]] = data.values[i];
        }

        Debug.Log("Q-table loaded from " + filePath);
    }
}
