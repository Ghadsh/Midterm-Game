// AimProvider.cs
using UnityEngine;

public class AimProvider : MonoBehaviour
{
    public Camera cam;
    public RectTransform crosshair;   // UI crosshair
    [Range(0.05f, 0.4f)] public float sphereRadius = 0.18f;

    void Awake() { if (!cam) cam = Camera.main; }

    public Ray GetAimRay()
    {
        Vector2 sp = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        if (crosshair) sp = RectTransformUtility.WorldToScreenPoint(null, crosshair.position);
        return cam.ScreenPointToRay(sp);
    }

    // convenience
    public bool SphereCast(float maxDist, LayerMask mask, out RaycastHit hit)
    {
        return Physics.SphereCast(GetAimRay(), sphereRadius, out hit, maxDist, mask, QueryTriggerInteraction.Ignore);
    }
}
