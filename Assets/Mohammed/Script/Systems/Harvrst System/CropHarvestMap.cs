// CropHarvestMap.cs
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Crop Harvest Map")]
public class CropHarvestMap : ScriptableObject
{
    public Entry[] entries;
    [System.Serializable]
    public struct Entry
    {
        public CropDefinition crop;
        public ItemDefinition harvestedItem;
    }

    public bool TryGetItem(CropDefinition crop, out ItemDefinition item)
    {
        foreach (var e in entries) { if (e.crop == crop) { item = e.harvestedItem; return true; } }
        item = null; return false;
    }
}
