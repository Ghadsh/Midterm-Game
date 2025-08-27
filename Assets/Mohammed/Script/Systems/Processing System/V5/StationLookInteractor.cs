// StationLookInteractor.cs
using UnityEngine;

public class StationLookInteractor : MonoBehaviour
{
    [Header("Detection")]
    public Camera cam;                     // auto-finds Camera.main if null
    public float maxDistance = 3.5f;       // how far you can 'look interact' a station
    public float sphereRadius = 0.28f;     // wider than a thin ray = easier hits
    public LayerMask stationMask;          // set to layer that your stations/trigger colliders are on

    [Header("Keys")]
    public KeyCode enterKey = KeyCode.F;
    public KeyCode exitKey1 = KeyCode.E;
    public KeyCode exitKey2 = KeyCode.Escape;

    StationFocus _current;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (stationMask.value == 0) stationMask = Physics.DefaultRaycastLayers; // safe default
    }

    void Update()
    {
        // 1) Find a station you're looking at (spherecast from camera center)
        StationFocus target = FindLookStation();

        // 2) Manage 'in range' state so prompts/UI know what's targetable
        if (target != _current)
        {
            if (_current) _current.SetInRange(false);
            if (target) target.SetInRange(true);
            _current = target;
        }

        // 3) Enter/Exit
        if (_current && Input.GetKeyDown(enterKey))
            _current.EnterFocus();

        if (_current && (Input.GetKeyDown(exitKey1) || Input.GetKeyDown(exitKey2)))
            _current.ExitFocus();
    }

    StationFocus FindLookStation()
    {
        if (!cam) return null;

        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // crosshair center
        if (Physics.SphereCast(ray, sphereRadius, out var hit, maxDistance, stationMask, QueryTriggerInteraction.Collide))
        {
            // accept stations hit directly or via any child/trigger
            return hit.collider.GetComponentInParent<StationFocus>();
        }

        return null;
    }
}
