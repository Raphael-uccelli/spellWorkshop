using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private float minAimDistance = 0.5f;

    private Camera mainCamera;
    private Rigidbody rb;
    private Quaternion targetRotation;

    void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;
    }

    void Update()
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.magnitude > minAimDistance)
            {
                targetRotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MoveRotation(targetRotation);
    }
}