using UnityEngine;

[CreateAssetMenu(menuName = "LanternLight/Light Tuning")]
public class LightTuning : ScriptableObject
{
    [Header("Capacity & Drain")]
    public float maxLumens = 100f;
    public float passiveDrainPerSecond = 0f;

    [Header("Light Mapping (0..1 fill)")]
    public AnimationCurve intensityByFill = AnimationCurve.Linear(0, 0.2f, 1, 2.5f);
    public AnimationCurve rangeByFill = AnimationCurve.Linear(0, 2f, 1, 10f);

    [Header("Emission (HDR)")]
    public Color emissionColor = Color.white;
    public AnimationCurve emissionByFill = AnimationCurve.Linear(0, 0.1f, 1, 3f);

    [Header("Split Rules")]
    public Vector2 splitRatioLimits = new Vector2(0.2f, 0.8f); // min/max allowed split ratio
}
