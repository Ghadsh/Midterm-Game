// InventorySlotDropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDropZone : MonoBehaviour, IDropHandler
{
    public InventoryService inventory;   // auto-find if null
    public SlotUI slotUI;                // auto-find if null

    void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
        if (!slotUI) slotUI = GetComponent<SlotUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventory == null || slotUI == null || eventData.pointerDrag == null) return;

        var src = eventData.pointerDrag.GetComponent<InventoryDragSource>();
        if (src == null) return;

        int from = src.GetSlotIndex();
        int to = slotUI.Index;

        if (from < 0 || to < 0 || from == to) return;

        // MoveOrMerge handles swap, merge, and events -> UI refreshes via OnInventoryChanged
        inventory.MoveOrMerge(from, to);
    }
}
