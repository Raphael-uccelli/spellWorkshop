using UnityEngine;
using UnityEngine.InputSystem;

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
        EnsureInputActions();
        currentSpell = spell1;
    }

    void OnEnable()
    {
        EnsureInputActions();
        if (inputActions == null)
        {
            Debug.LogError("PlayerShooting: impossible d'initialiser les InputActions.", this);
            return;
        }

        inputActions.player.Enable();
        inputActions.player.fire.performed += OnFire;
        inputActions.player.SelectSpell1.performed += OnSelectSpell1;
        inputActions.player.SelectSpell2.performed += OnSelectSpell2;
    }

    void OnDisable()
    {
        if (inputActions == null)
        {
            return;
        }

        inputActions.player.fire.performed -= OnFire;
        inputActions.player.SelectSpell1.performed -= OnSelectSpell1;
        inputActions.player.SelectSpell2.performed -= OnSelectSpell2;
        inputActions.player.Disable();
    }

    private void SelectSpell(SpellData spell)
    {
        if (spell == null)
        {
            return;
        }

        currentSpell = spell;
        Debug.Log("Selected spell: " + spell.spellName);
    }

    private void OnSelectSpell1(InputAction.CallbackContext context)
    {
        SelectSpell(spell1);
    }

    private void OnSelectSpell2(InputAction.CallbackContext context)
    {
        SelectSpell(spell2);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (projectilePrefab == null || firePoint == null || currentSpell == null)
        {
            Debug.LogWarning("PlayerShooting: tir ignoré, référence manquante (projectile/firePoint/spell).", this);
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("PlayerShooting: le projectile instancié ne contient pas de composant Projectile.", projectileObject);
            return;
        }

        projectile.Initialize(currentSpell);
    }

    private void EnsureInputActions()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
        }
    }
}