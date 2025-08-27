using UnityEngine;
using UnityEngine.InputSystem;
using PlayerForShop;

public class CameraRiglook : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float pivotRotationSpeed = 5f;

    private void HandleCameraPivot()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * pivotRotationSpeed * Time.deltaTime;
        float mouseY = -Mouse.current.delta.y.ReadValue() * pivotRotationSpeed * Time.deltaTime;

        // Rotate pivot horizontally
        cameraPivot.Rotate(Vector3.up, mouseX, Space.World);

        // Optionally rotate pivot vertically (clamped)
        Vector3 angles = cameraPivot.localEulerAngles;
        angles.x += mouseY;
        if (angles.x > 180) angles.x -= 360; // convert to -180..180
        angles.x = Mathf.Clamp(angles.x, -10f, 60f);
        cameraPivot.localEulerAngles = new Vector3(angles.x, angles.y, 0);
    }
}