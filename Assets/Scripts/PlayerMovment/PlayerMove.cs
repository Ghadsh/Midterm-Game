using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform broomTransform;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform broomGripPoint;
    //[SerializeField] private Transform Visual;





    [Header("Animation")]
    private Animator anim;
    private readonly int SpeedHash = Animator.StringToHash("Speed");

    private Rigidbody rb;
    private Vector3 input;
    private float rotationY;
    private bool isNearBroom = false;
    private bool isPickingUp = false;
    private Vector3 broomOriginalPosition;
    private Quaternion broomOriginalRotation;
    private Transform broomOriginalParent;





    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); 
        anim.applyRootMotion = false;
        rb.freezeRotation = true;
    }


    void Update()
    {

        float distance = Vector3.Distance(transform.position, broomTransform.position);
        isNearBroom = distance <= pickupRange;

        if (isNearBroom && Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Trying to pick up broom...");
            StartCoroutine(PickUpBroomRoutine());
        }

        
        HandleAnimationTriggers();
        UpdateAnimatorSpeed();
        HandleInput();
    }

    void FixedUpdate()
    {
        if (!isPickingUp)
        {
            //HandleInput();
            HandleMovement();
           
            Quaternion currentRot = rb.rotation;
            currentRot.x = 0f;
            currentRot.z = 0f;
            rb.MoveRotation(currentRot);

        }

    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0f;
        right.y = 0f;

        input = (forward * v + right * h).normalized;
    }

    //private void HandleMovement()
    //{
    //    if (input.magnitude > 0.1f)
    //    {

    //        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

    //        Vector3 moveDir = input;
    //        if (Vector3.Dot(moveDir.normalized, orientation.forward) > -0.9f)
    //        {
    //            Quaternion targetRot = Quaternion.LookRotation(moveDir);
    //            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
    //        }
    //    }
    //}
    private void HandleMovement()
    {
        if (input.magnitude > 0.1f)
        {
            // Move the Rigidbody
            rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);

            Vector3 moveDir = input;

            // Rotate only if the movement is NOT purely left/right
            float dot = Vector3.Dot(moveDir.normalized, orientation.forward);
            if (dot > -0.9f && Mathf.Abs(dot) > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
            }
        }
    }



    private void UpdateAnimatorSpeed()
    {
        anim.SetFloat(SpeedHash, input.magnitude, 0.1f, Time.deltaTime);
        Debug.Log("Speed: " + input.magnitude);

    }

    private void HandleAnimationTriggers()
    {

        if (Input.GetKeyDown(KeyCode.P)) anim.SetTrigger("Plant");
        if (Input.GetKeyDown(KeyCode.C)) anim.SetTrigger("PickCrops");

        if (anim.GetBool("HasBroom"))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetBool("Clean", true);
            }
            else
            {
                anim.SetBool("Clean", false);


                if (Input.GetKeyDown(KeyCode.R))
                {
                    broomTransform.SetParent(broomOriginalParent);
                    broomTransform.position = broomOriginalPosition;
                    broomTransform.rotation = broomOriginalRotation;

                    anim.SetBool("HasBroom", false);
                }
            }
        }

    }


    private void OnPickBroomDone()
    {
        anim.SetBool("HasBroom", true);
    }


    public IEnumerator PickUpBroomRoutine()
    {
        isPickingUp = true;

        input = Vector3.zero;

        Vector3 direction = broomTransform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction);


        anim.SetTrigger("PickBroom");

        yield return new WaitForSeconds(1.5f);

        anim.SetBool("HasBroom", true);


        broomOriginalPosition = broomTransform.position;
        broomOriginalRotation = broomTransform.rotation;
        broomOriginalParent = broomTransform.parent;


        broomTransform.SetParent(broomGripPoint);
        broomTransform.localPosition = Vector3.zero;
        broomTransform.localRotation = Quaternion.identity;


        isPickingUp = false;
    }


}