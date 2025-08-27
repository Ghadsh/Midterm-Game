using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryService inventory;
    [SerializeField] private Transform slotsParent;   // Grid Layout Group object
    [SerializeField] private SlotUI slotPrefab;

    private SlotUI[] slots;
    private int selected = -1;

    void Start()
    {
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
        if (!inventory || !slotsParent || !slotPrefab)
        {
            Debug.LogError("[InventoryUI] Missing references.");
            enabled = false;
            return;
        }

        inventory.OnCapacityChanged += RebuildSlots;
        inventory.OnInventoryChanged += Refresh;

        RebuildSlots();
    }

    void OnDestroy()
    {
        if (!inventory) return;
        inventory.OnCapacityChanged -= RebuildSlots;
        inventory.OnInventoryChanged -= Refresh;
    }

    // -------- UI lifecycle --------
    void RebuildSlots()
    {
        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            Destroy(slotsParent.GetChild(i).gameObject);

        int cap = inventory.Capacity;
        slots = new SlotUI[cap];

        for (int i = 0; i < cap; i++)
        {
            var s = Instantiate(slotPrefab, slotsParent);
            s.Index = i;
            s.RootUI = this;
            slots[i] = s;
        }
        selected = -1;
        Refresh();
    }

    public void Refresh()
    {
        if (slots == null || inventory == null) return;

        int n = Mathf.Min(slots.Length, inventory.Capacity);
        for (int i = 0; i < n; i++)
        {
            var stack = inventory.GetAt(i);
            slots[i].Set(stack);
            slots[i].SetSelected(i == selected);
        }
    }

    // -------- Click handling from SlotUI --------
    public void OnSlotClicked(int index)
    {
        if (inventory == null) return;

        var stack = inventory.GetAt(index);
        bool hasItem = (stack != null && stack.item != null && stack.count > 0);

        // Selection & move/merge only (no station logic)
        if (selected >= 0)
        {
            if (index == selected)
            {
                selected = -1;
                Refresh();
                return;
            }

            inventory.MoveOrMerge(selected, index);
            selected = -1;
            Refresh();
            return;
        }

        if (hasItem)
        {
            selected = index;
            Refresh();
            return;
        }

        // empty click with no selection: no-op
    }
}
