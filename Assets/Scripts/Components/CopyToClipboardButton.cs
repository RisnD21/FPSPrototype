using UnityEngine;
using UnityEngine.UI;

public class CopyToClipboardButton : MonoBehaviour
{
    [SerializeField] private Button copyButton;
    [SerializeField] private string textToCopy = "risen1221hyw@gmail.com";

    void Awake()
    {
        if (copyButton != null)
        {
            copyButton.onClick.AddListener(CopyText);
        }
    }

    void CopyText()
    {
        GUIUtility.systemCopyBuffer = textToCopy;
        Debug.Log($"已複製到剪貼簿：{textToCopy}");
    }
}
