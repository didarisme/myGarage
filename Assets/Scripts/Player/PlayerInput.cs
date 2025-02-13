using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    private Player playerInput;

    private void Awake()
    {
        Instance = this;
        playerInput = new Player();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Enable();
    }

    public Player GetPlayerInput()
    {
        return playerInput;
    }
}