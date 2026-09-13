using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private bool isPiercing;
    private bool hasExplosion;
    private float explosionRadius;
    private bool hasBounce;
    private int maxBounces;
    private int bounceCount = 0;

    private Rigidbody rb;

    [SerializeField] private LayerMask wallLayer;

    public void Initialize(SpellData spellData)
    {
        speed = spellData.projectileSpeed;
        damage = spellData.damage;
        isPiercing = spellData.isPiercing;
        hasExplosion = spellData.hasExplosion;
        explosionRadius = spellData.explosionRadius;
        hasBounce = spellData.hasBounce;
        maxBounces = spellData.maxBounces;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveDistance = speed * Time.fixedDeltaTime;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(rb.position, direction, out RaycastHit hit, moveDistance, wallLayer))
        {
            HandleWallHit(hit);
            return;
        }

        rb.MovePosition(rb.position + direction * moveDistance);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (hasExplosion)
            {
                Explode();
            }
            else
            {
                DamageSingleTarget(other);
            }

            if (!isPiercing)
            {
                Destroy(gameObject);
            }
        }
    }

    private void HandleWallHit(RaycastHit hit)
    {
        if (hasBounce && bounceCount < maxBounces)
        {
            Vector3 reflectedDirection = Vector3.Reflect(transform.forward, hit.normal);
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(reflectedDirection);
            bounceCount++;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DamageSingleTarget(Collider target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                DamageSingleTarget(hitCollider);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hasExplosion)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}