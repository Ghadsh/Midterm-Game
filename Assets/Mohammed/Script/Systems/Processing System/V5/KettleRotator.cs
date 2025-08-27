using UnityEngine;

[DisallowMultipleComponent]
public class KettleRotator : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode tiltForwardKey = KeyCode.Q;
    public KeyCode tiltBackKey = KeyCode.E;
    public KeyCode rollLeftKey = KeyCode.A;
    public KeyCode rollRightKey = KeyCode.D;

    [Header("Speeds (deg/sec)")]
    public float pitchSpeed = 90f;
    public float rollSpeed = 90f;

    [Header("Limits")]
    public bool limitPitch = false;
    [Range(-30f, 0f)] public float minPitchDeg = -5f;   // slight back tilt allowed
    [Range(10f, 120f)] public float maxPitchDeg = 100f;  // forward tilt to pour

    // We treat the kettle's local X as pitch, Z as roll
    float _pitch;  // degrees
    float _roll;   // degrees
    bool _initd;

    void InitFromCurrent()
    {
        var e = transform.localEulerAngles;
        _pitch = NormalizeAngle(e.x);
        _roll = NormalizeAngle(e.z);
        _initd = true;
    }

    float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a > 180f) a -= 360f;
        return a;
    }


    void Update()
    {
        if (!_initd) InitFromCurrent();

        float dt = Time.deltaTime;

        // Input
        if (Input.GetKey(tiltForwardKey)) _pitch += pitchSpeed * dt;
        if (Input.GetKey(tiltBackKey)) _pitch -= pitchSpeed * dt;
        if (Input.GetKey(rollLeftKey)) _roll -= rollSpeed * dt;
        if (Input.GetKey(rollRightKey)) _roll += rollSpeed * dt;

        // Clamp pitch; roll left free (or clamp if you want)
        if (limitPitch) _pitch = Mathf.Clamp(_pitch, minPitchDeg, maxPitchDeg); // ⟵ only if you enable it

        // Apply to local rotation (keep yaw as-is)
        var e = transform.localEulerAngles;
        float yaw = e.y;
        transform.localRotation = Quaternion.Euler(_pitch, yaw, _roll);
    }
}
