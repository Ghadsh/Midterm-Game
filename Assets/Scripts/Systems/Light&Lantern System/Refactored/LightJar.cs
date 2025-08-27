using UnityEngine;

public class LightJar : MonoBehaviour
{
    [Header("Config (SO)")]
    [SerializeField] private JarConfig config;

    [Header("Runtime State")]
    [SerializeField] private float lumens = 0f;

    [Header("Refs")]
    [SerializeField] private Light jarLight;
    [SerializeField] private Renderer rend;

    private int emissionID;

    public float Capacity => config ? config.capacity : 200f;
    public float Lumens => lumens;

    private void Awake()
    {
        if (!jarLight) jarLight = GetComponentInChildren<Light>();
        if (!rend) rend = GetComponentInChildren<Renderer>();
        emissionID = Shader.PropertyToID(config ? config.emissionColorName : "_EmissionColor");
        UpdateVisuals();
    }

    public float PourFrom(LightBlob blob, float deltaTime)
    {
        if (blob == null || config == null) return 0f;
        float capacity = config.capacity;
        float rate = config.transferRate;

        if (lumens >= capacity) return 0f;

        float need = capacity - lumens;
        float ask = rate * deltaTime;
        float request = Mathf.Min(need, ask);
        float got = blob.TakeLumens(request);
        lumens += got;

        UpdateVisuals();
        return got;
    }

    public float TakeLumens(float amount)
    {
        float taken = Mathf.Min(amount, lumens);
        lumens -= taken;
        UpdateVisuals();
        return taken;
    }

    public void AddLumens(float amount)
    {
        float cap = Capacity;
        lumens = Mathf.Clamp(lumens + amount, 0f, cap);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (config == null) return;

        float fill = Mathf.Clamp01(lumens / config.capacity);

        if (config.usePointLight && jarLight)
        {
            jarLight.intensity = config.lightIntensityByFill.Evaluate(fill);
            jarLight.range = config.lightRangeByFill.Evaluate(fill);
        }

        if (config.useEmission && rend && rend.material)
        {
            if (rend.material.HasProperty(emissionID))
            {
                float em = Mathf.LinearToGammaSpace(config.emissionByFill.Evaluate(fill));
                Color c = config.emissionColor * em;
                rend.material.SetColor(emissionID, c);
                DynamicGI.SetEmissive(rend, c);
            }
        }
    }
}
