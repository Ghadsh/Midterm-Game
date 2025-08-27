// UpgradeManager.cs
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private EconomyService economy;
    [SerializeField] private InventoryService inventory;
    [SerializeField] private List<UpgradeDefinition> available = new();

    private HashSet<string> purchased = new();

    public System.Action<UpgradeDefinition> OnPurchased;

    void Awake()
    {
        if (!economy) economy = FindObjectOfType<EconomyService>();
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
    }

    public IReadOnlyList<UpgradeDefinition> Available => available;

    public bool IsPurchased(UpgradeDefinition u) => purchased.Contains(u.id);

    public bool TryPurchase(UpgradeDefinition u)
    {
        if (u == null || IsPurchased(u)) return false;
        if (!economy.TrySpend(u.cost)) return false;

        Apply(u);
        purchased.Add(u.id);
        OnPurchased?.Invoke(u);
        return true;
    }

    void Apply(UpgradeDefinition u)
    {
        foreach (var e in u.effects)
        {
            switch (e.type)
            {
                case UpgradeDefinition.EffectType.IncreaseInventoryCapacity:
                    inventory.SetCapacity(inventory.Capacity + Mathf.Max(0, e.intValue));
                    break;

                case UpgradeDefinition.EffectType.SpawnPrefab:
                    if (e.prefab)
                    {
                        var parent = e.spawnParent ? e.spawnParent : null;
                        Instantiate(e.prefab, parent);
                    }
                    break;

                case UpgradeDefinition.EffectType.UnlockProcessor:
                    if (e.processorToEnable) e.processorToEnable.SetActive(true);
                    break;
            }
        }
    }
}
