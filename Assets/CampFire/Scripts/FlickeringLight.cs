using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlickeringLight : MonoBehaviour
{
    Light2D fireLight;
    float baseInnerRadius;
    float baseOuterRadius;
    [SerializeField] float flickerAmount = 1f;
    [SerializeField] float flickerSpeed = 2f;

    void Start()
    {
        fireLight = GetComponent<Light2D>();
        baseInnerRadius = fireLight.pointLightInnerRadius;
        baseOuterRadius = fireLight.pointLightOuterRadius;
    }

    void Update()
    {
        Flicker();
    }

    void Flicker()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        fireLight.pointLightInnerRadius = baseInnerRadius + noise * flickerAmount;
        fireLight.pointLightOuterRadius = baseOuterRadius + noise * flickerAmount;
    }
}
