using UnityEngine;

public class CollectableItem : Interactable
{
    [SerializeField] private Item item;
    [SerializeField] private int count = 1;

    private InventoryManager inventory;
    private Outline outline;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();

        if (TryGetComponent<Outline>(out outline))
            outline.enabled = false;
    }

    public override void OnFocus()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public override void OnInteract()
    {
        int newAmount = inventory.AddItem(item, count);

        if (newAmount <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            count = newAmount;
            Debug.Log(item.name + ": " + count);
        }
    }

    public override void OnLoseFocus()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void SetItem(Item newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }
}