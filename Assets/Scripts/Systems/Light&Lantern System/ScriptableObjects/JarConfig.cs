using UnityEngine;

[CreateAssetMenu(menuName = "LanternLight/Jar Config")]
public class JarConfig : ScriptableObject
{
    [Header("Storage")]
    public float capacity = 200f;
    public float transferRate = 50f; // lumens/sec

    [Header("Visuals")]
    public bool usePointLight = true;
    public AnimationCurve lightIntensityByFill = AnimationCurve.Linear(0, 0f, 1, 2.2f);
    public AnimationCurve lightRangeByFill = AnimationCurve.Linear(0, 0f, 1, 8f);

    public bool useEmission = true;
    public Color emissionColor = Color.yellow;
    public AnimationCurve emissionByFill = AnimationCurve.Linear(0, 0.05f, 1, 2.5f);

    [Header("Shader Property")]
    public string emissionColorName = "_EmissionColor";
}
