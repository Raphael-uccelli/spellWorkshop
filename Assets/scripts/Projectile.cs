using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private bool isPiercing;

    public void Initialize(SpellData spellData)
    {
        speed = spellData.projectileSpeed;
        damage = spellData.damage;
        isPiercing = spellData.isPiercing;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            if (!isPiercing)
            {
                Destroy(gameObject);
            }
        }
    }
}