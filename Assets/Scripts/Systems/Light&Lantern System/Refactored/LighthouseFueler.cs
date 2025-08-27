using UnityEngine;

/// <summary>
/// Consumes lumens from a linked LightJar to keep a lighthouse light running.
/// If the jar is empty, the lighthouse dims (or can fully go dark).
/// </summary>
public class LighthouseFueler : MonoBehaviour
{
    [Header("Config (SO)")]
    [SerializeField] private FuelConfig config;

    [Header("Sources & Outputs")]
    [SerializeField] private LightJar fuelJar;  // where we pull lumens from
    [SerializeField] private Light lighthouseLight; // the big light on top of the lighthouse

    private void Awake()
    {
        if (!lighthouseLight) lighthouseLight = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        if (config == null || lighthouseLight == null) return;

        float needed = config.consumePerSecond * Time.deltaTime;

        // Try to consume from the jar
        float got = 0f;
        if (fuelJar != null) got = fuelJar.TakeLumens(needed);

        if (got >= needed * 0.99f)
        {
            // Fully fueled: use target settings
            lighthouseLight.intensity = config.targetIntensity;
            lighthouseLight.range = config.targetRange;
        }
        else
        {
            // Not enough fuel: dim or clamp to min
            if (config.dimWhenLow)
            {
                float ratio = (needed <= 0f) ? 0f : Mathf.Clamp01(got / needed);
                lighthouseLight.intensity = Mathf.Lerp(config.minIntensity, config.targetIntensity, ratio);
                lighthouseLight.range = Mathf.Lerp(config.minRange, config.targetRange, ratio);
            }
            else
            {
                lighthouseLight.intensity = config.minIntensity;
                lighthouseLight.range = config.minRange;
            }
        }
    }
}
