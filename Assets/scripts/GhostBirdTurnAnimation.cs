using UnityEngine;

public class GhostBirdTurnAnimation : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private float hoverHeight = 0.16f;
    [SerializeField] private float hoverSpeed = 2.5f;
    [SerializeField] private float lateralSwing = 10f;

    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;

    private void Awake()
    {
        if (modelRoot == null)
        {
            modelRoot = transform.Find("GhostBirdModel");
        }

        if (modelRoot == null)
        {
            modelRoot = transform;
        }

        baseLocalPosition = modelRoot.localPosition;
        baseLocalRotation = modelRoot.localRotation;
    }

    private void LateUpdate()
    {
        var bob = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        modelRoot.localPosition = baseLocalPosition + Vector3.up * bob;

        var tilt = Mathf.Sin(Time.time * (hoverSpeed * 1.2f)) * lateralSwing;
        modelRoot.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, tilt);
    }
}
