using TMPro;
using UnityEngine;

public class ScorePrinter : MonoBehaviour
{
    float gameLength;
    int shotsFired;
    int enemiesKilled;
    float accuracy;
    float lifeLoss;
    float lifeRecover;

    float lifeTimeGameLength;
    int lifeTimeKills;
    int lifeTimeDeath;

    [SerializeField] TextMeshProUGUI gameLengthText;
    [SerializeField] TextMeshProUGUI shotsFiredText;
    [SerializeField] TextMeshProUGUI enemiesKilledText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI lifeLossText;
    [SerializeField] TextMeshProUGUI lifeRecoverText;
    [SerializeField] TextMeshProUGUI lifeTimeGameLengthText;
    [SerializeField] TextMeshProUGUI lifeTimeKillsText;
    [SerializeField] TextMeshProUGUI lifeTimeDeathText;

    void Start()
    {
        LoadStatistics();
        PrintScore();
    }

    void LoadStatistics()
    {
        gameLength = PlayerPrefs.GetFloat("GameLength", 0);
        shotsFired = PlayerPrefs.GetInt("ShotsFired", 0);
        enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
        accuracy = PlayerPrefs.GetFloat("Accuracy", 0);
        lifeLoss = PlayerPrefs.GetFloat("LifeLoss", 0);
        lifeRecover = PlayerPrefs.GetFloat("LifeRecover", 0);
        lifeTimeGameLength = PlayerPrefs.GetFloat("LifeTimeGameLength", 0);
        lifeTimeKills = PlayerPrefs.GetInt("LifeTimeEnemiesKilled", 0);
        lifeTimeDeath = PlayerPrefs.GetInt("LifeTimeDeath", 0);
    }

    void PrintScore()
    {
        gameLengthText.text = $"單局遊戲時長 {gameLength:F1} s";
        shotsFiredText.text = $"射擊次數 {shotsFired}";
        accuracyText.text = $"命中率 {accuracy:F1} %";
        enemiesKilledText.text = $"擊殺數 {enemiesKilled}";
        lifeLossText.text = $"受到傷害 {lifeLoss:F1}";
        lifeRecoverText.text = $"恢復生命 {lifeRecover:F1}";
        lifeTimeGameLengthText.text = $"總遊戲時長 {lifeTimeGameLength:F1} s";
        lifeTimeKillsText.text = $"總擊殺數 {lifeTimeKills}";
        lifeTimeDeathText.text = $"總死亡數 {lifeTimeDeath}";
    }

    void CalculateScore()
    {
        
    }
}
