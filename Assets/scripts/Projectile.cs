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

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            HandleWallHit(other);
            return;
        }

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

    private void HandleWallHit(Collider wall)
    {
        if (hasBounce && bounceCount < maxBounces)
        {
            Vector3 closestPoint = wall.ClosestPoint(transform.position);
            Vector3 normal = (transform.position - closestPoint).normalized;
            Vector3 reflectedDirection = Vector3.Reflect(transform.forward, normal);

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