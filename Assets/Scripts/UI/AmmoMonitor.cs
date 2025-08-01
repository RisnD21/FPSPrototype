using TMPro;
using UnityEngine;

public class AmmoMonitor : MonoBehaviour
{
    TextMeshProUGUI monitor;

    void Awake()
    {
        monitor = GetComponent<TextMeshProUGUI>();
    }
    
    public void SyncAmmo(int current, int reserved)
    {
        monitor.text = $"{current}/{reserved}";
    }
}