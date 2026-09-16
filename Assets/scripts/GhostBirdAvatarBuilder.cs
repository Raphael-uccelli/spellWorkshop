using System.Collections.Generic;
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
    [SerializeField] private Color bodyColor = new Color(0.92f, 0.93f, 0.97f, 1f);
    [SerializeField] private Color lightColor = new Color(0.98f, 0.98f, 1f, 1f);
    [SerializeField] private Color darkColor = new Color(0.04f, 0.04f, 0.07f, 1f);
    [SerializeField] private Color shadowColor = new Color(0.22f, 0.21f, 0.28f, 1f);
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
            Debug.LogError("GhostBirdAvatarBuilder: génération annulée car aucun shader URP compatible n'a été trouvé.", this);
            return;
        }

        ClearModelRoot();
        BuildStylizedGhostBird(
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

        ValidateForwardOrientation();

        var turnAnimation = GetComponent<GhostBirdTurnAnimation>();
        if (turnAnimation == null)
        {
            turnAnimation = gameObject.AddComponent<GhostBirdTurnAnimation>();
        }

        turnAnimation.AssignModelRoot(modelRoot);
    }

    private void BuildStylizedGhostBird(
        Material bodyMat,
        Material lightMat,
        Material darkMat,
        Material shadowMat,
        Material eyeMat)
    {
        var bodyOutline = new[]
        {
            new Vector2(-0.44f, 0.40f),
            new Vector2(-0.32f, 0.62f),
            new Vector2(0f, 0.70f),
            new Vector2(0.34f, 0.60f),
            new Vector2(0.46f, 0.38f),
            new Vector2(0.47f, 0.04f),
            new Vector2(0.38f, -0.26f),
            new Vector2(0.24f, -0.52f),
            new Vector2(0.08f, -0.80f),
            new Vector2(-0.06f, -0.57f),
            new Vector2(-0.20f, -0.84f),
            new Vector2(-0.34f, -0.48f),
            new Vector2(-0.44f, -0.18f),
        };

        var bodyHighlight = new[]
        {
            new Vector2(-0.28f, 0.39f),
            new Vector2(-0.20f, 0.52f),
            new Vector2(0f, 0.57f),
            new Vector2(0.22f, 0.50f),
            new Vector2(0.30f, 0.36f),
            new Vector2(0.29f, 0.05f),
            new Vector2(0.23f, -0.16f),
            new Vector2(0.12f, -0.38f),
            new Vector2(0f, -0.56f),
            new Vector2(-0.11f, -0.35f),
            new Vector2(-0.22f, -0.56f),
            new Vector2(-0.30f, -0.22f),
        };

        CreateMeshPart("Body", CreateExtrudedPolygonMesh("GhostBirdBody", bodyOutline, 0.24f), bodyMat,
            new Vector3(0f, -0.12f, -0.02f));

        CreateMeshPart("BodyHighlight", CreateExtrudedPolygonMesh("GhostBirdBodyHighlight", bodyHighlight, 0.12f), lightMat,
            new Vector3(0f, -0.10f, 0.10f));

        CreateMeshPart("BodyShadow", CreateExtrudedPolygonMesh("GhostBirdBodyShadow", bodyHighlight, 0.11f), shadowMat,
            new Vector3(-0.02f, -0.28f, -0.08f), Quaternion.Euler(0f, 10f, 0f), new Vector3(1.06f, 0.86f, 1f));

        CreateMeshPart("Head", CreateExtrudedEllipseMesh("GhostBirdHead", 0.34f, 0.36f, 0.30f, 24), darkMat,
            new Vector3(0f, 0.54f, 0.02f));

        var headTop = new[]
        {
            new Vector2(-0.12f, 0.10f),
            new Vector2(0f, 0.24f),
            new Vector2(0.12f, 0.10f),
            new Vector2(0f, -0.02f),
        };

        CreateMeshPart("HeadTop", CreateExtrudedPolygonMesh("GhostBirdHeadTop", headTop, 0.16f), darkMat,
            new Vector3(0f, 0.86f, 0.01f));

        CreateMeshPart("Beak", CreateBeakMesh("GhostBirdBeak", 0.76f, 0.13f, 0.10f), darkMat,
            new Vector3(0f, 0.46f, 0.22f));

        CreateMeshPart("LeftEye", CreateExtrudedEllipseMesh("GhostBirdLeftEye", 0.06f, 0.11f, 0.05f, 16), eyeMat,
            new Vector3(-0.11f, 0.56f, 0.30f), Quaternion.Euler(4f, -5f, 4f));

        CreateMeshPart("RightEye", CreateExtrudedEllipseMesh("GhostBirdRightEye", 0.06f, 0.11f, 0.05f, 16), eyeMat,
            new Vector3(0.11f, 0.56f, 0.30f), Quaternion.Euler(4f, 5f, -4f));

        CreateMeshPart("TendrilLeft", CreateTendrilMesh("GhostBirdTendrilLeft", 0.27f, 0.16f, 0.11f), bodyMat,
            new Vector3(-0.26f, -0.90f, 0.03f), Quaternion.Euler(8f, -10f, 22f));

        CreateMeshPart("TendrilMid", CreateTendrilMesh("GhostBirdTendrilMid", 0.34f, 0.18f, 0.12f), bodyMat,
            new Vector3(0f, -0.95f, 0.06f), Quaternion.Euler(0f, 4f, -2f));

        CreateMeshPart("TendrilRight", CreateTendrilMesh("GhostBirdTendrilRight", 0.25f, 0.14f, 0.11f), bodyMat,
            new Vector3(0.24f, -0.90f, -0.02f), Quaternion.Euler(-6f, 12f, -20f));
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

    private void ValidateForwardOrientation()
    {
        if (modelRoot == null)
        {
            return;
        }

        var firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            return;
        }

        var localFirePoint = transform.InverseTransformPoint(firePoint.position);
        if (localFirePoint.z < 0.02f)
        {
            Debug.LogWarning("GhostBirdAvatarBuilder: FirePoint semble derrière le bec. Placez-le devant le joueur sur +Z.", firePoint);
        }
    }

    private GameObject CreateMeshPart(
        string name,
        Mesh mesh,
        Material material,
        Vector3 localPosition,
        Quaternion localRotation = default,
        Vector3 localScale = default)
    {
        var go = new GameObject(name);
        go.transform.SetParent(modelRoot, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation == default ? Quaternion.identity : localRotation;
        go.transform.localScale = localScale == default ? Vector3.one : localScale;

        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        return go;
    }

    private static Mesh CreateExtrudedEllipseMesh(string meshName, float radiusX, float radiusY, float depth, int segments)
    {
        var points = new List<Vector2>(segments);
        for (int i = 0; i < segments; i++)
        {
            float angle = (Mathf.PI * 2f * i) / segments;
            points.Add(new Vector2(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY));
        }

        return CreateExtrudedPolygonMesh(meshName, points.ToArray(), depth);
    }

    private static Mesh CreateTendrilMesh(string meshName, float length, float width, float depth)
    {
        var points = new[]
        {
            new Vector2(0f, -length),
            new Vector2(-width * 0.5f, 0f),
            new Vector2(width * 0.5f, 0f),
        };

        return CreateExtrudedPolygonMesh(meshName, points, depth);
    }

    private static Mesh CreateBeakMesh(string meshName, float length, float width, float height)
    {
        var mesh = new Mesh { name = meshName };

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        var vertices = new[]
        {
            new Vector3(0f, halfHeight, 0f),
            new Vector3(-halfWidth, -halfHeight, 0f),
            new Vector3(halfWidth, -halfHeight, 0f),
            new Vector3(0f, halfHeight, length),
            new Vector3(-halfWidth * 0.15f, -halfHeight * 0.4f, length),
            new Vector3(halfWidth * 0.15f, -halfHeight * 0.4f, length),
        };

        var triangles = new[]
        {
            0, 1, 2,
            3, 5, 4,
            0, 2, 5,
            0, 5, 3,
            0, 3, 4,
            0, 4, 1,
            1, 4, 5,
            1, 5, 2,
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateExtrudedPolygonMesh(string meshName, Vector2[] points, float depth)
    {
        var mesh = new Mesh { name = meshName };
        if (points == null || points.Length < 3)
        {
            return mesh;
        }

        int count = points.Length;
        float frontZ = depth * 0.5f;
        float backZ = -frontZ;

        var vertices = new Vector3[count * 2];
        for (int i = 0; i < count; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, frontZ);
            vertices[i + count] = new Vector3(points[i].x, points[i].y, backZ);
        }

        var triangles = new List<int>((count - 2) * 6 + count * 6);

        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        for (int i = 1; i < count - 1; i++)
        {
            triangles.Add(count);
            triangles.Add(count + i + 1);
            triangles.Add(count + i);
        }

        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            int frontA = i;
            int frontB = next;
            int backA = i + count;
            int backB = next + count;

            triangles.Add(frontA);
            triangles.Add(frontB);
            triangles.Add(backB);

            triangles.Add(frontA);
            triangles.Add(backB);
            triangles.Add(backA);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private Material ResolveMaterial(string materialName, Color color, ref Material materialSlot)
    {
        var shader = GetCompatibleShader();
        if (shader == null)
        {
            Debug.LogError("GhostBirdAvatarBuilder: shader URP introuvable. Vérifiez Universal Render Pipeline.", this);
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
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader != null)
        {
            return shader;
        }

        return Shader.Find("Universal Render Pipeline/Lit");
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
            material.SetFloat("_Smoothness", 0f);
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
