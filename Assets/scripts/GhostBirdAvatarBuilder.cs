using UnityEngine;

[ExecuteAlways]
public class GhostBirdAvatarBuilder : MonoBehaviour
{
    [SerializeField] private bool autoBuildOnStart = true;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Color bodyColor = new Color(0.91f, 0.90f, 0.92f, 1f);
    [SerializeField] private Color lightColor = new Color(0.98f, 0.97f, 0.98f, 1f);
    [SerializeField] private Color darkColor = new Color(0.043f, 0.043f, 0.059f, 1f);
    [SerializeField] private Color shadowColor = new Color(0.18f, 0.16f, 0.23f, 1f);
    [SerializeField] private Color eyeColor = Color.white;

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

        var bodyMaterial = CreateMaterial(bodyColor);
        var lightMaterial = CreateMaterial(lightColor);
        var darkMaterial = CreateMaterial(darkColor);
        var shadowMaterial = CreateMaterial(shadowColor);
        var eyeMaterial = CreateMaterial(eyeColor);

        // Corps blanc flottant, avec pointes irrégulières comme sur la référence.
        CreateExtrudedPolygon("Body", new[]
        {
            new Vector2(-0.60f, 0.50f), new Vector2(-0.36f, 0.76f),
            new Vector2(-0.04f, 0.72f), new Vector2(0.20f, 0.46f),
            new Vector2(0.12f, 0.06f), new Vector2(-0.02f, -0.26f),
            new Vector2(-0.25f, -0.62f), new Vector2(-0.39f, -0.38f),
            new Vector2(-0.58f, -0.72f), new Vector2(-0.53f, -0.20f),
            new Vector2(-0.78f, -0.42f), new Vector2(-0.72f, 0.02f)
        }, 0.48f, bodyMaterial);

        CreateExtrudedPolygon("Chest", new[]
        {
            new Vector2(-0.42f, 0.39f), new Vector2(-0.15f, 0.54f),
            new Vector2(0.15f, 0.33f), new Vector2(0.07f, 0.00f),
            new Vector2(-0.13f, -0.35f), new Vector2(-0.31f, -0.17f),
            new Vector2(-0.53f, -0.29f), new Vector2(-0.57f, 0.02f)
        }, 0.505f, lightMaterial, new Vector3(-0.015f, -0.015f, 0f));

        // Capuche noire compacte et pointue.
        CreateExtrudedPolygon("Head", new[]
        {
            new Vector2(0.02f, 0.28f), new Vector2(0.13f, 0.69f),
            new Vector2(0.43f, 0.91f), new Vector2(0.67f, 0.82f),
            new Vector2(0.55f, 0.57f), new Vector2(0.65f, 0.24f),
            new Vector2(0.43f, -0.02f), new Vector2(0.14f, 0.00f)
        }, 0.54f, darkMaterial, new Vector3(0.08f, 0.22f, 0f));

        // Bec long, fin et légèrement descendant.
        CreateExtrudedPolygon("Beak", new[]
        {
            new Vector2(0.48f, 0.42f), new Vector2(1.34f, 0.28f),
            new Vector2(0.91f, 0.08f), new Vector2(0.54f, 0.14f)
        }, 0.28f, darkMaterial, new Vector3(0.10f, 0.21f, 0f));

        // Deux mèches latérales blanches, plus fines que les anciennes ailes.
        CreateExtrudedPolygon("LeftTendril", new[]
        {
            new Vector2(-0.28f, 0.35f), new Vector2(-0.75f, 0.22f),
            new Vector2(-1.00f, -0.10f), new Vector2(-0.67f, -0.01f),
            new Vector2(-0.82f, -0.35f), new Vector2(-0.35f, -0.12f)
        }, 0.14f, lightMaterial, new Vector3(0f, 0.01f, 0.31f));

