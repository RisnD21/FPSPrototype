using TMPro;
using UnityEngine;

public class StateMonitor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI state;
    [SerializeField] TextMeshProUGUI meter;

    void Start()
    {
        UpdateState("");
        UpdateMeter(0);
    }

    public void UpdateMeter(float value)
    {
        int intValue = Mathf.FloorToInt(value);
        meter.text = "Alert " + intValue.ToString();
    }

    public void UpdateState(string value)
    {
        state.text = value;
    }
}
