using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) 
        {
            SceneLoader.Instance.LoadScene("ScoreScene");
        }
    }
}