using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private Vector2 input;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
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
        input = inputActions.player.move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}