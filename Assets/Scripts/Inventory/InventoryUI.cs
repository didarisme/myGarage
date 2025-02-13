using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Color selectedColor, notSelectedColor;

    private Image[] slotImages;
    private int oldSlot;

    public void ActivateSlot(int activateSlot)
    {
        slotImages[activateSlot].color = selectedColor;

        if (activateSlot != oldSlot)
            slotImages[oldSlot].color = notSelectedColor;

        oldSlot = activateSlot;
    }

    public void InitializeSlots(InventorySlot[] slots)
    {
        slotImages = new Image[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slotImages[i] = slots[i].GetComponent<Image>();
        }
    }
}