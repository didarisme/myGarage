using System;
using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private bool IsSprinting => canSprint && playerInput.PlayerMain.Sprint.ReadValue<float>() > 0 && isGrounded;
    private bool ShouldJump => playerInput.PlayerMain.Jump.triggered && isGrounded;
    private bool ShouldCrouch => playerInput.PlayerMain.Crouch.triggered && !duringCrouchAnimation && isGrounded;

    [Header("Links")]
    [SerializeField] private Transform cameraHandler;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 30f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 1.35f;
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float timeToCroach = 0.5f;
    [SerializeField] [Tooltip("use if CharacterController Center Y != 0")] private Vector3 crouchingCenter = new Vector3(0, 0.675f, 0);
    [SerializeField] [Tooltip("use if CharacterController Center Y != 0")] private Vector3 standingCenter = new Vector3(0, 0.925f, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Ground Parameters")]
    [SerializeField] private float GroundedOffset = 0.14f;
    [SerializeField] private float GroundedRadius = 0.4f;
    [SerializeField] private LayerMask GroundLayers;
    private bool isGrounded = true;

    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    public event Action<Vector2, bool> OnMovement;
    public MovementState CurrentState { get; private set; }

    public enum MovementState
    {
        walking,
        running,
        crouchning
    }

    private Player playerInput;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = PlayerInput.Instance.GetPlayerInput();
    }

    private void Update()
    {
        HandleState();
        HandleMovementInput();
        GroundedCheck();

        if (canJump)
            HandleJump();

        if (canCrouch)
            HandleCrouch();

        ApplyFinalMovements();
    }

    private void HandleState()
    {
        CurrentState = isCrouching ? MovementState.crouchning : IsSprinting ? MovementState.running : MovementState.walking;
    }

    private void HandleMovementInput()
    {
        currentInput = playerInput.PlayerMain.Move.ReadValue<Vector2>();

        Vector2 inputNormalized = currentInput.normalized;
        inputNormalized.x = Mathf.Abs(inputNormalized.x);
        inputNormalized.y = Mathf.Abs(inputNormalized.y);

        currentInput = new Vector2(Mathf.Clamp(currentInput.x, -inputNormalized.x, inputNormalized.x), Mathf.Clamp(currentInput.y, -inputNormalized.y, inputNormalized.y));
        currentInput *= isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.y) + (transform.TransformDirection(Vector3.right) * currentInput.x);
        moveDirection.y = moveDirectionY;
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void ApplyFinalMovements()
    {
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            moveDirection.y = Mathf.Clamp(moveDirection.y, -20f, 20f);
        }
        else if (moveDirection.y < 0f && isGrounded)
        {
            moveDirection.y = 0f;
        }

        OnMovement?.Invoke(currentInput, isGrounded);
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(cameraHandler.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        float cameraOffset = characterController.height - cameraHandler.localPosition.y;

        float heightVelocity = 0f;
        Vector3 centerVelocity = Vector3.zero;

        isCrouching = !isCrouching;

        while (Mathf.Abs(characterController.height - targetHeight) > 0.01f || Vector3.Distance(characterController.center, targetCenter) > 0.01f)
        {
            characterController.height = Mathf.SmoothDamp(characterController.height, targetHeight, ref heightVelocity, timeToCroach);
            characterController.center = Vector3.SmoothDamp(characterController.center, targetCenter, ref centerVelocity, timeToCroach);

            cameraHandler.localPosition = new Vector3(cameraHandler.localPosition.x, characterController.height - cameraOffset, cameraHandler.localPosition.z);

            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;
        cameraHandler.localPosition = new Vector3(cameraHandler.localPosition.x, characterController.height - cameraOffset, cameraHandler.localPosition.z);

        duringCrouchAnimation = false;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + GroundedOffset, transform.position.z), GroundedRadius);
    }
}