using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class StationFocusCamera : MonoBehaviour
{
    [Header("Look")]
    public float lookSensitivity = 0.15f;
    public float pitchClamp = 85f;
    public float lookSmoothing = 12f;

    [Header("Move (optional)")]
    public bool allowWASD = true;
    public float moveSpeed = 1.5f;
    public float sprintMultiplier = 2f;

    [Header("Lens")]
    public float normalFOV = 60f;
    public float focusFOV = 50f;
    public float fovLerp = 10f;

    [Header("Behavior")]
    public bool staticDuringFocus = true;   // <- keep camera fixed while focusing

    [Header("Integration")]
    [Tooltip("Assign if your Main Camera has a CinemachineBrain.")]
    public Behaviour cinemachineBrainToDisable;

    [Header("Timing")]
    public bool useUnscaledTime = true;

    public bool useExactAnchorPose = true;

    public bool IsActive { get; private set; }

    Transform _prevParent; Vector3 _prevPos; Quaternion _prevRot;
    float _yaw, _pitch;        // still stored for persistence if you ever want it
    float _currYaw, _currPitch;
    Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam) _cam.fieldOfView = normalFOV;
        var e = transform.rotation.eulerAngles;
        _yaw = _currYaw = e.y;
        _pitch = _currPitch = e.x;
    }

    public (float yaw, float pitch) GetAngles() => (_yaw, _pitch);
    public void SetAngles(float yaw, float pitch) { _yaw = yaw; _pitch = pitch; _currYaw = yaw; _currPitch = pitch; }
    public void EnterFocus(Transform anchor, Vector3 localOffset, Quaternion localRotation)
    {
        if (IsActive || anchor == null) return;

        // Save world pose to restore on Exit
        _prevParent = transform.parent;
        _prevPos = transform.position;
        _prevRot = transform.rotation;

        // Compute the exact world pose from the anchor (independent of prefab center)
        Vector3 worldPos = anchor.TransformPoint(localOffset);
        Quaternion worldRot = anchor.rotation * localRotation;

        // Detach from any parent and set the exact world pose (static FP cam)
        transform.SetParent(null, true);
        transform.SetPositionAndRotation(worldPos, worldRot);

        // Initialize internal angles (even if staticDuringFocus blocks look)
        var e = transform.rotation.eulerAngles;
        _yaw = _currYaw = e.y;
        _pitch = _currPitch = e.x;

        // Input/camera integration
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cinemachineBrainToDisable) cinemachineBrainToDisable.enabled = false;

        IsActive = true;
    }

    public void ExitFocus()
    {
        if (!IsActive) return;

        transform.SetParent(_prevParent, true);
        transform.SetPositionAndRotation(_prevPos, _prevRot);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cinemachineBrainToDisable) cinemachineBrainToDisable.enabled = true;

        IsActive = false;

        if (_cam) _cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        if (!IsActive) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // FOV easing while in focus
        if (_cam) _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, focusFOV, 1f - Mathf.Exp(-fovLerp * dt));

        // STATIC MODE: no look/no move at all
        if (staticDuringFocus) return;

        // (non-static mode below — still available if you ever toggle it back on)
        Vector2 look = ReadLookDelta();
        _yaw += look.x * lookSensitivity;
        _pitch -= look.y * lookSensitivity;
        _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);

        _currYaw = Mathf.LerpAngle(_currYaw, _yaw, 1f - Mathf.Exp(-lookSmoothing * dt));
        _currPitch = Mathf.LerpAngle(_currPitch, _pitch, 1f - Mathf.Exp(-lookSmoothing * dt));
        transform.rotation = Quaternion.Euler(_currPitch, _currYaw, 0f);

        if (allowWASD)
        {
            Vector3 mv = ReadMoveAxes();
            bool sprint = ReadSprint();
            float spd = moveSpeed * (sprint ? sprintMultiplier : 1f);
            transform.position += mv * spd * dt;
        }
    }

    Vector2 ReadLookDelta()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
#else
        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 10f;
#endif
    }

    Vector3 ReadMoveAxes()
    {
#if ENABLE_INPUT_SYSTEM
        float x = 0, y = 0, z = 0; var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed) x -= 1; if (kb.dKey.isPressed) x += 1;
            if (kb.sKey.isPressed) z -= 1; if (kb.wKey.isPressed) z += 1;
            if (kb.spaceKey.isPressed) y += 1; if (kb.leftCtrlKey.isPressed) y -= 1;
        }
        Vector3 fwd = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 v = (right * x + fwd * z + Vector3.up * y);
        return v.sqrMagnitude > 1 ? v.normalized : v;
#else
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float y = (Input.GetKey(KeyCode.Space)?1:0) + (Input.GetKey(KeyCode.LeftControl)?-1:0);
        Vector3 fwd = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 v = (right * x + fwd * z + Vector3.up * y);
        return v.sqrMagnitude > 1 ? v.normalized : v;
#endif
    }

    bool ReadSprint()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        return kb != null && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed);
#else
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
    }

    public void EnterFocusExact(Transform anchor)
    {
        if (IsActive || anchor == null) return;

        // Save world pose so we can restore on exit
        _prevParent = transform.parent;
        _prevPos = transform.position;
        _prevRot = transform.rotation;

        // Do NOT parent to station if you want to be totally independent of prefab pivots.
        // We snap to the anchor's *world* position/rotation and stay static there.
        transform.SetParent(null, true);
        transform.SetPositionAndRotation(anchor.position, anchor.rotation);

        // Initialize internal angles (even if staticDuringFocus is true)
        var e = transform.rotation.eulerAngles;
        _yaw = _currYaw = e.y;
        _pitch = _currPitch = e.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cinemachineBrainToDisable) cinemachineBrainToDisable.enabled = false;

        IsActive = true;
    }

}
