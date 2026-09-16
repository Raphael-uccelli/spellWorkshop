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

    private void Awake()
    {
        EnsureInputActions();
        currentSpell = spell1 != null ? spell1 : spell2;
    }

    private void OnEnable()
    {
        EnsureInputActions();
        if (inputActions == null)
            return;

        inputActions.player.Enable();
        inputActions.player.fire.performed += OnFire;
        inputActions.player.SelectSpell1.performed += OnSelectSpell1;
        inputActions.player.SelectSpell2.performed += OnSelectSpell2;
    }

    private void OnDisable()
    {
        if (inputActions == null)
            return;

        inputActions.player.fire.performed -= OnFire;
        inputActions.player.SelectSpell1.performed -= OnSelectSpell1;
        inputActions.player.SelectSpell2.performed -= OnSelectSpell2;
        inputActions.player.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
        inputActions = null;
    }

    private void OnSelectSpell1(InputAction.CallbackContext context)
    {
        SelectSpell(spell1);
    }

    private void OnSelectSpell2(InputAction.CallbackContext context)
    {
        SelectSpell(spell2);
    }

    private void SelectSpell(SpellData spell)
    {
        if (spell == null)
        {
            Debug.LogWarning("PlayerShooting: le sort sélectionné n'est pas assigné.", this);
            return;
        }

        currentSpell = spell;
        Debug.Log("Selected spell: " + spell.spellName);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerShooting: projectilePrefab n'est pas assigné.", this);
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("PlayerShooting: FirePoint n'est pas assigné.", this);
            return;
        }

        if (currentSpell == null)
        {
            Debug.LogWarning("PlayerShooting: aucun sort n'est assigné.", this);
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (projectileObject == null)
            return;

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogError("PlayerShooting: le prefab projectile ne contient pas le composant Projectile.", projectileObject);
            Destroy(projectileObject);
            return;
        }

        projectile.Initialize(currentSpell);
    }

    private void EnsureInputActions()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }
}
