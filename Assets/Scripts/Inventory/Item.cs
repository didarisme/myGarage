using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("Gameplay")]
    public string itemName;
    public int itemsInStack = 1;
    public int dropAmount;
    public ActionType actionType;
    public GameObject[] itemPrefab;

    [Header("UI")]
    public Sprite image;
    public string description;
}

public enum ActionType
{
    PlaceableObject,
    Usable,
    Nothing
}