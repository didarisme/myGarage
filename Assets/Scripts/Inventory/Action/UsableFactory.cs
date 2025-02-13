using UnityEngine;

public class UsableFactory : MonoBehaviour
{
    private static InventoryManager inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    public IUsable CreateUsable(Item item)
    {
        switch (item.name)
        {
            //case "Medkit":
                //return new HealthIncreaseAction(inventory, playerStats, 50f);
            default:
                Debug.LogWarning($"No usable action defined for item: {item.name}");
                return null;
        }
    }
}