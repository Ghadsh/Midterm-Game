// UpgradeDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Upgrade")]
public class UpgradeDefinition : ScriptableObject
{
    public string id;                    // unique key
    public string displayName;
    [TextArea] public string description;
    public int cost = 50;

    public enum EffectType { IncreaseInventoryCapacity, SpawnPrefab, UnlockProcessor }
    [System.Serializable]
    public struct Effect
    {
        public EffectType type;
        public int intValue;                   // e.g. +5 slots
        public GameObject prefab;              // for SpawnPrefab
        public Transform spawnParent;          // optional
        public GameObject processorToEnable;   // for UnlockProcessor
    }
    public Effect[] effects;
}
