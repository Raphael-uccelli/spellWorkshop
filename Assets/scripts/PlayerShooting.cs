using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpellData currentSpell;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.player.Enable();
        inputActions.player.fire.performed += OnFire;
    }

    void OnDisable()
    {
        inputActions.player.fire.performed -= OnFire;
        inputActions.player.Disable();
    }

    private void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Initialize(currentSpell);
    }
}