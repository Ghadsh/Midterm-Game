// TeaRecipe.cs  (extend your existing file)
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Tea Recipe")]
public class TeaRecipe : ScriptableObject
{
    [Header("Inputs")]
    public ItemDefinition baseItem;

    [System.Serializable]
    public class ModReq { public ItemDefinition item; [Min(1)] public int count = 1; }
    public List<ModReq> mods = new();

    [Header("Output")]
    public ItemDefinition resultItem;
    [Min(1)] public int resultCount = 1;

    [Header("Brew Constraints")]
    public float targetTempMinC = 70f;
    public float targetTempMaxC = 95f;

    public float pourMlMin = 180f;
    public float pourMlMax = 260f;

    [Range(0f, 1f)] public float stirRequired01 = 0.75f; // normalized stir progress

    [Header("Options")]
    public bool returnInputsOnFail = false;
    public float brewSeconds = 0f; // currently unused (we're doing immediate once constraints met)
}
