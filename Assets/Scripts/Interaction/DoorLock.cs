using UnityEngine;
using UnityEngine.Events;

public class DoorLock : Interactable
{
    [SerializeField] private UnityEvent lockEvent;
    [SerializeField] private UnityEvent unlockEvent;
    [SerializeField] private Item keyItem;

    private InventoryManager inventory;
    private Outline outline;

    private void Start()
    {
        lockEvent?.Invoke();

        inventory = FindObjectOfType<InventoryManager>();

        if (TryGetComponent<Outline>(out outline))
            outline.enabled = false;

        if (keyItem == null)
            Debug.Log("Keys not connected!");
    }

    public override void OnFocus()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public override void OnInteract()
    {
        if (inventory.GetItemCount(keyItem) > 0)
        {
            inventory.UseItem(keyItem, 1);
            unlockEvent?.Invoke();
        }
        else
        {
            Debug.Log("Find keys!");
        }
    }

    public override void OnLoseFocus()
    {
        if (outline != null)
            outline.enabled = false;
    }
}