using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private bool isPiercing;
    private bool hasExplosion;
    private float explosionRadius;

    public void Initialize(SpellData spellData)
    {
        speed = spellData.projectileSpeed;
        damage = spellData.damage;
        isPiercing = spellData.isPiercing;
        hasExplosion = spellData.hasExplosion;
        explosionRadius = spellData.explosionRadius;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
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