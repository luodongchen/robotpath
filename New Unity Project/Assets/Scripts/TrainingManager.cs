using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TrainingManager : MonoBehaviour
{

    public static TrainingManager Instance { get; private set; } // ✅ 单例实例

    public GameObject robotPrefab;
    public Transform robotParent;
    public Text episodeText;
    public Text efficiencyText;

    public int totalEpisodes = 100;
    public float delayBetweenEpisodes = 0.5f;

    private int currentEpisode = 0;
    private float cumulativeEfficiency = 0f;
    private RobotController currentRobot;

    private void Awake()
    {
        // ✅ 设置单例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(TrainEpisodes());
    }

    IEnumerator TrainEpisodes()
    {
        for (currentEpisode = 1; currentEpisode <= totalEpisodes; currentEpisode++)
        {
            StartOneEpisode();
            yield return new WaitUntil(() => currentRobot.TaskCompleted);
            cumulativeEfficiency += currentRobot.LastEfficiency;
            UpdateUI();
            yield return new WaitForSeconds(delayBetweenEpisodes);
        }

        Debug.Log($"训练完成！平均效率: {(cumulativeEfficiency / totalEpisodes):F2}");
    }

    void StartOneEpisode()
    {
        if (currentRobot != null)
            Destroy(currentRobot.gameObject);

        Vector2Int start = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));
        Vector2Int target = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));
        while (target == start)
            target = new Vector2Int(Random.Range(0, GameController.Instance.width), Random.Range(0, GameController.Instance.height));

        GameObject robot = Instantiate(robotPrefab, robotParent);
        currentRobot = robot.GetComponent<RobotController>();
        currentRobot.Initialize(start, target);
    }

    void UpdateUI()
    {
        episodeText.text = $"Episode: {currentEpisode}/{totalEpisodes}";
        efficiencyText.text = $"Avg Efficiency: {(cumulativeEfficiency / currentEpisode):F2}";
    }

    public void RecordEfficiency(float efficiency)
    {
        cumulativeEfficiency += efficiency;
        UpdateUI();
    }

}
