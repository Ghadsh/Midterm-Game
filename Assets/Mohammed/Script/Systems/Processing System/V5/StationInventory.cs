// StationInventory.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StationItemStack
{
    public ItemDefinition item;
    public int count = 1;
}

public class StationInventory : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Unique ID per placed station in the scene.")]
    public string stationId = "station_01";

    [Header("Slots")]
    public StationItemStack baseSlot;               // only 1 allowed
    public List<StationItemStack> modSlots = new List<StationItemStack>();

    [Header("Limits")]
    public int maxMods = 3;

    [Header("Slot Stack Limits")]
    public int baseSlotMaxStack = 5;
    public int modSlotMaxStack = 5;

    [Header("Result Slot")]
    public StationItemStack resultSlot;
    public int resultMaxStack = 99;

    public float camYaw;
    public float camPitch;

    public event System.Action Changed;

    // --- Public API used by UI/bridge ---


    public int GetAddableToBase(ItemDefinition def, int count)
    {
        if (def == null || count <= 0) return 0;
        // category check: must be Base (or Both)
        if ((def.category & ItemCategory.Base) == 0) return 0;

        int cap = Mathf.Min(baseSlotMaxStack, def.stackable ? def.maxStack : 1);

        if (baseSlot == null || baseSlot.item == null)
            return Mathf.Clamp(count, 0, cap);

        // same item? top up
        if (baseSlot.item == def && def.stackable)
            return Mathf.Clamp(cap - baseSlot.count, 0, count);

        // different item already there
        return 0;
    }

    public int AddToBase(ItemDefinition def, int count)
    {
        int can = GetAddableToBase(def, count);
        if (can <= 0) return 0;

        if (baseSlot == null || baseSlot.item == null)
            baseSlot = new StationItemStack { item = def, count = can };
        else
            baseSlot.count += can;

        Changed?.Invoke();
        return can;
    }

    public void SubtractFromBase(int amount)
    {
        if (baseSlot == null || baseSlot.item == null || amount <= 0) return;
        baseSlot.count -= amount;
        if (baseSlot.count <= 0) baseSlot = null;
        Changed?.Invoke();
    }

    public int GetAddableToModAt(int index, ItemDefinition def, int count)
    {
        if (def == null || count <= 0) return 0;
        if ((def.category & ItemCategory.Mod) == 0) return 0;
        if (index < 0 || index >= maxMods) return 0;

        int cap = Mathf.Min(modSlotMaxStack, def.stackable ? def.maxStack : 1);

        // ensure list has capacity
        while (modSlots.Count <= index) modSlots.Add(null);

        var s = modSlots[index];
        if (s == null || s.item == null)
            return Mathf.Clamp(count, 0, cap);

        if (s.item == def && def.stackable)
            return Mathf.Clamp(cap - s.count, 0, count);

        return 0; // different item already present → blocked
    }

    public int AddToModAt(int index, ItemDefinition def, int count)
    {
        int can = GetAddableToModAt(index, def, count);
        if (can <= 0) return 0;

        while (modSlots.Count <= index) modSlots.Add(null);
        var s = modSlots[index];
        if (s == null || s.item == null)
            modSlots[index] = new StationItemStack { item = def, count = can };
        else
            s.count += can;

        Changed?.Invoke();
        return can;
    }

    public void SubtractFromModAt(int index, int amount)
    {
        if (index < 0 || index >= modSlots.Count) return;
        var s = modSlots[index];
        if (s == null || s.item == null || amount <= 0) return;

        s.count -= amount;
        if (s.count <= 0) modSlots[index] = null;
        Changed?.Invoke();
    }

    public bool TryAddBase(ItemDefinition def, int count)
    {
        if (def == null || (def.category & ItemCategory.Base) == 0) return false;
        if (baseSlot != null && baseSlot.item != null) return false;
        baseSlot = new StationItemStack { item = def, count = Mathf.Max(1, count) };
        Changed?.Invoke();
        return true;
    }

    public bool TryAddMod(ItemDefinition def, int count)
    {
        if (def == null || (def.category & ItemCategory.Mod) == 0) return false;
        if (modSlots.Count >= maxMods) return false;
        modSlots.Add(new StationItemStack { item = def, count = Mathf.Max(1, count) });
        Changed?.Invoke();
        return true;
    }

    public void ReturnAllToPlayer(IPlayerInventoryBridge bridge)
    {
        if (bridge == null) return;
        if (baseSlot != null && baseSlot.item != null)
        {
            bridge.GiveToPlayer(baseSlot.item, baseSlot.count);
            baseSlot = null;
        }
        foreach (var s in modSlots)
            if (s != null && s.item != null)
                bridge.GiveToPlayer(s.item, s.count);
        modSlots.Clear();
        Changed?.Invoke();
    }

    public void ConsumeAllForBegin(out ItemDefinition baseItem, out List<ItemDefinition> mods)
    {
        baseItem = baseSlot?.item;
        mods = new List<ItemDefinition>();
        foreach (var s in modSlots) if (s?.item) mods.Add(s.item);
        baseSlot = null;
        modSlots.Clear();
        Changed?.Invoke();
    }

    // --- Persistence container (other systems write into this) ---
    [Serializable]
    public class PersistData
    {
        public string stationId;
        public string baseItemId;
        public int baseCount;
        public List<string> modIds = new List<string>();
        public List<int> modCounts = new List<int>();

        public int heaterLevel;
        public Vector3 kettlePos;
        public Quaternion kettleRot;
        public string stage;          // Idle / Preview / Processing / Completed
        public float stageTime;       // progress within stage
        public float camYaw;    // persisted FP camera yaw

        public float camPitch;  // persisted FP camera pitch
    }

    public PersistData BuildPersist(Func<string, ItemDefinition> idToDef, Func<ItemDefinition, string> defToId,
                                    int heaterLevel, Vector3 kettlePos, Quaternion kettleRot,
                                    string stage, float stageTime)
    {
        var data = new PersistData { stationId = stationId, heaterLevel = heaterLevel, kettlePos = kettlePos, kettleRot = kettleRot, stage = stage, stageTime = stageTime };
        if (baseSlot?.item)
        {
            data.baseItemId = defToId(baseSlot.item);
            data.baseCount = baseSlot.count;
        }
        foreach (var m in modSlots)
        {
            data.modIds.Add(defToId(m.item));
            data.modCounts.Add(m.count);
        }
        return data;
    }

    public void RestorePersist(PersistData data, Func<string, ItemDefinition> idToDef)
    {
        baseSlot = null;
        modSlots.Clear();

        if (!string.IsNullOrEmpty(data.baseItemId))
            baseSlot = new StationItemStack { item = idToDef(data.baseItemId), count = data.baseCount };

        for (int i = 0; i < data.modIds.Count; i++)
        {
            var def = idToDef(data.modIds[i]);
            var ct = (i < data.modCounts.Count) ? data.modCounts[i] : 1;
            modSlots.Add(new StationItemStack { item = def, count = ct });
        }
        Changed?.Invoke();
    }

    // Add to StationInventory.cs (inside the class)
    public void ClearBase()
    {
        baseSlot = null;
        Changed?.Invoke();
    }

    public bool RemoveLastMod()
    {
        if (modSlots.Count > 0)
        {
            modSlots.RemoveAt(modSlots.Count - 1);
            Changed?.Invoke();
            return true;
        }
        return false;
    }

    // Optional, general-purpose notifier if you just need to signal changes:
    public void NotifyChanged()
    {
        Changed?.Invoke();
    }
    public bool RemoveModAt(int index)
    {
        if (modSlots == null) return false;
        if (index < 0 || index >= modSlots.Count) return false;

        modSlots.RemoveAt(index);
        Changed?.Invoke();   // notify UI to refresh
        return true;
    }

    public int AddToResult(ItemDefinition def, int count)
    {
        if (def == null || count <= 0) return 0;
        int cap = Mathf.Min(resultMaxStack, def.stackable ? def.maxStack : 1);

        if (resultSlot == null || resultSlot.item == null)
        {
            int add = Mathf.Clamp(count, 0, cap);
            if (add <= 0) return 0;
            resultSlot = new StationItemStack { item = def, count = add };
            Changed?.Invoke();
            return add;
        }

        if (resultSlot.item != def || !def.stackable) return 0;

        int space = cap - resultSlot.count;
        int add2 = Mathf.Clamp(count, 0, space);
        if (add2 <= 0) return 0;

        resultSlot.count += add2;
        Changed?.Invoke();
        return add2;
    }

    public StationItemStack TakeResultAll()
    {
        var s = resultSlot;
        resultSlot = null;
        Changed?.Invoke();
        return s;
    }
}
