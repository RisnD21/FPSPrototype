using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreScene");
        }
    }
}