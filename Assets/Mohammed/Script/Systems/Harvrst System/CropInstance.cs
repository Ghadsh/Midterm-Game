// CropInstance.cs
using UnityEngine;

public class CropInstance : MonoBehaviour
{
    public CropDefinition definition;
    [Range(0, 5)] public int CurrentStageIndex = 0; // set by Growth System later

    public bool IsMature => definition != null && CurrentStageIndex >= definition.matureStageIndex;
}
