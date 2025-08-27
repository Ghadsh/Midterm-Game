// Drag3DHandle.cs
using UnityEngine;

public class Drag3DHandle : MonoBehaviour
{
    public float depth = 1.0f;         // meters from camera
    public float zStepPerScroll = 0.25f;
    public float followSpeed = 20f;

    Camera cam;
    Rigidbody rb;
    bool dragging;

    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        dragging = true;
        if (!cam) cam = Camera.main;
        depth = Vector3.Distance(transform.position, cam.transform.position);
    }

    void OnMouseUp() { dragging = false; }

    void Update()
    {
        if (!dragging || !cam) return;

        // scroll = depth
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f) depth = Mathf.Max(0.2f, depth - scroll * zStepPerScroll);

        // target world pos from mouse + depth
        var mouse = Input.mousePosition;
        mouse.z = depth;
        Vector3 target = cam.ScreenToWorldPoint(mouse);

        if (rb)
            rb.linearVelocity = (target - transform.position) * followSpeed;
        else
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
    }
}
