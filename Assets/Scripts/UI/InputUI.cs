using UnityEngine;

public class InputUI : MonoBehaviour
{
    [SerializeField] private GameObject ObjectPlacement;
    [SerializeField] private GameObject placeStartBtn;

    public void ActivatePlacementBtns(bool isActive)
    {
        ObjectPlacement.SetActive(isActive);
        placeStartBtn.SetActive(!isActive);
    }
}