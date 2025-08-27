// TeaRecipeResolver.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TeaGame/Tea Recipe Resolver")]
public class TeaRecipeResolver : ScriptableObject
{
    public List<TeaRecipe> recipes = new();

    public TeaRecipe FindMatch(ItemDefinition baseItem, IReadOnlyList<StationItemStack> mods)
    {
        if (baseItem == null) return null;

        foreach (var r in recipes)
        {
            if (r == null || r.baseItem != baseItem) continue;
            if (ModsMatch(r.mods, mods)) return r;
        }
        return null;
    }

    static bool ModsMatch(List<TeaRecipe.ModReq> reqs, IReadOnlyList<StationItemStack> mods)
    {
        // Build "needed" map
        var need = new Dictionary<ItemDefinition, int>();
        foreach (var m in reqs)
        {
            if (m == null || m.item == null || m.count <= 0) continue;
            need[m.item] = need.TryGetValue(m.item, out var c) ? c + m.count : m.count;
        }

        // Build "have" map from the station mod slots
        var have = new Dictionary<ItemDefinition, int>();
        if (mods != null)
        {
            for (int i = 0; i < mods.Count; i++)
            {
                var s = mods[i];
                if (s == null || s.item == null || s.count <= 0) continue;
                have[s.item] = have.TryGetValue(s.item, out var c) ? c + s.count : s.count;
            }
        }

        // Exact multiset match
        if (need.Count != have.Count) return false;
        foreach (var kv in need)
            if (!have.TryGetValue(kv.Key, out var c) || c != kv.Value) return false;

        return true;
    }
}
