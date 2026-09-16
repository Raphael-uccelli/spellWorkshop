using UnityEngine;

public class GhostBirdTurnAnimation : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private float hoverHeight = 0.16f;
    [SerializeField] private float hoverSpeed = 2.5f;
    [SerializeField] private float lateralSwing = 10f;

    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private Transform cachedRoot;
    private bool warnedMissingRoot;

    private void Awake()
    {
        TryBindModelRoot(logWarning: true);
    }

    private void OnEnable()
    {
        TryBindModelRoot(logWarning: false);
    }

    public void AssignModelRoot(Transform root)
    {
        modelRoot = root;
        CachePose();
        warnedMissingRoot = false;
    }

    private void LateUpdate()
    {
        if (!TryBindModelRoot(logWarning: true))
        {
            return;
        }

        float bob = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        modelRoot.localPosition = baseLocalPosition + Vector3.up * bob;

        float tilt = Mathf.Sin(Time.time * (hoverSpeed * 1.2f)) * lateralSwing;
        modelRoot.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, tilt);
    }

    private bool TryBindModelRoot(bool logWarning)
    {
        if (modelRoot == null)
        {
            modelRoot = transform.Find("GhostBirdModel");
        }

        if (modelRoot == null)
        {
            if (logWarning && !warnedMissingRoot)
            {
                Debug.LogWarning("GhostBirdTurnAnimation: aucun enfant 'GhostBirdModel' trouvé. Le flottement est ignoré jusqu'à reconstruction du modèle.", this);
                warnedMissingRoot = true;
            }

            return false;
        }

        warnedMissingRoot = false;

        if (cachedRoot != modelRoot)
        {
            CachePose();
        }

        return true;
    }

    private void CachePose()
    {
        if (modelRoot == null)
        {
            return;
        }

        cachedRoot = modelRoot;
        baseLocalPosition = modelRoot.localPosition;
        baseLocalRotation = modelRoot.localRotation;
    }
}
