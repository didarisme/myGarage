using UnityEngine;

public class CheckingPlacement : MonoBehaviour
{
    private ObjectPlacement objectPlacement;

    private void Start()
    {
        objectPlacement = FindObjectOfType<ObjectPlacement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectPlacement.IsTriggering = true;
    }

    private void OnTriggerExit(Collider other)
    {
        objectPlacement.IsTriggering = false;
    }
}