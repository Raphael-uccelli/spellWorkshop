using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GhostBirdAvatarBuilder : MonoBehaviour
{
    private const string ModelRootName = "GhostBirdModel";

#if UNITY_EDITOR
    private const string MaterialFolderPath = "Assets/Materials/GhostBird";
#endif

    [SerializeField] private bool autoBuildOnStart = true;
    [SerializeField] private Transform modelRoot;

    [Header("Palette")]
    [SerializeField] private Color bodyColor = new Color(0.90f, 0.90f, 0.94f, 1f);
    [SerializeField] private Color lightColor = new Color(0.97f, 0.97f, 0.99f, 1f);
    [SerializeField] private Color darkColor = new Color(0.05f, 0.05f, 0.08f, 1f);
    [SerializeField] private Color shadowColor = new Color(0.23f, 0.20f, 0.30f, 1f);
    [SerializeField] private Color eyeColor = Color.white;

    [Header("Persistent Materials (URP)")]
    [SerializeField] private Material bodyMaterial;
    [SerializeField] private Material lightMaterial;
    [SerializeField] private Material darkMaterial;
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private Material eyeMaterial;

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
        RemoveLegacyModels();
        EnsureModelRoot();
        ClearModelRoot();
        if (modelRoot == null)
        {
            return;
        }

        var resolvedBodyMaterial = ResolveMaterial("GhostBird_Body", bodyColor, ref bodyMaterial);
        var resolvedLightMaterial = ResolveMaterial("GhostBird_Light", lightColor, ref lightMaterial);
        var resolvedDarkMaterial = ResolveMaterial("GhostBird_Dark", darkColor, ref darkMaterial);
        var resolvedShadowMaterial = ResolveMaterial("GhostBird_Shadow", shadowColor, ref shadowMaterial);
        var resolvedEyeMaterial = ResolveMaterial("GhostBird_Eye", eyeColor, ref eyeMaterial);
        if (resolvedBodyMaterial == null ||
            resolvedLightMaterial == null ||
            resolvedDarkMaterial == null ||
            resolvedShadowMaterial == null ||
            resolvedEyeMaterial == null)
        {
            Debug.LogError("GhostBirdAvatarBuilder: génération annulée car un matériau n'a pas pu être créé.", this);
            return;
        }

        BuildOrganicGhostBird(
            resolvedBodyMaterial,
            resolvedLightMaterial,
            resolvedDarkMaterial,
            resolvedShadowMaterial,
            resolvedEyeMaterial);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

        var turnAnimation = GetComponent<GhostBirdTurnAnimation>();
        if (turnAnimation == null)
        {
            turnAnimation = gameObject.AddComponent<GhostBirdTurnAnimation>();
        }

        turnAnimation.AssignModelRoot(modelRoot);
    }

    private void BuildOrganicGhostBird(
        Material bodyMat,
        Material lightMat,
        Material darkMat,
        Material shadowMat,
        Material eyeMat)
    {
        CreatePrimitivePart("BodyCore", PrimitiveType.Sphere, bodyMat,
            new Vector3(0f, -0.06f, 0f), new Vector3(0.86f, 0.98f, 0.80f));

        CreatePrimitivePart("BodyLower", PrimitiveType.Sphere, lightMat,
            new Vector3(0f, -0.42f, 0f), new Vector3(1.08f, 0.82f, 1.00f));

        CreatePrimitivePart("ChestHighlight", PrimitiveType.Sphere, lightMat,
            new Vector3(0.03f, -0.11f, 0.18f), new Vector3(0.58f, 0.54f, 0.36f));

        CreatePrimitivePart("ChestShadow", PrimitiveType.Sphere, shadowMat,
            new Vector3(-0.01f, -0.34f, -0.08f), new Vector3(0.70f, 0.24f, 0.66f));

        CreatePrimitivePart("Head", PrimitiveType.Sphere, darkMat,
            new Vector3(0f, 0.52f, 0.03f), new Vector3(0.68f, 0.62f, 0.62f));

        CreateConePart("HeadTip", darkMat,
            new Vector3(0f, 0.95f, -0.05f), new Vector3(0.20f, 0.24f, 0.20f),
            Quaternion.identity);

        CreateConePart("Beak", darkMat,
            new Vector3(0f, 0.45f, 0.54f), new Vector3(0.10f, 0.72f, 0.10f),
            Quaternion.Euler(90f, 0f, 0f));

        CreatePrimitivePart("BeakBase", PrimitiveType.Capsule, darkMat,
            new Vector3(0f, 0.45f, 0.25f), new Vector3(0.15f, 0.12f, 0.15f),
            Quaternion.Euler(90f, 0f, 0f));

        CreatePrimitivePart("LeftEye", PrimitiveType.Sphere, eyeMat,
            new Vector3(-0.14f, 0.56f, 0.23f), new Vector3(0.16f, 0.22f, 0.11f));

        CreatePrimitivePart("RightEye", PrimitiveType.Sphere, eyeMat,
            new Vector3(0.14f, 0.56f, 0.23f), new Vector3(0.16f, 0.22f, 0.11f));

        CreatePrimitivePart("LeftWing", PrimitiveType.Capsule, lightMat,
            new Vector3(-0.53f, -0.07f, -0.05f), new Vector3(0.22f, 0.52f, 0.22f),
            Quaternion.Euler(0f, 0f, 35f));

        CreatePrimitivePart("RightWing", PrimitiveType.Capsule, lightMat,
            new Vector3(0.53f, -0.07f, -0.05f), new Vector3(0.22f, 0.52f, 0.22f),
            Quaternion.Euler(0f, 0f, -35f));

        CreateConePart("TendrilLeft", bodyMat,
            new Vector3(-0.32f, -0.74f, 0.08f), new Vector3(0.13f, 0.30f, 0.13f),
            Quaternion.Euler(198f, 0f, 16f));

        CreateConePart("TendrilMidLeft", bodyMat,
            new Vector3(-0.10f, -0.82f, 0.14f), new Vector3(0.12f, 0.34f, 0.12f),
            Quaternion.Euler(188f, 0f, 3f));

        CreateConePart("TendrilMidRight", bodyMat,
            new Vector3(0.10f, -0.81f, 0.02f), new Vector3(0.12f, 0.36f, 0.12f),
            Quaternion.Euler(184f, 0f, -4f));

        CreateConePart("TendrilRight", bodyMat,
            new Vector3(0.34f, -0.73f, -0.08f), new Vector3(0.13f, 0.30f, 0.13f),
            Quaternion.Euler(196f, 0f, -17f));
    }

    private void RemoveLegacyModels()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child == modelRoot)
            {
                continue;
            }

            if (child.name == "GhostModel")
            {
                DestroyObject(child.gameObject);
            }
        }
    }

    private void EnsureModelRoot()
    {
        if (modelRoot != null && modelRoot.parent != transform)
        {
            modelRoot = null;
        }

        Transform firstMatchingRoot = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name != ModelRootName)
            {
                continue;
            }

            if (firstMatchingRoot == null)
            {
                firstMatchingRoot = child;
                continue;
            }

            DestroyObject(child.gameObject);
        }

        modelRoot = modelRoot != null ? modelRoot : firstMatchingRoot;

        if (modelRoot != null)
        {
            return;
        }

        var root = new GameObject(ModelRootName);
        modelRoot = root.transform;
        modelRoot.SetParent(transform, false);
    }

    private void ClearModelRoot()
    {
        if (modelRoot == null)
        {
            Debug.LogError("GhostBirdAvatarBuilder: modelRoot est introuvable, impossible de reconstruire le modèle.", this);
            return;
        }

        for (int i = modelRoot.childCount - 1; i >= 0; i--)
        {
            DestroyObject(modelRoot.GetChild(i).gameObject);
        }

        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.identity;
        modelRoot.localScale = Vector3.one;
    }

    private GameObject CreatePrimitivePart(
        string name,
        PrimitiveType primitiveType,
        Material material,
        Vector3 localPosition,
        Vector3 localScale,
        Quaternion localRotation = default)
    {
        var go = GameObject.CreatePrimitive(primitiveType);
        go.name = name;
        go.transform.SetParent(modelRoot, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        go.transform.localRotation = localRotation == default ? Quaternion.identity : localRotation;

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        RemoveCollider(go);
        return go;
    }

    private GameObject CreateConePart(
        string name,
        Material material,
        Vector3 localPosition,
        Vector3 localScale,
        Quaternion localRotation)
    {
        var go = new GameObject(name);
        go.transform.SetParent(modelRoot, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;
        go.transform.localScale = localScale;

        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = CreateConeMesh(name + "Mesh", 18);

        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        return go;
    }

    private static Mesh CreateConeMesh(string meshName, int segments)
    {
        var mesh = new Mesh { name = meshName };

        var vertices = new Vector3[segments + 2];
        vertices[0] = new Vector3(0f, 0.5f, 0f); // Tip
        vertices[1] = new Vector3(0f, -0.5f, 0f); // Base center

        for (int i = 0; i < segments; i++)
        {
            float angle = (Mathf.PI * 2f * i) / segments;
            vertices[i + 2] = new Vector3(Mathf.Cos(angle) * 0.5f, -0.5f, Mathf.Sin(angle) * 0.5f);
        }

        var triangles = new int[segments * 6];
        int index = 0;

        for (int i = 0; i < segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % segments + 2;

            triangles[index++] = 0;
            triangles[index++] = current;
            triangles[index++] = next;

            triangles[index++] = 1;
            triangles[index++] = next;
            triangles[index++] = current;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private Material ResolveMaterial(string materialName, Color color, ref Material materialSlot)
    {
        var shader = GetCompatibleShader();
        if (shader == null)
        {
            Debug.LogError("GhostBirdAvatarBuilder: shader compatible introuvable. Vérifiez la configuration URP.", this);
            return null;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            materialSlot = GetOrCreateMaterialAsset(materialName, shader, color);
            return materialSlot;
        }
#endif

        if (materialSlot == null)
        {
            materialSlot = new Material(shader);
        }
        else if (materialSlot.shader != shader)
        {
            materialSlot.shader = shader;
        }

        ApplyMaterialColor(materialSlot, color);
        return materialSlot;
    }

    private static Shader GetCompatibleShader()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null)
        {
            return shader;
        }

        shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader != null)
        {
            return shader;
        }

        shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            return shader;
        }

        return Shader.Find("Standard");
    }

    private static void ApplyMaterialColor(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.08f);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 0f);
        }
    }

#if UNITY_EDITOR
    private static Material GetOrCreateMaterialAsset(string materialName, Shader shader, Color color)
    {
        EnsureMaterialFolderExists();
        var assetPath = $"{MaterialFolderPath}/{materialName}.mat";

        var materialAsset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (materialAsset == null)
        {
            materialAsset = new Material(shader);
            AssetDatabase.CreateAsset(materialAsset, assetPath);
        }

        if (materialAsset.shader != shader)
        {
            materialAsset.shader = shader;
        }

        ApplyMaterialColor(materialAsset, color);
        EditorUtility.SetDirty(materialAsset);
        return materialAsset;
    }

    private static void EnsureMaterialFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        if (!AssetDatabase.IsValidFolder(MaterialFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "GhostBird");
        }
    }
#endif

    private static void RemoveCollider(GameObject go)
    {
        var collider = go.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyObject(collider);
        }
    }

    private static void DestroyObject(Object target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }
}
