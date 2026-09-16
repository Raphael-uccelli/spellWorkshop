using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private Vector2 input;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnEnable()
    {
        EnsureInitialized();
        if (inputActions == null)
        {
            Debug.LogError("PlayerMovement: impossible d'initialiser les InputActions.", this);
            return;
        }

        inputActions.player.Enable();
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.player.Disable();
        }
    }

    private void Update()
    {
        if (inputActions == null)
        {
            return;
        }

        input = inputActions.player.move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void EnsureInitialized()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("PlayerMovement: Rigidbody manquant sur l'objet player.", this);
            }
        }
    }
}