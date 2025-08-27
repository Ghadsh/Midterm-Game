using UnityEngine;

public class MThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;      // player root/rig
    public Transform PlayerObj;   // the visible character mesh
    public Rigidbody rb;
    public Transform cam;         // assign Camera.main.transform in inspector

    [Header("Tuning")]
    public float rotationSpeed = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (cam == null) cam = Camera.main.transform;
    }

    void Update()
    {
        // 1) Make orientation follow the camera YAW
        Vector3 camForward = cam.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = cam.right; camRight.y = 0f; camRight.Normalize();
        orientation.forward = camForward;

        // 2) Read input relative to camera
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 3) Rotate player: when moving -> face moveDir, when idle -> face camera yaw
        Vector3 targetDir = moveDir.sqrMagnitude > 0.0001f ? moveDir : camForward;
        PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, targetDir, rotationSpeed * Time.deltaTime);
    }
}
