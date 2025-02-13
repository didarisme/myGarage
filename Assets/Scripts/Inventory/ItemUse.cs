using UnityEngine;

public class ItemUse : MonoBehaviour
{
    [SerializeField] private int currentSlot = 0;

    private int invSize = 1;

    private Player playerInput;
    private ObjectPlacement objectPlacement;
    private InventoryManager inventory;
    private InventoryUI inventoryUI;
    private Item item;

    private void Start()
    {
        playerInput = PlayerInput.Instance.GetPlayerInput();
        objectPlacement = FindObjectOfType<ObjectPlacement>();
        inventory = GetComponent<InventoryManager>();
        inventoryUI = GetComponent<InventoryUI>();
        inventoryUI.ActivateSlot(currentSlot);
    }

    private void Update()
    {
        HandleSlotSelection();

        if (playerInput.PlayerMain.StartPlacing.triggered)
        {
            UseThisItem();
        }
        else if (playerInput.PlayerMain.Drop.triggered && !objectPlacement.IsPlacing)
        {
            inventory.DropFromSlot(currentSlot);
        }
    }

    private void HandleSlotSelection()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int slotNumber);
            slotNumber--;

            if (!isNumber) return;

            SelectSlot(slotNumber);
        }
    }

    public void SelectSlot(int slotNumber)
    {
        if (slotNumber >= 0 && slotNumber < invSize)
        {
            inventoryUI.ActivateSlot(slotNumber);
            currentSlot = slotNumber;
        }
    }

    public void SetInventorySize(int newSize)
    {
        invSize = newSize;
    }

    public void UseThisItem()
    {
        item = inventory.GetSelectedItem(false, currentSlot);

        if (item == null) return;

        if (item.actionType == ActionType.Usable)
        {
            Debug.Log("Nothing"); //Extend it by your own
        }
        else if (item.actionType == ActionType.PlaceableObject)
        {
            objectPlacement.SelectObject(item, currentSlot);
        }
        else
        {
            Debug.Log("Nothing"); //Extend it by your own
        }
    }
}