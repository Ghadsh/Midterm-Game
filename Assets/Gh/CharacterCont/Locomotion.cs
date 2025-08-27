//using UnityEngine;
//using UnityEngine.InputSystem;
//using PlayerForShop;

//[RequireComponent(typeof(CharacterController))]
//public class Locomotion : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] private Transform cameraTransform;
//    [SerializeField] private Animator animator;

//    [Header("Settings")]
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private float rotationSpeed = 10f;
//    [SerializeField] private float gravity = -9.81f;

//    private CharacterController controller;
//    private PlayerInputAction inputAction;
//    private Vector2 moveInput;
//    private float verticalVelocity;

//    private readonly int SpeedHash = Animator.StringToHash("Speed");

//    private void Awake()
//    {
//        controller = GetComponent<CharacterController>();
//        inputAction = new PlayerInputAction();
//    }

//    private void OnEnable()
//    {
//        inputAction.Player.Enable();
//        inputAction.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
//        inputAction.Player.Move.canceled += ctx => moveInput = Vector2.zero;
//    }

//    private void OnDisable()
//    {
//        inputAction.Player.Disable();
//    }

//    private void Update()
//    {
//        HandleMovement();
//        UpdateAnimator();
//    }

//    private void HandleMovement()
//    {
//        // Input magnitude
//        if (moveInput.sqrMagnitude < 0.01f) return;

//        // Camera-based direction
//        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
//        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
//        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

//        // Move
//        controller.Move(moveDir * moveSpeed * Time.deltaTime);

//        // Rotate smoothly toward movement direction
//        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

//        // Gravity
//        if (controller.isGrounded && verticalVelocity < 0f)
//            verticalVelocity = -2f;
//        verticalVelocity += gravity * Time.deltaTime;
//        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
//    }

//    private void UpdateAnimator()
//    {
//        animator.SetFloat(SpeedHash, moveInput.magnitude, 0.1f, Time.deltaTime);
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using PlayerForShop;

[RequireComponent(typeof(CharacterController))]
public class Locomotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    private CharacterController controller;
    private PlayerInputAction inputAction;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 currentVelocity;
    private float verticalVelocity;

    private float currentYaw;
    private float yawVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputAction.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputAction.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputAction.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputAction.Player.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimation();
    }

    private void HandleMovement()
    {
        // Ground check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;

        // Smooth movement
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float targetSpeed = ((Keyboard.current.leftShiftKey.isPressed ? runSpeed : walkSpeed) * inputDir.magnitude);
        Vector3 desiredVelocity = transform.TransformDirection(inputDir) * targetSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, 10f * Time.deltaTime);

        controller.Move(currentVelocity * Time.deltaTime);

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Smooth rotation based on look input
        currentYaw += lookInput.x * rotationSpeed * Time.deltaTime;
        float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentYaw, ref yawVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);

        // Optional: if you want vertical camera rotation
        // cameraTransform.localRotation = Quaternion.Euler(-lookInput.y, 0f, 0f);
    }

    private void UpdateAnimation()
    {
        float speedPercent = currentVelocity.magnitude / runSpeed;
        playerAnimator.SetSpeed(speedPercent);
    }
}