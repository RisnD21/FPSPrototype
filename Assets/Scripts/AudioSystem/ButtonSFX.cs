using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AudioSystem.SFX;

//Add this on buttons that require hover, click sound effect
[RequireComponent(typeof(Button))]
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Start()
    {
        if (TryGetComponent<Button>(out var btn))
        {
            btn.onClick.AddListener(() =>
            {
                if (SFXManager.Instance != null) SFXManager.Instance.PlaySound(SoundID.ClickButton);
            });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound(SoundID.Select);
        }
        transform.localScale = Vector3.one * 1.1f;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1f; 
    }
}
