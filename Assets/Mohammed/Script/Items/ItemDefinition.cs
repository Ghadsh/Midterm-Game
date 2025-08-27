// ItemDefinition.cs (extend your existing file)
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "TeaGame/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Core")]
    public string itemId;               // "tea_leaf_item"
    public string displayName;
    public Sprite icon;
    public bool stackable = true;
    public int maxStack = 99;

    [Header("V5 Additions")]
    public GameObject stationObjectPrefab;   // optional 3D prefab when spawned/previewed on station
    public ItemCategory category;            // Base, Mod, or Both
}

[Flags]
public enum ItemCategory
{
    None = 0,
    Base = 1,
    Mod = 2,
    Both = Base | Mod
}
