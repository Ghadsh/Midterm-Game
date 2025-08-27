using System.Collections.Generic;
using UnityEngine;

public class CupContainer : MonoBehaviour
{
    [Header("Contents")]
    public float leavesUnits = 0f;     // simple unit; 1 = one base stack
    public float waterMl = 0f;
    public float tempC = 20f;          // mixed
    public Dictionary<ItemDefinition, int> mods = new();

    [Header("Stirring")]
    [Range(0f, 1f)] public float stirProgress01 = 0f;
    public float stirDecayPerSec = 0f; // set >0 if you want it to decay slowly

    void Update()
    {
        if (stirDecayPerSec > 0f)
            stirProgress01 = Mathf.Max(0f, stirProgress01 - stirDecayPerSec * Time.deltaTime);
    }

    public void AddLeaves(float units)
    {
        if (units <= 0f) return;
        leavesUnits += units;
    }

    public void AddWater(float ml, float incomingTempC)
    {
        if (ml <= 0f) return;

        float total = waterMl + ml;
        if (total <= 0f) { waterMl = 0f; tempC = 20f; return; }

        // mix temps proportional to ml
        tempC = (tempC * waterMl + incomingTempC * ml) / total;
        waterMl = total;
    }

    public void AddMod(ItemDefinition def, int count)
    {
        if (!def || count <= 0) return;
        mods.TryGetValue(def, out int c);
        mods[def] = c + count;
    }

    public void AddStir(float normDelta)
    {
        if (normDelta <= 0f) return;
        stirProgress01 = Mathf.Clamp01(stirProgress01 + normDelta);
    }

    public void ResetAll()
    {
        leavesUnits = 0f;
        waterMl = 0f;
        tempC = 20f;
        mods.Clear();
        stirProgress01 = 0f;
    }
}
