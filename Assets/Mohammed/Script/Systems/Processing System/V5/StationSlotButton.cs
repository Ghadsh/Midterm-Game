// StationSlotButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StationSlotButton : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    public StationUIController stationUI;
    public bool isBase = false;
    public int modIndex = 0;

    [Header("Visuals")]
    public Image icon;              // child "Icon" (display only)
    public TMPro.TMP_Text countText;// child "Count"
    public Image highlight;         // optional overlay image
    public Color okColor = new Color(0f, 1f, 0f, 0.30f);
    public Color blockedColor = new Color(1f, 0f, 0f, 0.30f);

    void Reset()
    {
        stationUI = GetComponentInParent<StationUIController>();
        icon = transform.Find("Icon")?.GetComponent<Image>();
        countText = transform.Find("Count")?.GetComponent<TMPro.TMP_Text>();

        var bg = GetComponent<Image>(); if (bg) bg.raycastTarget = true;
        if (icon) icon.raycastTarget = false;

        if (!highlight)
        {
            // auto-create a simple overlay if missing
            var go = new GameObject("Highlight", typeof(Image));
            go.transform.SetParent(transform, false);
            highlight = go.GetComponent<Image>();
            var rt = highlight.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            highlight.raycastTarget = false;
            highlight.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {
        var payload = DragBus.CurrentPayload;
        if (!highlight) return;

        if (payload != null && stationUI != null)
        {
            bool can = stationUI.CanAcceptDrop(isBase, modIndex, payload.itemDef, payload.count);
            highlight.color = can ? okColor : blockedColor;
            highlight.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (highlight) highlight.gameObject.SetActive(false);
    }

    // drop from inventory
    public void OnDrop(PointerEventData e)
    {
        if (highlight) highlight.gameObject.SetActive(false);
        if (stationUI == null) return;

        var payload = e.pointerDrag ? e.pointerDrag.GetComponent<InventoryDragPayload>() : null;
        if (payload == null) payload = DragBus.CurrentPayload;
        if (payload == null || payload.itemDef == null || payload.count <= 0) return;

        stationUI.OnDropToSlot(isBase, modIndex, payload.itemDef, payload.count);
    }

    // click to return items from this slot
    public void OnPointerClick(PointerEventData e)
    {
        if (!stationUI) return;
        if (isBase) stationUI.OnReturnBase();
        else stationUI.OnReturnMod(modIndex);
    }
}
