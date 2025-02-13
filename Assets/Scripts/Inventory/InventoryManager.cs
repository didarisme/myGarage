using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject inventoryParent;

    public InventorySlot[] InventorySlots { get => inventorySlots; }

    private void Awake()
    {
        GetComponent<InventoryUI>().InitializeSlots(inventorySlots);
        GetComponent<ItemUse>().SetInventorySize(inventorySlots.Length);
    }

    public int AddItem(Item item, int addAmount)
    {
        //--------CHECK-FOR-SAME-ITEM--------//
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < itemInSlot.item.itemsInStack)
            {
                if (addAmount < itemInSlot.item.itemsInStack - itemInSlot.count)
                {
                    itemInSlot.count += addAmount;
                    addAmount = 0;
                }
                else
                {
                    addAmount -= itemInSlot.item.itemsInStack - itemInSlot.count;
                    itemInSlot.count = itemInSlot.item.itemsInStack;
                }

                itemInSlot.RefreshCount();

                if (addAmount <= 0)
                    return 0; 
            }
        }

        //--------CHECK-FOR-EMPTY-SLOT--------//
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                int newAmount;

                if (addAmount > item.itemsInStack)
                {
                    newAmount = item.itemsInStack;
                    addAmount -= item.itemsInStack;
                }
                else
                {
                    newAmount = addAmount;
                    addAmount = 0;
                }

                SpawnNewItem(item, inventorySlots[i], newAmount);

                if (addAmount <= 0)
                    return 0;
            }
        }

        return addAmount;
    }

    private void SpawnNewItem(Item item, InventorySlot slot, int itemAmount)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform.position, Quaternion.identity, slot.transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.count = itemAmount;
        inventoryItem.RefreshCount();
    }

    public int UseItem(Item item, int useAmount)
    {
        int previousAmount = useAmount;
        int currentAmount;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null && itemInSlot.item == item)
            {
                currentAmount = itemInSlot.count - useAmount;

                if (currentAmount > 0)
                {
                    itemInSlot.count = currentAmount;
                    itemInSlot.RefreshCount();
                    return previousAmount;
                }
                else
                {
                    itemInSlot.count = 0;
                    Destroy(itemInSlot.gameObject);
                    useAmount = Mathf.Abs(currentAmount);
                }
            }
        }

        return previousAmount - useAmount;
    }

    public void DropFromSlot(int slotNumber)
    {
        InventoryItem itemInSlot = inventorySlots[slotNumber].GetComponentInChildren<InventoryItem>();

        if (itemInSlot == null) return;

        DropStack(itemInSlot);
    }

    public void DropStack(InventoryItem inventoryItem)
    {
        int dropAmount = inventoryItem.item.dropAmount;
        int itemAmount = inventoryItem.count;

        GameObject newItem = Instantiate(inventoryItem.item.itemPrefab[0], transform.position, Quaternion.identity);

        if (dropAmount > 1 && newItem.TryGetComponent<CollectableItem>(out CollectableItem collectableItem))
        {
            int actualDropAmount = Mathf.Min(dropAmount, itemAmount);
            collectableItem.SetItem(inventoryItem.item, actualDropAmount);
            itemAmount -= actualDropAmount;
        }
        else
        {
            itemAmount--;
        }

        if (itemAmount <= 0)
        {
            Destroy(inventoryItem.gameObject);
        }
        else
        {
            inventoryItem.count = itemAmount;
            inventoryItem.RefreshCount();
        }
    }

    public int GetItemCount(Item searchItem)
    {
        int countItem = 0;

        InventoryItem[] itemsInInventory = inventoryParent.GetComponentsInChildren<InventoryItem>();

        foreach (InventoryItem itemInInventory in itemsInInventory)
        {
            if (itemInInventory.item == searchItem)
            {
                countItem += itemInInventory.count;
            }
        }

        return countItem;
    }

    public Item GetSelectedItem(bool use, int selectedSlot)
    {
        InventoryItem itemInSlot = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();

        if (itemInSlot == null) return null;

        Item item = itemInSlot.item;

        if (use)
        {
            itemInSlot.count--;
            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
            }
            else
            {
                itemInSlot.RefreshCount();
            }
        }

        return item;
    }
}