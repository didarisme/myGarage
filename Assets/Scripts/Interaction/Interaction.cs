using UnityEngine;

public class Interaction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] [Tooltip("Center is (0.5f; 0.5f; 0.0f)")] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;

    private Interactable currentInteractable;
    private bool canInteract = true;
    private bool isCentered = true;

    private Player playerInput;

    private void Start()
    {
        playerInput = PlayerInput.Instance.GetPlayerInput();
    }

    private void Update()
    {
        if (canInteract)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }

    private void HandleInteractionCheck()
    {
        // 3 - Interactable layer
        if (Physics.Raycast(InteractionRay(), out RaycastHit hit, interactionDistance) && hit.collider.gameObject.layer == 3)
        {
            if (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID())
            {
                if (currentInteractable)
                {
                    currentInteractable.OnLoseFocus();
                    currentInteractable = null;
                }

                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnFocus();
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private Ray InteractionRay()
    {
        Ray interactionRay;

        if (isCentered)
        {
            interactionRay = Camera.main.ViewportPointToRay(interactionRayPoint);
        }
        else
        {
            interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        return interactionRay;
    }

    private void HandleInteractionInput()
    {
        if (playerInput.PlayerMain.Interact.triggered)
        {
            if (currentInteractable == null) return;

            if (Physics.Raycast(InteractionRay(), out RaycastHit hit, interactionDistance, interactionLayer))
            {
                currentInteractable.OnInteract();
            }
        }
    }

    public void SetInteractionBlock(bool unlocked)
    {
        canInteract = unlocked;
    }
}