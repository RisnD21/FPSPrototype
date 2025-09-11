using UnityEngine;

public class PanelController : MonoBehaviour
{
    void CloseOnEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    void Update() 
    {
        CloseOnEsc();
    }
}
