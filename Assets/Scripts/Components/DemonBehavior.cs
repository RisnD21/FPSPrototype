using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DemonBehavior : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] SpriteRenderer sprite; 

    [Header("Config")]
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration  = 0.25f;

    Tween fadeTween;


    public void Proceed()
    {
        FadeThenTeleportTo(targetPos);
    }

    public void Leave()
    {
        FadeThenTeleportTo(endPos);
    }

    void FadeThenTeleportTo(Vector3 pos)
    {
        fadeTween = sprite
            .DOFade(0f, fadeOutDuration)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
            .OnComplete(() => TeleportThenAppear(pos));
    }

    void TeleportThenAppear(Vector3 pos)
    {
        transform.position = pos;

        fadeTween = sprite
            .DOFade(1f, fadeInDuration)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }
}
