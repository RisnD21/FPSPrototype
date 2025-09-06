using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(CinemachineCameraOffset))]
public class CameraMouseOffset : MonoBehaviour
{
    [Header("Tuning")]
    [SerializeField] float maxOffset = 10f;           // 偏移距離（世界單位）
    [SerializeField] float tweenDuration = 0.5f;    // 追目標的補間時間
    [SerializeField] Ease easeType = Ease.OutQuad;   // 速度曲線

    [Header("State (set from PlayerInput)")]
    [SerializeField] PlayerInput playerInput;

    CinemachineCameraOffset camOffset;
    Transform follow;
    Tween offsetTween;
    CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        follow = vcam.Follow;
        camOffset = GetComponent<CinemachineCameraOffset>();
    }

    void OnDisable()
    {
        // 元件被停用時，確保 tween 回收並歸零
        if (offsetTween != null) { offsetTween.Kill(); offsetTween = null; }
        if (camOffset != null) camOffset.Offset = Vector3.zero;
    }

    void LateUpdate()
    {
        if (playerInput == null) return;
        if (playerInput.isAiming)
        {
            var targetOffset = CalculateCamOffset();

            offsetTween?.Kill();

            offsetTween = DOTween
                .To(() => camOffset.Offset, x => camOffset.Offset = x, targetOffset, tweenDuration)
                .SetEase(easeType)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
        else
        {
            offsetTween?.Kill();

            offsetTween = DOTween
                .To(() => camOffset.Offset, x => camOffset.Offset = x, Vector3.zero, tweenDuration)
                .SetEase(easeType)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
    }

    Vector3 CalculateCamOffset()
    {
        var cam = Camera.main;
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldProj = cam.ScreenToWorldPoint(mouseScreenPos);

        mouseWorldProj.z = follow.position.z;
        Vector3 playerToMouse = mouseWorldProj - follow.position;
        playerToMouse.z = 0f;

        if (playerToMouse.sqrMagnitude > 1e-6f) playerToMouse.Normalize(); 
        else return Vector3.zero;
        
        Vector3 camSpaceDir = vcam.transform.InverseTransformDirection(playerToMouse);
        Vector3 camPlanar = new (camSpaceDir.x, camSpaceDir.y, 0f);

        return camPlanar.sqrMagnitude > 1e-6f ? camPlanar.normalized * maxOffset : Vector3.zero;
    }
}