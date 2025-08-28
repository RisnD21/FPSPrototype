using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TooltipHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float showDelay = 0.3f; // 反跳，避免太躁動
    [SerializeField] bool followMouse = true; // 是否跟著滑鼠
    [SerializeField] Transform worldAnchor;    // 如果是 NPC/3D 物件可指定錨點（選填）

    string staticText;                 // 99% 用這個
    Func<string> textProvider;         // 少數情況：需要動態更新
    bool isHovering;
    float refreshTimer;
    [SerializeField] float refreshInterval = 0.25f; // 動態來源的更新頻率
    Coroutine delayRoutine;

    public void SetTooltip(string text) => staticText = text;
    public void SetTooltip(Func<string> provider) => textProvider = provider; // 動態版（非必須）
    public void SetWorldAnchor(Transform t) => worldAnchor = t;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isHovering) return;

        if (delayRoutine != null) StopCoroutine(delayRoutine);
        delayRoutine = StartCoroutine(ShowAfterDelay(eventData));
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (delayRoutine != null) StopCoroutine(delayRoutine);
        if(TooltipManager.Instance != null) TooltipManager.Instance.Hide();
    }

    void OnDisable()
    {
        isHovering = false;
        if (delayRoutine != null) StopCoroutine(delayRoutine);
        if(TooltipManager.Instance != null) TooltipManager.Instance.Hide();
    }

    System.Collections.IEnumerator ShowAfterDelay(PointerEventData eventData)
    {
        yield return new WaitForSeconds(showDelay);
        if (!isHovering) yield break;

        string content = textProvider != null ? textProvider() : staticText;
        Vector2 pos = GetScreenPosition(eventData);

        if(TooltipManager.Instance != null) TooltipManager.Instance.Show(content, pos);
        refreshTimer = 0f;
    }

    void Update()
    {
        if (!isHovering || TooltipManager.Instance == null) return;

        // 位置更新（滑鼠或世界座標）
        Vector2 pos = GetScreenPosition();
        TooltipManager.Instance.UpdatePosition(pos);

        // 動態內容更新（可選）
        if (textProvider != null)
        {
            refreshTimer += Time.unscaledDeltaTime;
            if (refreshTimer >= refreshInterval)
            {
                refreshTimer = 0f;
                TooltipManager.Instance.Show(textProvider(), pos);
            }
        }
    }

    Vector2 GetScreenPosition(PointerEventData e = null)
    {
        if (worldAnchor != null)
        {
            var sp = Camera.main != null ? Camera.main.WorldToScreenPoint(worldAnchor.position) : Input.mousePosition;
            return sp;
        }
        if (followMouse || e == null) return Input.mousePosition;
        return e.position;
    }
}
