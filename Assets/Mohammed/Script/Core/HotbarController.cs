// HotbarController.cs
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    [SerializeField] private InventoryService inventory;
    [SerializeField, Range(1, 9)] private int size = 8;   // number of hotbar slots
    public int Size => size;

    public int SelectedIndex { get; private set; } = 0;  // 0..size-1

    void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
        SelectedIndex = 0;
    }

    public void SelectIndex(int idx)
    {
        if (idx < 0 || idx >= size) return;
        SelectedIndex = idx;
        // UI will poll SelectedIndex in Refresh()
    }

    public void SelectNext(int dir)
    {
        // dir = +1 (scroll up) or -1 (scroll down)
        int n = size;
        SelectedIndex = (SelectedIndex + dir + n) % n;
    }

    // Map hotbar i => inventory i (first N)
    public InventoryService.Stack GetStackAtHotbar(int i)
    {
        if (!inventory) return null;
        return inventory.GetAt(i);
    }

    public InventoryService.Stack GetSelectedStack() => GetStackAtHotbar(SelectedIndex);
}
