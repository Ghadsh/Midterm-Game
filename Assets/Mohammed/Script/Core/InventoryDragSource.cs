// InventoryDragSource.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryService inventory;        // assign (or auto-find)
    public SlotUI slotUI;                     // optional; auto-read Index if present
    public int slotIndexOverride = -1;        // use if you don't have SlotUI

    [Header("Drag visuals")]
    public Canvas canvas;                     // your Canvas_UI (auto-find if null)
    public Image dragIconTemplate;            // optional prefab (Image) for the ghost icon
    public Vector2 iconPivot = new Vector2(0.1f, 0.9f);

    [Header("Amount")]
    public bool dragFullStack = false;        // false = 1 item, true = full stack
    public int customAmount = 1;              // used when dragFullStack == false

    InventoryDragPayload payloadOnSlot;       // lives on THIS slot object
    RectTransform iconRT;                     // runtime ghost icon
    CanvasGroup iconCG;
    public int GetSlotIndex() => (slotUI != null ? slotUI.Index : slotIndexOverride);

    int SlotIndex => slotUI != null ? slotUI.Index : slotIndexOverride;

    void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<InventoryService>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!slotUI) slotUI = GetComponent<SlotUI>();

        // ensure payload component exists on this slot (so drop zones can read pointerDrag)
        payloadOnSlot = GetComponent<InventoryDragPayload>();
        if (!payloadOnSlot) payloadOnSlot = gameObject.AddComponent<InventoryDragPayload>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        var idx = SlotIndex;
        if (idx < 0 || inventory == null) return;

        var stack = inventory.GetAt(idx);
        if (stack == null || stack.item == null || stack.count <= 0) return;

        payloadOnSlot.itemDef = stack.item;
        payloadOnSlot.count = dragFullStack ? stack.count : Mathf.Clamp(customAmount, 1, stack.count);

        CreateIcon(stack.item.icon);
        UpdateIconPosition(e);
    }

    public void OnDrag(PointerEventData e) => UpdateIconPosition(e);

    public void OnEndDrag(PointerEventData e) => DestroyIcon();

    // ---- visuals ----
    void CreateIcon(Sprite sprite)
    {
        if (!canvas) canvas = FindObjectOfType<Canvas>();
        if (!canvas) return;

        if (dragIconTemplate)
        {
            iconRT = Instantiate(dragIconTemplate, canvas.transform).rectTransform;
            iconCG = iconRT.GetComponent<CanvasGroup>();
        }
        else
        {
            var go = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            go.transform.SetParent(canvas.transform, false);
            iconRT = go.GetComponent<RectTransform>();
            iconCG = go.GetComponent<CanvasGroup>();
            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.sprite = sprite;
            img.preserveAspect = true;
            iconRT.sizeDelta = new Vector2(64, 64);
        }
        if (iconCG) iconCG.blocksRaycasts = false; // allow drop zones to receive pointer
        iconRT.pivot = iconPivot;
    }

    void UpdateIconPosition(PointerEventData e)
    {
        if (!iconRT) return;

        Vector2 localPos;
        RectTransform cRT = canvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cRT, e.position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out localPos))
        {
            iconRT.anchoredPosition = localPos;
        }
    }

    void DestroyIcon()
    {
        if (iconRT) Destroy(iconRT.gameObject);
        iconRT = null; iconCG = null;
    }
}
