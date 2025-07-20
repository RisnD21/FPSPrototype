using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance {get; private set;}
    [SerializeField] Color negative;
    [SerializeField] Color positive;

    [SerializeField] Canvas textContainer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Initialize()
    {
        PoolSystem.Instance.InitPool(textContainer, 20);
    }
    public void SpawnText(int value, Vector3 pos, float duration = 1)
    {
        var canvas = PoolSystem.Instance.GetInstance<Canvas>(textContainer);
        canvas.worldCamera = Camera.main;
        
        var panel = canvas.GetComponentInChildren<TextMeshProUGUI>();

        panel.transform.position = pos;
        panel.transform.position += 0.5f * (Vector3) Random.insideUnitCircle;
        panel.color = value >= 0 ? positive : negative;
        panel.text = value.ToString();

        UpdateRect(panel, duration);
    }

    void UpdateRect(TextMeshProUGUI panel, float duration)
    {
        var rt = panel.rectTransform;
        Vector3 endPos = rt.transform.position + new Vector3(0,1,0);

        rt.localScale = Vector3.zero;
        panel.alpha = 1;

        Sequence seq = DOTween.Sequence();
        // Scale up
        seq.Append(rt.DOScale(Vector3.one, duration/6).SetEase(Ease.OutBack));
        // 停留一段時間
        seq.AppendInterval(duration * 0.667f);
        // 同步加入 Fade，從 alpha=1→0 整段時間跑完
        seq.Join(panel.DOFade(0, duration*5/6).From(1));
        // Scale down
        seq.Append(rt.DOMove(endPos, duration/6));
    }
}