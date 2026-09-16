using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private Vector2 input;
    private bool warnedMissingRigidbody;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnEnable()
    {
        EnsureInitialized();
        inputActions?.player.Enable();
    }

    private void OnDisable()
    {
        inputActions?.player.Disable();
        input = Vector2.zero;
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
        inputActions = null;
    }

    private void Update()
    {
        if (inputActions == null)
            EnsureInitialized();

        if (inputActions != null)
            input = inputActions.player.move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 movement = new Vector3(input.x, 0f, input.y);
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void EnsureInitialized()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null && !warnedMissingRigidbody)
        {
            Debug.LogError("PlayerMovement: Rigidbody manquant sur l'objet player.", this);
            warnedMissingRigidbody = true;
        }
    }
}
