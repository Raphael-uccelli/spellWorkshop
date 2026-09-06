using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.player.Enable();
    }

    private void OnDisable()
    {
        inputActions.player.Disable();
    }

    private void Update()
    {
        Vector2 input = inputActions.player.move.ReadValue<Vector2>();

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        transform.position += movement * speed * Time.deltaTime;
    }
}