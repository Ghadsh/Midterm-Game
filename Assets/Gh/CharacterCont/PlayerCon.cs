//using UnityEngine;
//using UnityEngine.InputSystem;
//using System.Collections;
//using PlayerForShop;

//public class PlayerCon : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] private Transform broomTransform;
//    [SerializeField] private Transform broomGripPoint;
//    [SerializeField] private PlayerAnimator playerAnimator;

//    [Header("Settings")]
//    [SerializeField] private float pickupRange = 5f;

//    private PlayerInputAction inputAction;

//    private bool isNearBroom = false;
//    private bool isPickingUp = false;
//    private Vector3 broomOriginalPosition;
//    private Quaternion broomOriginalRotation;
//    private Transform broomOriginalParent;

//    private void Start()
//    {
//        if (playerAnimator == null)
//        {
//            Debug.LogError("PlayerAnimator not assigned in PlayerCon!");
//        }
//    }


//    private void Awake()
//    {
//        inputAction = new PlayerInputAction();
//        inputAction.Player.Enable();


//    }

//    private void Update()
//    {
//        HandleAnimationTriggers();
//        CheckBroomDistance();
//    }

//    private void HandleAnimationTriggers()
//    {
//        if (Keyboard.current.pKey.wasPressedThisFrame) playerAnimator.Trigger("Plant");
//        if (Keyboard.current.cKey.wasPressedThisFrame) playerAnimator.Trigger("PickCrops");

//        if (playerAnimator.GetBool("HasBroom"))
//        {
//            if (Keyboard.current.spaceKey.isPressed)
//            {
//                playerAnimator.SetBool("Clean", true);
//            }
//            else
//            {
//                playerAnimator.SetBool("Clean", false);

//                if (Keyboard.current.rKey.wasPressedThisFrame)
//                {
//                    broomTransform.SetParent(broomOriginalParent);
//                    broomTransform.position = broomOriginalPosition;
//                    broomTransform.rotation = broomOriginalRotation;

//                    playerAnimator.SetBool("HasBroom", false);
//                }
//            }
//        }
//    }

//    private void CheckBroomDistance()
//    {
//        float distance = Vector3.Distance(transform.position, broomTransform.position);
//        isNearBroom = distance <= pickupRange;

//        if (isNearBroom && Keyboard.current.bKey.wasPressedThisFrame)
//        {
//            Debug.Log("Trying to pick up broom...");
//            StartCoroutine(PickUpBroomRoutine());
//        }
//    }

//    public IEnumerator PickUpBroomRoutine()
//    {
//        isPickingUp = true;

//        Vector3 direction = broomTransform.position - transform.position;
//        direction.y = 0f;
//        direction.Normalize();
//        transform.rotation = Quaternion.LookRotation(direction);

//        playerAnimator.Trigger("PickBroom");

//        yield return new WaitForSeconds(1.5f);

//        playerAnimator.SetBool("HasBroom", true);

//        broomOriginalPosition = broomTransform.position;
//        broomOriginalRotation = broomTransform.rotation;
//        broomOriginalParent = broomTransform.parent;

//        broomTransform.SetParent(broomGripPoint);
//        broomTransform.localPosition = Vector3.zero;
//        broomTransform.localRotation = Quaternion.identity;

//        isPickingUp = false;
//    }

//    private void OnDisable()
//    {
//        inputAction.Player.Disable();
//    }
//}
//using UnityEngine;
//using UnityEngine.SceneManagement;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerCon : MonoBehaviour
//{
//    [Header("Movement")]
//    public float walkSpeed = 3f;
//    public float runSpeed = 6f;
//    public float rotationSpeed = 150f;
//    public float gravity = -9.81f;

//    private CharacterController controller;
//    private Vector3 velocity;
//    private bool isGrounded;
//    private Vector3 currentVelocity;

//    [Header("Animation")]
//    [SerializeField] private PlayerAnimator playerAnimator; 
//    [SerializeField] private float rotationSmoothTime = 0.15f;

//    private float currentYaw;
//    private float yawVelocity;

//    private Vector2 lookInput;

//    private void Start()
//    {
//        controller = GetComponent<CharacterController>();
//    }

//    private void Update()
//    {

//        lookInput.x = Input.GetAxis("Mouse X");
//        lookInput.y = Input.GetAxis("Mouse Y");

//        HandleMovement();
//        HandleRotation();
//    }

//    private void HandleMovement()
//    {
//        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.2f, 0.3f, LayerMask.GetMask("Ground"));

//        if (isGrounded && velocity.y < 0) velocity.y = -2f;

//        float horizontal = Input.GetAxisRaw("Horizontal");
//        float vertical = Input.GetAxisRaw("Vertical");
//        bool runPressed = Input.GetKey(KeyCode.LeftShift);
//        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

