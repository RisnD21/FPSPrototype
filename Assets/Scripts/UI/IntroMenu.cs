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
        SceneLoader.Instance.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}