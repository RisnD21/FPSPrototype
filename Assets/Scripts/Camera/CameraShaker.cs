using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance {get; private set;}

    float shakeRemainTime;
    float shakeStrength;
    Vector3 originalPos;
    public bool hasControl;
    

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    // void Update()
    // {
    //     if (shakeRemainTime > 0)
    //     {

    //         shakeRemainTime -= Time.deltaTime;
    //         StartShake();
            
    //         if(shakeRemainTime <= 0) hasControl = false;
    //     }
    // }

    void StartShake()
    {
        Vector3 randomDir = Random.insideUnitSphere;
        transform.localPosition = originalPos + randomDir * shakeStrength;
    }

    public void Shake(Vector2 direction, float duration, float magnitude)
    {
        // hasControl = true;
        // originalPos = transform.position;
        // shakeStrength = magnitude;
        // shakeRemainTime = duration;

        transform.localPosition = transform.localPosition + (Vector3) direction * magnitude;
    }
}