//        float targetSpeed = (runPressed ? runSpeed : walkSpeed) * inputDir.magnitude;
//        Vector3 desiredVelocity = transform.TransformDirection(inputDir) * targetSpeed;
//        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, 10f * Time.deltaTime);

//        controller.Move(currentVelocity * Time.deltaTime);

//        velocity.y += gravity * Time.deltaTime;
//        controller.Move(velocity * Time.deltaTime);

//        float speedPercent = currentVelocity.magnitude / runSpeed;
//        playerAnimator.SetSpeed(speedPercent); 
//    }

//    private void HandleRotation()
//    {
//        currentYaw += lookInput.x * rotationSpeed * Time.deltaTime;
//        float smoothedYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentYaw, ref yawVelocity, rotationSmoothTime);
//        transform.rotation = Quaternion.Euler(0f, smoothedYaw, 0f);
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using PlayerForShop;

[RequireComponent(typeof(CharacterController))]
public class PlayerCon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerObj;      // The body/mesh of the player
    [SerializeField] private Transform playerCamera;   // Camera to rotate
    [SerializeField] private Transform broomTransform;
    [SerializeField] private Transform broomGripPoint;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float pickupRange = 5f;

    [Header("CharacterController")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 moveDirection;
    private Vector3 velocity;
    private bool isPickingUp = false;
    private bool isNearBroom = false;
    private Vector3 broomOriginalPosition;
    private Quaternion broomOriginalRotation;
    private Transform broomOriginalParent;

    private PlayerInputAction inputAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        inputAction = new PlayerInputAction();
        inputAction.Player.Enable();
    }

    private void Update()
    {
        if (!isPickingUp)
        {
            moveInput = inputAction.Player.Move.ReadValue<Vector2>();
            lookInput = inputAction.Player.Look.ReadValue<Vector2>();

            HandleCameraRotation();
            HandleMovement();
            HandleAnimationTriggers();
            CheckBroomDistance();
        }
    }

    private void HandleCameraRotation()
    {
        // Rotate camera based on mouse
        float xRotation = -lookInput.y;
        float yRotation = lookInput.x;

        playerCamera.Rotate(new Vector3(xRotation, 0f, 0f));
        playerObj.Rotate(Vector3.up * yRotation * rotationSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Camera-aligned movement
        Vector3 camForward = new Vector3(playerCamera.forward.x, 0f, playerCamera.forward.z).normalized;
        Vector3 camRight = new Vector3(playerCamera.right.x, 0f, playerCamera.right.z).normalized;

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animator speed
        playerAnimator.SetSpeed(moveDirection.magnitude);
    }

    private void HandleAnimationTriggers()
    {
        // Trigger planting/crop animations
        if (Keyboard.current.pKey.wasPressedThisFrame) playerAnimator.Trigger("Plant");
        if (Keyboard.current.cKey.wasPressedThisFrame) playerAnimator.Trigger("PickCrops");

        // Handle broom cleaning
        if (playerAnimator.GetBool("HasBroom"))
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                playerAnimator.SetBool("Clean", true);
            }
            else
            {
                playerAnimator.SetBool("Clean", false);

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    // Drop broom
                    broomTransform.SetParent(broomOriginalParent);
                    broomTransform.position = broomOriginalPosition;
                    broomTransform.rotation = broomOriginalRotation;
                    playerAnimator.SetBool("HasBroom", false);
                }
            }
        }
    }

    private void CheckBroomDistance()
    {
        float distance = Vector3.Distance(transform.position, broomTransform.position);
        isNearBroom = distance <= pickupRange;

        if (isNearBroom && Keyboard.current.bKey.wasPressedThisFrame)
        {
            StartCoroutine(PickUpBroomRoutine());
        }
    }

    public IEnumerator PickUpBroomRoutine()
    {
        isPickingUp = true;
        moveDirection = Vector3.zero;

        // Rotate toward broom
        Vector3 direction = broomTransform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        playerObj.rotation = Quaternion.LookRotation(direction);

        // Trigger pick animation
        playerAnimator.Trigger("PickBroom");

        // Wait for animation to finish (animation event will handle parenting)
        yield return new WaitForSeconds(1.5f);
    }

    // Called at the end of PickBroom animation via Animation Event
    public void OnPickBroomDone()
    {
        broomOriginalPosition = broomTransform.position;
        broomOriginalRotation = broomTransform.rotation;
        broomOriginalParent = broomTransform.parent;

        broomTransform.SetParent(broomGripPoint);
        broomTransform.localPosition = Vector3.zero;
        broomTransform.localRotation = Quaternion.identity;

        playerAnimator.SetBool("HasBroom", true);
        isPickingUp = false;
    }

    private void OnDisable()
    {
        inputAction.Player.Disable();
    }
}