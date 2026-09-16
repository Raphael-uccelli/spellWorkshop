using UnityEngine;

[ExecuteAlways]
public class GhostBirdAvatarBuilder : MonoBehaviour
{
    [SerializeField] private bool autoBuildOnStart = true;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Color bodyColor = new Color(0.90f, 0.90f, 0.93f, 1f);
    [SerializeField] private Color lightColor = new Color(0.96f, 0.96f, 0.98f, 1f);
    [SerializeField] private Color darkColor = new Color(0.16f, 0.18f, 0.22f, 1f);
    [SerializeField] private Color eyeColor = new Color(1f, 1f, 1f, 1f);

    private void Reset()
    {
        BuildAvatar();
    }

    private void Awake()
    {
        if (autoBuildOnStart)
        {
            BuildAvatar();
        }
    }

    [ContextMenu("Build Avatar")]
    public void BuildAvatar()
    {
        if (modelRoot == null)
        {
            var existing = transform.Find("GhostBirdModel");
            if (existing != null)
            {
                modelRoot = existing;
            }
            else
            {
                var go = new GameObject("GhostBirdModel");
                modelRoot = go.transform;
                modelRoot.SetParent(transform, false);
            }
        }

        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.identity;
        modelRoot.localScale = Vector3.one;

        for (int i = modelRoot.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(modelRoot.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(modelRoot.GetChild(i).gameObject);
            }
        }

        if (GetComponent<GhostBirdTurnAnimation>() == null)
        {
            gameObject.AddComponent<GhostBirdTurnAnimation>();
        }

        var bodyMaterial = CreateMaterial(bodyColor);
        var lightMaterial = CreateMaterial(lightColor);
        var darkMaterial = CreateMaterial(darkColor);
        var eyeMaterial = CreateMaterial(eyeColor);

        var torso = CreatePrimitive("Body", PrimitiveType.Sphere, new Vector3(0.82f, 0.72f, 0.72f), new Vector3(0f, 0.2f, 0f), Quaternion.identity, bodyMaterial);
        torso.transform.SetParent(modelRoot, false);

        var chest = CreatePrimitive("Chest", PrimitiveType.Sphere, new Vector3(0.52f, 0.52f, 0.54f), new Vector3(0.18f, 0.08f, 0f), Quaternion.identity, lightMaterial);
        chest.transform.SetParent(modelRoot, false);

        var head = CreatePrimitive("Head", PrimitiveType.Sphere, new Vector3(0.54f, 0.58f, 0.5f), new Vector3(0.58f, 0.36f, 0f), Quaternion.identity, darkMaterial);
        head.transform.SetParent(modelRoot, false);

        var hood = CreatePrimitive("Hood", PrimitiveType.Cube, new Vector3(0.48f, 0.22f, 0.44f), new Vector3(0.42f, 0.62f, 0f), Quaternion.Euler(0f, 0f, 12f), darkMaterial);
        hood.transform.SetParent(modelRoot, false);

        var beak = CreatePrimitive("Beak", PrimitiveType.Cube, new Vector3(0.5f, 0.16f, 0.16f), new Vector3(1.02f, 0.25f, 0f), Quaternion.Euler(0f, 0f, -13f), darkMaterial);
        beak.transform.SetParent(modelRoot, false);

        var leftEye = CreatePrimitive("LeftEye", PrimitiveType.Sphere, new Vector3(0.13f, 0.18f, 0.12f), new Vector3(0.69f, 0.48f, 0.17f), Quaternion.identity, eyeMaterial);
        leftEye.transform.SetParent(modelRoot, false);

        var rightEye = CreatePrimitive("RightEye", PrimitiveType.Sphere, new Vector3(0.13f, 0.18f, 0.12f), new Vector3(0.69f, 0.48f, -0.17f), Quaternion.identity, eyeMaterial);
        rightEye.transform.SetParent(modelRoot, false);

        var leftWing = CreatePrimitive("LeftWing", PrimitiveType.Cube, new Vector3(0.7f, 0.16f, 0.42f), new Vector3(-0.42f, 0.1f, 0.38f), Quaternion.Euler(0f, 0f, 25f), lightMaterial);
        leftWing.transform.SetParent(modelRoot, false);

        var rightWing = CreatePrimitive("RightWing", PrimitiveType.Cube, new Vector3(0.7f, 0.16f, 0.42f), new Vector3(-0.42f, 0.1f, -0.38f), Quaternion.Euler(0f, 0f, -25f), lightMaterial);
        rightWing.transform.SetParent(modelRoot, false);

        var tailLeft = CreatePrimitive("TailLeft", PrimitiveType.Cube, new Vector3(0.52f, 0.14f, 0.2f), new Vector3(-0.82f, 0.05f, 0.2f), Quaternion.Euler(0f, 0f, 42f), lightMaterial);
        tailLeft.transform.SetParent(modelRoot, false);

        var tailRight = CreatePrimitive("TailRight", PrimitiveType.Cube, new Vector3(0.52f, 0.14f, 0.2f), new Vector3(-0.82f, 0.05f, -0.2f), Quaternion.Euler(0f, 0f, -42f), lightMaterial);
        tailRight.transform.SetParent(modelRoot, false);

        var tailCenter = CreatePrimitive("TailCenter", PrimitiveType.Cube, new Vector3(0.42f, 0.12f, 0.18f), new Vector3(-0.96f, -0.08f, 0f), Quaternion.Euler(0f, 0f, 90f), lightMaterial);
        tailCenter.transform.SetParent(modelRoot, false);

        var underside = CreatePrimitive("Underside", PrimitiveType.Capsule, new Vector3(0.56f, 0.28f, 0.5f), new Vector3(-0.08f, -0.12f, 0f), Quaternion.Euler(90f, 0f, 0f), lightMaterial);
        underside.transform.SetParent(modelRoot, false);
    }

    private static GameObject CreatePrimitive(string name, PrimitiveType type, Vector3 localScale, Vector3 localPosition, Quaternion localRotation, Material material)
    {
        var primitive = GameObject.CreatePrimitive(type);
        primitive.name = name;
        primitive.transform.localScale = localScale;
        primitive.transform.localPosition = localPosition;
        primitive.transform.localRotation = localRotation;

        var renderer = primitive.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        var collider = primitive.GetComponent<Collider>();
        if (collider != null)
        {
            if (Application.isPlaying)
            {
                Destroy(collider);
            }
            else
            {
                DestroyImmediate(collider);
            }
        }

        return primitive;
    }

    private static Material CreateMaterial(Color color)
    {
        var shader = Shader.Find("Standard");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        var material = new Material(shader);
        material.color = color;
        material.SetFloat("_Glossiness", 0.12f);
        return material;
    }
}
