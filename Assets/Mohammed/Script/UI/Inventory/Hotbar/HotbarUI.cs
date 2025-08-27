// HotbarUI.cs
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private HotbarController hotbar;
    [SerializeField] private Transform slotsParent;  // Horizontal Layout Group
    [SerializeField] private HotbarSlotUI slotPrefab;

    private HotbarSlotUI[] _slots;

    void Start()
    {
        if (!hotbar) hotbar = FindObjectOfType<HotbarController>();
        _slots = new HotbarSlotUI[hotbar.Size];
        for (int i = 0; i < hotbar.Size; i++)
        {
            var s = Instantiate(slotPrefab, slotsParent);
            s.Index = i;
            _slots[i] = s;
        }
        Refresh();

        // also refresh when inventory changes
        var inv = FindObjectOfType<InventoryService>();
        if (inv) inv.OnInventoryChanged += Refresh;
    }

    void OnDestroy()
    {
        var inv = FindObjectOfType<InventoryService>();
        if (inv) inv.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var stack = hotbar.GetStackAtHotbar(i);
            _slots[i].Set(stack);
            _slots[i].SetSelected(i == hotbar.SelectedIndex);
        }
    }

    // Public so input can force a redraw after selection changes
    public void ForceRefresh() => Refresh();
}
