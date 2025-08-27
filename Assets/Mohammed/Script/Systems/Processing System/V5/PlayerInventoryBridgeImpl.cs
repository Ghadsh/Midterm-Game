// PlayerInventoryBridgeImpl.cs
using UnityEngine;

[AddComponentMenu("TeaGame/Player Inventory Bridge")]
public class PlayerInventoryBridgeImpl : MonoBehaviour, IPlayerInventoryBridge
{
    [Tooltip("Your InventoryService instance. If left empty, it will auto-find one in the scene.")]
    public InventoryService inventory;

    [Header("Debug")]
    public bool logMoves = false;

    void Awake()
    {
        EnsureInventory();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) EnsureInventory();
    }
#endif

    void EnsureInventory()
    {
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
        if (!inventory)
            Debug.LogWarning("[PlayerInventoryBridgeImpl] No InventoryService found in scene.");
    }

    // ---- IPlayerInventoryBridge ----

    // Move items FROM player inventory TO the station (non-destructive until Begin).
    public bool TakeFromPlayer(ItemDefinition def, int count)
    {
        if (!def || count <= 0) return false;
        EnsureInventory();
        if (!inventory) return false;

        bool ok = inventory.Remove(def, count);
        if (logMoves) Debug.Log($"[Bridge] TakeFromPlayer {def.displayName} x{count} -> {(ok ? "OK" : "FAIL")}");
        return ok;
    }

    // Return items TO player inventory (when retrieving or canceling).
    public void GiveToPlayer(ItemDefinition def, int count)
    {
        if (!def || count <= 0) return;
        EnsureInventory();
        if (!inventory) return;

        inventory.Add(def, count);
        if (logMoves) Debug.Log($"[Bridge] GiveToPlayer {def.displayName} x{count}");
    }

    // ---- Optional helpers (not in the interface) ----

    // Quick check: do we have at least this many of an item?
    public int CountInPlayer(ItemDefinition def)
    {
        if (!def) return 0;
        EnsureInventory();
        if (!inventory) return 0;

        int total = 0;
        for (int i = 0; i < inventory.Capacity; i++)
        {
            var s = inventory.GetAt(i);
            if (s != null && s.item == def) total += s.count;
        }
        return total;
    }
}
