using UnityEngine;

[DisallowMultipleComponent]
public class Draggable3D : MonoBehaviour
{
    [Header("Drag Camera (assign your FP station cam here)")]
    [SerializeField] private Camera dragCamera;      // <- assign in Inspector if possible
    [Header("Tuning")]
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private float zStepPerScroll = 0.25f;
    [SerializeField] private float minDepth = 0.2f;

    Rigidbody rb;
    bool dragging;
    float depth;
    bool warnedNoCam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        TryFindCamera();
    }

    // Call this from StationFocus when entering focus
    public void SetCamera(Camera cam) { dragCamera = cam; warnedNoCam = false; }

    void OnMouseDown()
    {
        dragging = true;
        if (!dragCamera) TryFindCamera();
        if (dragCamera)
        {
            // distance along camera forward to this object
            Vector3 toObj = transform.position - dragCamera.transform.position;
            depth = Vector3.Dot(toObj, dragCamera.transform.forward);
            if (depth < minDepth) depth = Mathf.Max(minDepth, depth);
        }
    }

    void OnMouseUp() { dragging = false; }

    void Update()
    {
        if (!dragging) return;

        if (!dragCamera)
        {
            TryFindCamera();
            if (!dragCamera)
            {
                if (!warnedNoCam) { Debug.LogWarning("[Draggable3D] No camera available for dragging."); warnedNoCam = true; }
                return;
            }
        }

        // scroll to change depth
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f) depth = Mathf.Max(minDepth, depth - scroll * zStepPerScroll);

        // target world position from mouse + depth (distance from camera)
        var mouse = Input.mousePosition;
        mouse.z = depth;
        Vector3 target = dragCamera.ScreenToWorldPoint(mouse);

        if (rb)
            rb.linearVelocity = (target - transform.position) * followSpeed;
        else
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
    }

    void TryFindCamera()
    {
        if (dragCamera && dragCamera.isActiveAndEnabled) return;

        // Prefer MainCamera if present
        var main = Camera.main;
        if (main && main.isActiveAndEnabled) { dragCamera = main; return; }

        // Otherwise grab any active camera
        var cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            if (cams[i] && cams[i].isActiveAndEnabled) { dragCamera = cams[i]; return; }
        }

        dragCamera = null;
    }
}
