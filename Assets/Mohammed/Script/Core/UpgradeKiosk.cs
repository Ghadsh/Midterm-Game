// UpgradeKiosk.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeKiosk : MonoBehaviour
{
    [Header("Open/Close")]
    public PanelToggle upgradesPanel;     // assign your upgrades UI panel
    public KeyCode interactKey = KeyCode.E;

    [Header("Detection")]
    public Camera cam;                    // player camera
    public RectTransform crosshair;       // assign your crosshair UI (optional, falls back to screen center)
    public float useRange = 2.5f;
    [Range(0.05f, 0.3f)] public float aimRadius = 0.15f; // forgiving sphere radius
    public LayerMask hitMask = ~0;        // default: everything

    [Header("Prompt (optional)")]
    public GameObject promptUI;           // "Press E to open Upgrades"

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (promptUI) promptUI.SetActive(false);
    }

    void Update()
    {
        // If any UI is open, do nothing (mouse belongs to UI)
        if (UIState.BlockGameplay) { if (promptUI) promptUI.SetActive(false); return; }

        // Build a ray from the crosshair (or screen center if not assigned)
        Vector2 sp = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        if (crosshair)
            sp = RectTransformUtility.WorldToScreenPoint(null, crosshair.position);
        Ray ray = cam.ScreenPointToRay(sp);

        // SphereCast is more forgiving than a thin ray in 3rd person
        if (!Physics.SphereCast(ray, aimRadius, out var hit, useRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (promptUI) promptUI.SetActive(false);
            return;
        }

        // Only show prompt when looking at THIS kiosk (or any collider under it)
        bool lookingAtMe = hit.collider.GetComponentInParent<UpgradeKiosk>() == this;
        if (promptUI) promptUI.SetActive(lookingAtMe);

        if (!lookingAtMe) return;

        // Ignore clicks when mouse is over UI
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

        // Interact
        if (Input.GetKeyDown(interactKey))
        {
            if (!upgradesPanel) return;
            // Open the upgrades UI (PanelToggle pushes UIState for you)
            upgradesPanel.Open();
        }
    }
}
