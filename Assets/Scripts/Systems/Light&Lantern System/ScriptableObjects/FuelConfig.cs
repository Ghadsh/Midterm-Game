using UnityEngine;

[CreateAssetMenu(menuName = "LanternLight/Fuel Config")]
public class FuelConfig : ScriptableObject
{
    [Header("Consumption")]
    public float consumePerSecond = 20f; // how fast lighthouse consumes lumens
    public float targetIntensity = 5f;
    public float targetRange = 25f;

    [Header("Fail States")]
    public bool dimWhenLow = true;
    public float minIntensity = 0.2f;
    public float minRange = 5f;
}
