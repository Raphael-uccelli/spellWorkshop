using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpellData spell1;
    [SerializeField] private SpellData spell2;

    private SpellData currentSpell;
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        currentSpell = spell1;
    }

    void OnEnable()
    {
        inputActions.player.Enable();
        inputActions.player.fire.performed += OnFire;
        inputActions.player.SelectSpell1.performed += ctx => SelectSpell(spell1);
        inputActions.player.SelectSpell2.performed += ctx => SelectSpell(spell2);
    }

    void OnDisable()
    {
        inputActions.player.fire.performed -= OnFire;
        inputActions.player.Disable();
    }

    private void SelectSpell(SpellData spell)
    {
        currentSpell = spell;
        Debug.Log("Selected spell: " + spell.spellName);
    }

    private void OnFire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Initialize(currentSpell);
    }
}