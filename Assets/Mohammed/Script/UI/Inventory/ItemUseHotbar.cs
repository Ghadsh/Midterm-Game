using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUseHotbar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera cam;                 // used only for fallback
    [SerializeField] private HotbarController hotbar;
    [SerializeField] private SeedCatalog seedCatalog;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Aim")]
    [SerializeField] private AimProvider aim;            // shared crosshair aim (preferred)
    [SerializeField] private LayerMask groundMask;       // your "Ground" layer
    [SerializeField] private float maxDistance = 8f;     // tweak for 3rd person
    [SerializeField] private float fallbackAimRadius = 0.15f; // used only if AimProvider is missing

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!hotbar) hotbar = FindObjectOfType<HotbarController>();
        if (!aim) aim = FindObjectOfType<AimProvider>();
    }

    void Update()
    {
        // Pause hotbar when gameplay is blocked or UI is focused
        if (UIState.BlockGameplay) return;
        if (inventoryPanel && inventoryPanel.activeSelf) return;
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        // Fire only on LMB down.
        if (!Input.GetMouseButtonDown(0)) return;

        // Must have a valid, usable seed selected.
        var stack = hotbar?.GetSelectedStack();
        if (stack == null || stack.item == null || stack.count <= 0) return;
        if (seedCatalog == null || !seedCatalog.TryGetCrop(stack.item, out _)) return;

        // Raycast using shared AimProvider (crosshair + spherecast), fallback to screen-center spherecast.
        RaycastHit hit;
        bool gotHit = (aim != null)
            ? aim.SphereCast(maxDistance, groundMask, out hit)
            : Physics.SphereCast((cam ? cam : Camera.main).ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)),
                                 fallbackAimRadius, out hit, maxDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (!gotHit) return;

        // Request planting at the hit point (actual planting system will consume/decrement on success).
        GameEvents.OnSeedUseRequested?.Invoke(stack.item, hit.point);
    }
}
