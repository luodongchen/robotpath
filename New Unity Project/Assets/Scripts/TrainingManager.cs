using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance { get; private set; }

    public GameObject robotPrefab;
    public Transform robotParent;
    public Text episodeText;
    public Text efficiencyText;

    public int totalEpisodes = 100;
    public float delayBetweenEpisodes = 0.5f;
    public int robotCount = 20; // ✅ 同时训练机器人数量

    private int currentEpisode = 0;
    private float cumulativeEfficiency = 0f;

    private List<RobotController> robots = new List<RobotController>();
    private QLearningAgent sharedAgent = new QLearningAgent();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        sharedAgent.LoadQTable(Application.persistentDataPath + "/qtable.json");
        StartCoroutine(Train());
    }

    IEnumerator Train()
    {
        for (currentEpisode = 1; currentEpisode <= totalEpisodes; currentEpisode++)
        {
            StartOneEpisode();

            yield return new WaitUntil(() => AllRobotsCompleted());

            float episodeEfficiency = 0f;
            foreach (var robot in robots)
                episodeEfficiency += robot.LastEfficiency;

            cumulativeEfficiency += episodeEfficiency / robotCount;

            UpdateUI();

            foreach (var robot in robots)
                Destroy(robot.gameObject);
            robots.Clear();

            yield return new WaitForSeconds(delayBetweenEpisodes);
        }

        sharedAgent.SaveQTable(Application.persistentDataPath + "/qtable.json");
        Debug.Log($"训练完成！平均效率: {(cumulativeEfficiency / totalEpisodes):F2}");
    }

    void StartOneEpisode()
    {
        robots.Clear();

        for (int i = 0; i < robotCount; i++)
        {
            Vector2Int start = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));
            Vector2Int target = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));
            while (target == start)
                target = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));

            GameObject robot = Instantiate(robotPrefab, robotParent);
            RobotController controller = robot.GetComponent<RobotController>();
            controller.agent = sharedAgent; // ✅ 所有机器人使用共享 Q-learning
            controller.Initialize(start, target);
            robots.Add(controller);
        }
    }

    bool AllRobotsCompleted()
    {
        foreach (var robot in robots)
        {
            if (!robot.TaskCompleted)
                return false;
        }
        return true;
    }

    void UpdateUI()
    {
        episodeText.text = $"Episode: {currentEpisode}/{totalEpisodes}";
        efficiencyText.text = $"Avg Efficiency: {(cumulativeEfficiency / currentEpisode):F3}";
    }

    public void RecordEfficiency(float efficiency)
    {
        cumulativeEfficiency += efficiency;
        UpdateUI();
    }

}
