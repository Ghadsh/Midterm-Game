using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


namespace PlayerForShop
{

    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerObj;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform broomTransform;
        [SerializeField] private Transform broomGripPoint;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float pickupRange = 5f;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        private readonly int SpeedHash = Animator.StringToHash("Speed");

        private PlayerInputAction inputAction;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private Vector3 moveDirection;

        private bool isNearBroom = false;
        private bool isPickingUp = false;
        private Vector3 broomOriginalPosition;
        private Quaternion broomOriginalRotation;
        private Transform broomOriginalParent;

        private void Awake()
        {
            inputAction = new PlayerInputAction();
            inputAction.Player.Enable();
        }

        private void Update()
        {
            moveInput = inputAction.Player.Move.ReadValue<Vector2>();
            lookInput = inputAction.Player.Look.ReadValue<Vector2>();

            HandleCameraRotation();
            HandleInput();
            HandleMovement();
            HandleAnimationTriggers();
            UpdateAnimatorSpeed();
            CheckBroomDistance();
        }

        private void HandleCameraRotation()
        {
            float xRotation = -lookInput.y;
            float yRotation = lookInput.x;

            Quaternion cameraRot = Quaternion.Euler(xRotation, yRotation, 0f);
            Quaternion playerRot = Quaternion.Euler(0f, yRotation, 0f);

            playerCamera.rotation = cameraRot;
            orientation.rotation = playerRot;
        }

        private void HandleInput()
        {
            Vector3 camForward = new Vector3(playerCamera.forward.x, 0f, playerCamera.forward.z).normalized;
            Vector3 camRight = new Vector3(playerCamera.right.x, 0f, playerCamera.right.z).normalized;

            moveDirection = camForward * moveInput.y + camRight * moveInput.x;
        }

        private void HandleMovement()
        {
            if (!isPickingUp)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                    playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRot, Time.deltaTime * rotationSpeed);
                }
            }
        }

        private void UpdateAnimatorSpeed()
        {
            animator.SetFloat(SpeedHash, moveDirection.magnitude, 0.1f, Time.deltaTime);
        }

        private void HandleAnimationTriggers()
        {
            if (Input.GetKeyDown(KeyCode.P)) animator.SetTrigger("Plant");
            if (Input.GetKeyDown(KeyCode.C)) animator.SetTrigger("PickCrops");

            if (animator.GetBool("HasBroom"))
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    animator.SetBool("Clean", true);
                }
                else
                {
                    animator.SetBool("Clean", false);

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        broomTransform.SetParent(broomOriginalParent);
                        broomTransform.position = broomOriginalPosition;
                        broomTransform.rotation = broomOriginalRotation;

                        animator.SetBool("HasBroom", false);
                    }
                }
            }
        }

        private void CheckBroomDistance()
        {
            float distance = Vector3.Distance(transform.position, broomTransform.position);
            isNearBroom = distance <= pickupRange;

            if (isNearBroom && Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("Trying to pick up broom...");
                StartCoroutine(PickUpBroomRoutine());
            }
        }

        public IEnumerator PickUpBroomRoutine()
        {
            isPickingUp = true;
            moveDirection = Vector3.zero;

            Vector3 direction = broomTransform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetTrigger("PickBroom");

            yield return new WaitForSeconds(1.5f);

            animator.SetBool("HasBroom", true);

            broomOriginalPosition = broomTransform.position;
            broomOriginalRotation = broomTransform.rotation;
            broomOriginalParent = broomTransform.parent;

            broomTransform.SetParent(broomGripPoint);
            broomTransform.localPosition = Vector3.zero;
            broomTransform.localRotation = Quaternion.identity;

            isPickingUp = false;
        }

        private void OnDisable()
        {
            inputAction.Player.Disable();
        }
    }
}
