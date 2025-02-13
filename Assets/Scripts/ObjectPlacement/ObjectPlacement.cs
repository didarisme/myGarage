using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private GameObject[] placeObjects;
    [SerializeField] private Material[] materials;
    [SerializeField] private float placeDistance = 3.5f;
    [SerializeField] private float rotateAmount = 90f;
    [SerializeField] private LayerMask layerMask;

    private Vector3 placePosition;
    private int currentIndex, currentSlot;
    private bool isGreen = true;
    private bool canPlace = true;

    [HideInInspector] public bool IsTriggering = false;
    [HideInInspector] public bool IsPlacing { get => currentIndex != -1; }

    private Player playerInput;
    private InventoryManager inventory;
    private Item placeItem;
    private InputUI inputUI;

    private void Start()
    {
        playerInput = PlayerInput.Instance.GetPlayerInput();
        inventory = FindObjectOfType<InventoryManager>();
        inputUI = FindObjectOfType<InputUI>();

        currentIndex = -1;
        currentSlot = -1;
    }

    private void LateUpdate()
    {
        if (currentIndex >= 0f)
        {
            HandlePosition();
            HandleMaterial();
            HandleInput();
        }
    }

    private void HandlePosition()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, placeDistance, layerMask))
        {
            placePosition = hit.point;
            canPlace = !IsTriggering;
        }
        else
        {
            placePosition = ray.origin + ray.direction * placeDistance;
            canPlace = false;
        }

        placeObjects[currentIndex].transform.position = placePosition;
    }

    private void HandleMaterial()
    {
        if (canPlace && !isGreen)
        {
            placeObjects[currentIndex].GetComponentInChildren<MeshRenderer>().material = materials[0];
            isGreen = true;
        }
        else if (!canPlace && isGreen)
        {
            placeObjects[currentIndex].GetComponentInChildren<MeshRenderer>().material = materials[1];
            isGreen = false;
        }
    }

    private void HandleInput()
    {
        if (playerInput.PlayerMain.CancelPlacing.triggered)
        {
            CancelSelection();
        }
        else if (playerInput.PlayerMain.FinishPlacing.triggered && canPlace)
        {
            PlaceObect();
        }
        else if (playerInput.PlayerMain.Rotate.ReadValue<float>() > 0)
        {
            RotateObject();
        }
    }

    private void PlaceObect()
    {
        if (placeItem != null)
        {
            int lastIndex = placeItem.itemPrefab.Length - 1;
            GameObject newItem = Instantiate(placeItem.itemPrefab[lastIndex], placePosition + Vector3.up * 0.2f, placeObjects[currentIndex].transform.rotation);

            inventory.GetSelectedItem(true, currentSlot);
        }
        else
        {
            Debug.Log("Why null? " + placeItem);
        }

        CancelSelection();
    }

    private void RotateObject()
    {
        placeObjects[currentIndex].transform.Rotate(Vector3.up, rotateAmount * Time.deltaTime);
    }

    private void CancelSelection()
    {
        placeObjects[currentIndex].transform.rotation = Quaternion.identity;
        placeObjects[currentIndex].SetActive(false);
        currentIndex = -1;
        currentSlot = -1;
        placeItem = null;

        inputUI.ActivatePlacementBtns(false);
    }

    public void SelectObject(Item item, int slot)
    {
        for (int i = 0; i < placeObjects.Length; i++)
        {
            if (item.itemName == placeObjects[i].name)
            {
                placeObjects[i].SetActive(true);

                currentSlot = slot;
                placeItem = item;

                currentIndex = i;

                inputUI.ActivatePlacementBtns(true);
            }
            else
            {
                placeObjects[i].SetActive(false);
            }
        }
    }
}