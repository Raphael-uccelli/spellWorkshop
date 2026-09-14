using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float bodyRadius = 0.5f;
    [SerializeField] private LayerMask wallLayer;

    private Transform target;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, rb.position.y, target.position.z);
        Vector3 direction = (targetPosition - rb.position).normalized;
        float moveDistance = moveSpeed * Time.fixedDeltaTime;

        if (Physics.SphereCast(rb.position, bodyRadius, direction, out RaycastHit hit, moveDistance, wallLayer))
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
            rb.MovePosition(rb.position + slideDirection * moveDistance);
        }
        else
        {
            rb.MovePosition(rb.position + direction * moveDistance);
        }
    }
}