        CreateExtrudedPolygon("RightTendril", new[]
        {
            new Vector2(-0.28f, 0.35f), new Vector2(-0.75f, 0.22f),
            new Vector2(-1.00f, -0.10f), new Vector2(-0.67f, -0.01f),
            new Vector2(-0.82f, -0.35f), new Vector2(-0.35f, -0.12f)
        }, 0.14f, lightMaterial, new Vector3(0f, 0.01f, -0.31f));

        CreateExtrudedPolygon("ChestShadow", new[]
        {
            new Vector2(-0.26f, 0.39f), new Vector2(0.12f, 0.26f),
            new Vector2(-0.04f, -0.18f), new Vector2(-0.33f, -0.39f),
            new Vector2(-0.48f, -0.10f)
        }, 0.515f, shadowMaterial, new Vector3(0f, 0f, 0f));

        CreateEye("LeftEye", new Vector3(0.51f, 0.52f, 0.285f), eyeMaterial);
        CreateEye("RightEye", new Vector3(0.51f, 0.52f, -0.285f), eyeMaterial);

        if (GetComponent<GhostBirdTurnAnimation>() == null)
        {
            gameObject.AddComponent<GhostBirdTurnAnimation>();
        }
    }

    private void RemoveLegacyModels()
    {
        var legacy = transform.Find("GhostModel");
        if (legacy == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(legacy.gameObject);
        }
        else
        {
            DestroyImmediate(legacy.gameObject);
        }
    }

    private void EnsureModelRoot()
    {
        if (modelRoot != null)
        {
            return;
        }

        var existing = transform.Find("GhostBirdModel");
        if (existing != null)
        {
            modelRoot = existing;
            return;
        }

        var root = new GameObject("GhostBirdModel");
        modelRoot = root.transform;
        modelRoot.SetParent(transform, false);
    }

    private void ClearModelRoot()
    {
        for (int i = modelRoot.childCount - 1; i >= 0; i--)
        {
            var child = modelRoot.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.Euler(0f, 90f, 0f);
        modelRoot.localScale = Vector3.one;
    }

    private void CreateEye(string name, Vector3 position, Material material)
    {
        var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eye.name = name;
        eye.transform.SetParent(modelRoot, false);
        eye.transform.localPosition = position;
        eye.transform.localScale = new Vector3(0.13f, 0.20f, 0.10f);
        eye.GetComponent<Renderer>().sharedMaterial = material;
        RemoveCollider(eye);
    }

    private GameObject CreateExtrudedPolygon(string name, Vector2[] points, float depth, Material material, Vector3 localPosition = default)
    {
        var go = new GameObject(name);
        go.transform.SetParent(modelRoot, false);
        go.transform.localPosition = localPosition;

        var mesh = new Mesh { name = name + "Mesh" };
        int count = points.Length;
        var vertices = new Vector3[count * 2];
        var triangles = new int[(count - 2) * 6 + count * 6];

        for (int i = 0; i < count; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, depth * 0.5f);
            vertices[i + count] = new Vector3(points[i].x, points[i].y, -depth * 0.5f);
        }

        int t = 0;
        for (int i = 1; i < count - 1; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = i + 1;
            triangles[t++] = i;
            triangles[t++] = count;
            triangles[t++] = count + i;
            triangles[t++] = count + i + 1;
        }

        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;
            triangles[t++] = i;
            triangles[t++] = count + next;
            triangles[t++] = next;
            triangles[t++] = i;
            triangles[t++] = count + i;
            triangles[t++] = count + next;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        return go;
    }

    private static void RemoveCollider(GameObject go)
    {
        var collider = go.GetComponent<Collider>();
        if (collider == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(collider);
        }
        else
        {
            DestroyImmediate(collider);
        }
    }

    private static Material CreateMaterial(Color color)
    {
        // Unlit URP est volontairement utilisé ici : il évite les matériaux magenta
        // et conserve exactement les couleurs de la planche de concept.
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Lit");
        }
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            Debug.LogError("Impossible de trouver un shader Unity compatible.");
            return null;
        }

        var material = new Material(shader);
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        return material;
    }
}
