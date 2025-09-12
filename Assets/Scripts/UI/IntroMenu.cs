using UnityEngine;

public class IntroMenu : MonoBehaviour
{
    [SerializeField] GameObject controls;

    public void ShowControls()
    {
        controls.SetActive(true);
    }

    public void ToGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}