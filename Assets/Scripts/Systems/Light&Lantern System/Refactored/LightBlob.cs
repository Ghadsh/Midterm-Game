using UnityEngine;

public class LightBlob : MonoBehaviour
{
    [Header("Config (SO)")]
    [SerializeField] private LightTuning tuning;
    // ScriptableObject holding all tuning values for this light blob
    // This includes max lumens, light curves, emission color, split rules, etc.

    [Header("Runtime State")]
    [SerializeField] private float lumens = 60f;
    // Current lumens (light energy) stored in this blob.
    // This value changes during gameplay as light is added, taken, or drained.

    [Header("References")]
    [SerializeField] private Light pointLight;
    // Unity Light component for visual illumination in the scene.
    [SerializeField] private Renderer rend;
    // Renderer with an emissive material for glow effect.

    // Public read-only accessors
    public float Lumens => lumens;
    // Read-only property: lets other scripts check current lumens
    public float MaxLumens => tuning ? tuning.maxLumens : 100f;
    // Read-only property: gets the max lumens from tuning, defaults to 100 if no tuning asset is assigned

    // Cache shader emission property ID for performance
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        // Auto-assign references if not set manually
        if (!pointLight) pointLight = GetComponentInChildren<Light>();
        if (!rend) rend = GetComponentInChildren<Renderer>();

        // Ensure starting lumens do not exceed maximum
        if (tuning) lumens = Mathf.Clamp(lumens, 0f, tuning.maxLumens);

        // Update visuals based on current lumens at start
        UpdateVisuals();
    }

    private void Update()
    {
        // Apply passive drain if configured in tuning
        if (tuning && tuning.passiveDrainPerSecond > 0f && lumens > 0f)
        {
            lumens = Mathf.Max(0f, lumens - tuning.passiveDrainPerSecond * Time.deltaTime);
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Removes a certain amount of lumens from this blob and returns the amount actually removed.
    /// This is used when pouring into jars or fueling something.
    /// </summary>
    public float TakeLumens(float amount)
    {
        float taken = Mathf.Min(amount, lumens);
        lumens -= taken;
        UpdateVisuals();
        return taken;
    }

    /// <summary>
    /// Adds lumens to this blob, clamped to maximum capacity from tuning.
    /// This is used when light is transferred into the blob.
    /// </summary>
    public void AddLumens(float amount)
    {
        float max = MaxLumens;
        lumens = Mathf.Clamp(lumens + amount, 0f, max);
        UpdateVisuals();
    }

    /// <summary>
    /// Splits this blob into a new smaller blob based on a ratio (from tuning limits).
    /// Creates a clone of this GameObject with part of the lumens transferred.
    /// </summary>
    public LightBlob Split(float ratio)
    {
        if (!tuning) return null;

        // Clamp ratio to limits from tuning SO
        ratio = Mathf.Clamp(ratio, tuning.splitRatioLimits.x, tuning.splitRatioLimits.y);

        float part = lumens * ratio;
        if (part <= 0.01f) return null; // Don't bother splitting if tiny

        // Create a new blob prefab clone near the current blob
        var clone = Instantiate(gameObject, transform.position + Random.insideUnitSphere * 0.3f, Random.rotation);

        // Set the lumens of the new blob and reduce this blob's lumens
        var blob = clone.GetComponent<LightBlob>();
        blob.SetLumens(Mathf.Min(part, MaxLumens));
        TakeLumens(part);

        return blob;
    }

    /// <summary>
    /// Directly sets the lumens of this blob (used when spawning or splitting).
    /// </summary>
    public void SetLumens(float value)
    {
        float max = MaxLumens;
        lumens = Mathf.Clamp(value, 0f, max);
        UpdateVisuals();
    }

    /// <summary>
    /// Updates the Unity Light component and emissive material based on current lumens.
    /// Uses curves from the tuning SO to map fill percentage to visuals.
    /// </summary>
    private void UpdateVisuals()
    {
        float max = MaxLumens;
        float fill = (max <= 0f) ? 0f : lumens / max; // 0 = empty, 1 = full

        // Update Unity Light intensity and range using tuning curves
        if (pointLight && tuning)
        {
            pointLight.intensity = tuning.intensityByFill.Evaluate(fill);
            pointLight.range = tuning.rangeByFill.Evaluate(fill);
        }

        // Update emission color based on tuning settings
        if (rend && rend.material && tuning)
        {
            if (rend.material.HasProperty(EmissionID))
            {
                float em = Mathf.LinearToGammaSpace(tuning.emissionByFill.Evaluate(fill));
                Color c = tuning.emissionColor * em;
                rend.material.SetColor(EmissionID, c);
                DynamicGI.SetEmissive(rend, c); // Updates GI for dynamic lighting
            }
        }

        // Destroy blob if lumens are depleted
        if (lumens <= 0.001f) Destroy(gameObject);
    }
}
