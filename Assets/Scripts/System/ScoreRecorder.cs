using UnityEngine;

public class ScoreRecorder : MonoBehaviour
{
    public static ScoreRecorder Instance;

    float initTime;
    float gameLength;
    int shotsFired;
    int shotsHit;
    int enemiesKilled;
    float accuracy;
    float lifeLoss;
    float lifeRecover;

    float lifeTimeGameLength;
    int lifeTimeKills;
    int lifeTimeDeath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void HasShot() => shotsFired++;
    public void HasHit() => shotsHit++;
    public void HasKilled() => enemiesKilled++;
    public void HasLostLife(float value) => lifeLoss+=value;
    public void HasRecoveredLife(float value) => lifeRecover+=value;

    void OnEnable()
    {
        initTime = Time.time;
    }

    void OnDisable()
    {
        gameLength = Time.time - initTime;
        accuracy = (shotsFired == 0) ? 0 : (float)shotsHit / shotsFired * 100f;

        

    }


}
