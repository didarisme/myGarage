using UnityEngine;
using UnityEngine.EventSystems;

public class DropBack : MonoBehaviour, IDropHandler
{
    private InventoryManager inventory;

    private void Awake()
    {
        inventory = FindAnyObjectByType<InventoryManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        inventory.DropStack(eventData.pointerDrag.GetComponent<InventoryItem>());
    }
}