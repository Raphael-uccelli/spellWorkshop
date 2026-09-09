using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}