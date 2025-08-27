// HarvestInteractor.cs
using UnityEngine;

public class HarvestInteractor : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private LayerMask cropLayer;
    [SerializeField] private InventoryService inventory;
    [SerializeField] private CropHarvestMap harvestMap;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryHarvestUnderCursor();
    }

    void TryHarvestUnderCursor()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, maxDistance, cropLayer)) return;

        var crop = hit.collider.GetComponentInParent<CropInstance>();
        if (crop == null || !crop.IsMature) return;

        if (!harvestMap.TryGetItem(crop.definition, out var item)) return;

        int yield = Random.Range(crop.definition.yieldMin, crop.definition.yieldMax + 1);
        bool added = inventory.Add(item, yield);
        if (!added) { /* TODO: play inventory full SFX/UI */ return; }

        Destroy(crop.gameObject); // remove from soil
        // TODO: spawn vfx/sfx, floating text, etc.
    }
}
