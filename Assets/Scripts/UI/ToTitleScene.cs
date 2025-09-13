using UnityEngine;
using DG.Tweening;

public class ToTitleScene : MonoBehaviour
{
    public void Load()
    {
        SceneLoader.Instance.LoadScene("TitleScene");   
    }
}