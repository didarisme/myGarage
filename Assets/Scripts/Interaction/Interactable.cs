using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake()
    {
        gameObject.layer = 3;
    }

    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}