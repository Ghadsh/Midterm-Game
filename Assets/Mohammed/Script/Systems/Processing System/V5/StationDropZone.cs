// StationDropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class StationDropZone : MonoBehaviour, IDropHandler
{
    public StationUIController stationUI;
    public bool dropToBase = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (stationUI == null || eventData.pointerDrag == null) return;

        var payload = eventData.pointerDrag.GetComponent<InventoryDragPayload>();
        if (payload == null || payload.itemDef == null || payload.count <= 0) return;

        stationUI.OnDropFromPlayer(payload.itemDef, payload.count, dropToBase);
    }
}
