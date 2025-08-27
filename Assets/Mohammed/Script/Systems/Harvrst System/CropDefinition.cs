// CropDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Crop Definition")]
public class CropDefinition : ScriptableObject
{
    public string cropId;               // "tea_leaf", "coffee_cherry"
    public string displayName;
    public Sprite icon;
    public int matureStageIndex = 3;    // stage considered harvestable
    public int yieldMin = 1;
    public int yieldMax = 3;
}
