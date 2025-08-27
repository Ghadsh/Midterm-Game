// UpgradeUI.cs (key changes)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager manager;
    [SerializeField] private Transform listParent;
    [SerializeField] private Button rowPrefab;
    [SerializeField] private TextMeshProUGUI coinsLabel;
    [SerializeField] private EconomyService economy;

    readonly Dictionary<UpgradeDefinition, Button> rows = new();

    void OnEnable()
    {
        if (!manager) manager = FindObjectOfType<UpgradeManager>();
        if (!economy) economy = FindObjectOfType<EconomyService>();

        manager.OnPurchased += OnPurchased;
        Rebuild();
        Refresh();
    }
    void OnDisable()
    {
        if (manager) manager.OnPurchased -= OnPurchased;
    }

    void Rebuild()
    {
        rows.Clear();
        foreach (Transform c in listParent) Destroy(c.gameObject);

        foreach (var u in manager.Available)
        {
            var btn = Instantiate(rowPrefab, listParent);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.text = $"{u.displayName}  (${u.cost})";

            btn.onClick.AddListener(() => {
                if (manager.TryPurchase(u)) Refresh();
            });

            rows[u] = btn;
            ApplyPurchasedState(u); // set initial state
        }
    }

    void OnPurchased(UpgradeDefinition u)
    {
        ApplyPurchasedState(u);
        Refresh();
    }

    void ApplyPurchasedState(UpgradeDefinition u)
    {
        if (!rows.TryGetValue(u, out var btn)) return;
        bool owned = manager.IsPurchased(u);
        btn.interactable = !owned;
        var label = btn.GetComponentInChildren<TextMeshProUGUI>();
        label.text = owned ? $"{u.displayName}  (Purchased)" : $"{u.displayName}  (${u.cost})";
        // Or: btn.gameObject.SetActive(!owned);  // Hide instead of disable
    }

    void Refresh()
    {
        if (coinsLabel && economy) coinsLabel.text = $"Coins: {economy.Coins}";
        // Also refresh all rows (affordability tint optional)
        foreach (var kv in rows) ApplyPurchasedState(kv.Key);
    }
}
