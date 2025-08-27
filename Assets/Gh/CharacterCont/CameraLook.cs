using UnityEngine;
using UnityEngine.InputSystem;
using PlayerForShop;
using Unity.Cinemachine; 

public class CameraLook : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCamera; // your Cinemachine Camera
    [SerializeField] private float sensitivity = 1.5f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-30f, 70f);

    private PlayerInputAction inputAction;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        inputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputAction.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputAction.Player.Disable();
    }

    private void LateUpdate()
    {
        // Debug to check if input is received
        // Debug.Log("Look Input: " + lookInput);

        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Rotate the camera
        if (cineCamera != null)
            cineCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}