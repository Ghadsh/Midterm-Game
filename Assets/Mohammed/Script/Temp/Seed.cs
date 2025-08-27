// SeedCatalog.cs
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Seed Catalog")]
public class SeedCatalog : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public ItemDefinition seedItem;
        public CropDefinition crop;
    }
    public Entry[] entries;

    public bool TryGetCrop(ItemDefinition seed, out CropDefinition crop)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].seedItem == seed) { crop = entries[i].crop; return true; }
        }
        crop = null; return false;
    }
}
