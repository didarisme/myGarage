using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Transform cameraTransform;

    [Header("Look Parameters")]
    [SerializeField] private bool useHeadbob = true;
    [SerializeField, Range(0.1f, 3f)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(0.1f, 3f)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    private float rotationX = 0;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 12f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.8f;
    [SerializeField] private float crouchBobSpeed = 9f;
    [SerializeField] private float crouchBobAmount = 0.03f;
    private float defaultYPos = 0;
    private float timer;

    private PlayerMove playerMove;
    private Player playerInput;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        playerMove = GetComponent<PlayerMove>();
        defaultYPos = cameraTransform.transform.localPosition.y;
    }

    private void Start()
    {
        playerInput = PlayerInput.Instance.GetPlayerInput();
    }

    private void OnEnable()
    {
        playerMove.OnMovement += HandleHeadBob;
    }

    private void OnDisable()
    {
        playerMove.OnMovement -= HandleHeadBob;
    }

    private void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        Vector2 currentInput = playerInput.PlayerMain.Look.ReadValue<Vector2>();

        rotationX -= currentInput.y * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        cameraTransform.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, currentInput.x * lookSpeedX, 0);
    }

    private void HandleHeadBob(Vector2 currentInput, bool isGrounded)
    {
        if (!useHeadbob) return;
        if (!isGrounded) return;

        if (Mathf.Abs(currentInput.x) > 0.1f || Mathf.Abs(currentInput.y) > 0.1f)
        {
            timer += Time.deltaTime * (playerMove.CurrentState == PlayerMove.MovementState.crouchning ?
                crouchBobSpeed : playerMove.CurrentState == PlayerMove.MovementState.running ? sprintBobSpeed : walkBobSpeed);

            //skyscraper
            cameraTransform.transform.localPosition = new Vector3(
                cameraTransform.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (playerMove.CurrentState == PlayerMove.MovementState.crouchning ?
                crouchBobAmount : playerMove.CurrentState == PlayerMove.MovementState.running ? sprintBobAmount : walkBobAmount),
                cameraTransform.transform.localPosition.z);
        }
    }
}