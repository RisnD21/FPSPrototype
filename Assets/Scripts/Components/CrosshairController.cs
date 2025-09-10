using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] Transform[] crosshairParts; // 上下左右四個部分
    [SerializeField] Vector2 crosshairOffset;
    [SerializeField] float initExpandDistance = 10f; // 擴張距離
    [SerializeField] float maxExpandDistance = 3f; // 最大擴張距離
    [SerializeField] Weapon weapon1; 
    [SerializeField] Weapon weapon2; 

    float expandAmount = 0f; // 當前擴張量
    [Range(0,1)][SerializeField] float accuracy;

    void Awake() 
    {
        if (crosshairParts.Length < 4) Debug.LogError("Crosshair parts not assigned properly");
        if (weapon1 == null) Debug.LogError("Weapon1 not assigned");
        if (weapon1 == null) Debug.LogError("Weapon2 not assigned");
        Cursor.visible = false;
    }

    void OnEnable() 
    {
        Cursor.visible = false;
    }

    void OnDisable() 
    {
        Cursor.visible = true;
    }

    void ApplyExpand()
    {
        FetchAccuracy();
        float targetExpand = (1f - accuracy) * maxExpandDistance;
        SetCrosshairExpand(targetExpand);
    }

    void FetchAccuracy()
    {
        if(weapon1 != null && weapon1.isActiveAndEnabled)
        {
            accuracy = Mathf.Clamp01(weapon1.Accuracy + 0.05f);
        }
        else if(weapon2 != null && weapon2.isActiveAndEnabled)
        {
            accuracy = Mathf.Clamp01(weapon2.Accuracy + 0.05f);
        } else accuracy = 1f;
    }

    void SetCrosshairExpand(float amount)
    {
        expandAmount = amount;

        foreach(var part in crosshairParts)
        {
            var expandDirection = (part.up + -part.right).normalized;

            part.localPosition = (initExpandDistance + expandAmount) * expandDirection;
        }
    }

    void Update() 
    {
        UpdateCrosshairPosition();
        ApplyExpand();
    }

    void UpdateCrosshairPosition()
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = mousePos + crosshairOffset;
    }
}