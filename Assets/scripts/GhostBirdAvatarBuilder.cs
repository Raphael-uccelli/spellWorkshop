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
        EnsureModelRoot();
        ClearModelRoot();

        var bodyMaterial = CreateMaterial(bodyColor);
        var lightMaterial = CreateMaterial(lightColor);
        var darkMaterial = CreateMaterial(darkColor);
        var shadowMaterial = CreateMaterial(shadowColor);
        var eyeMaterial = CreateMaterial(eyeColor);

        // Corps fantomatique : silhouette blanche irrégulière, plus proche du concept que des primitives.
        CreateExtrudedPolygon(
            "Body",
            new[]
            {
                new Vector2(-0.58f, 0.54f),
                new Vector2(-0.30f, 0.78f),
                new Vector2(0.02f, 0.72f),
                new Vector2(0.28f, 0.45f),
                new Vector2(0.22f, 0.08f),
                new Vector2(0.04f, -0.25f),
                new Vector2(-0.20f, -0.60f),
                new Vector2(-0.38f, -0.40f),
                new Vector2(-0.55f, -0.72f),
                new Vector2(-0.52f, -0.23f),
                new Vector2(-0.77f, -0.42f),
                new Vector2(-0.72f, 0.04f),
            },
            0.56f,
            bodyMaterial);

        // Partie basse plus lumineuse pour retrouver le dégradé clair du dessin.
        CreateExtrudedPolygon(
            "Chest",
            new[]
            {
                new Vector2(-0.42f, 0.38f),
                new Vector2(-0.16f, 0.54f),
                new Vector2(0.16f, 0.34f),
                new Vector2(0.10f, 0.02f),
                new Vector2(-0.12f, -0.34f),
                new Vector2(-0.30f, -0.16f),
                new Vector2(-0.52f, -0.28f),
                new Vector2(-0.58f, 0.02f),
            },
            0.59f,
            lightMaterial,
            new Vector3(-0.03f, -0.02f, 0f));

        // Tête noire en forme de capuche, avec sommet pointu.
        CreateExtrudedPolygon(
            "Head",
            new[]
            {
                new Vector2(0.05f, 0.33f),
                new Vector2(0.18f, 0.72f),
                new Vector2(0.47f, 0.93f),
                new Vector2(0.70f, 0.86f),
                new Vector2(0.58f, 0.60f),
                new Vector2(0.68f, 0.27f),
                new Vector2(0.48f, -0.02f),
                new Vector2(0.18f, 0.02f),
            },
            0.62f,
            darkMaterial,
            new Vector3(0.06f, 0.22f, 0f));

        // Bec long et plongeant.
        CreateExtrudedPolygon(
            "Beak",
            new[]
            {
                new Vector2(0.48f, 0.46f),
                new Vector2(1.38f, 0.34f),
                new Vector2(0.90f, 0.13f),
                new Vector2(0.58f, 0.16f),
            },
            0.34f,
            darkMaterial,
            new Vector3(0.12f, 0.20f, 0f));

        // Ailes / mèches latérales blanches.
        CreateExtrudedPolygon(
            "LeftWing",
            new[]
            {
                new Vector2(-0.25f, 0.34f),
                new Vector2(-0.78f, 0.26f),
                new Vector2(-1.02f, -0.08f),
                new Vector2(-0.72f, 0.00f),
                new Vector2(-0.88f, -0.34f),
                new Vector2(-0.38f, -0.12f),
            },
            0.20f,
            lightMaterial,
            new Vector3(0f, 0.02f, 0.39f));

        CreateExtrudedPolygon(
            "RightWing",
            new[]
            {
                new Vector2(-0.25f, 0.34f),
                new Vector2(-0.78f, 0.26f),
                new Vector2(-1.02f, -0.08f),
                new Vector2(-0.72f, 0.00f),
                new Vector2(-0.88f, -0.34f),
                new Vector2(-0.38f, -0.12f),
            },
            0.20f,
            lightMaterial,
            new Vector3(0f, 0.02f, -0.39f));

        // Ombre violette sous la tête et sur le ventre.
        CreateExtrudedPolygon(
            "ChestShadow",
            new[]
            {
                new Vector2(-0.26f, 0.40f),
                new Vector2(0.12f, 0.28f),
                new Vector2(-0.04f, -0.18f),
                new Vector2(-0.33f, -0.40f),
                new Vector2(-0.48f, -0.10f),
            },
            0.605f,
            shadowMaterial,
            new Vector3(0f, 0f, 0f));

        // Les yeux restent des volumes ronds pour conserver le regard de la référence.
        CreateEye("LeftEye", new Vector3(0.52f, 0.53f, 0.33f), eyeMaterial);
        CreateEye("RightEye", new Vector3(0.52f, 0.53f, -0.33f), eyeMaterial);

        if (GetComponent<GhostBirdTurnAnimation>() == null)
        {
            gameObject.AddComponent<GhostBirdTurnAnimation>();
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
        eye.transform.localScale = new Vector3(0.14f, 0.22f, 0.12f);
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
            triangles[t++] = i;
            triangles[t++] = i + 1;
            triangles[t++] = count;
            triangles[t++] = count + i + 1;
            triangles[t++] = count + i;
        }

        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;
            triangles[t++] = i;
            triangles[t++] = next;
            triangles[t++] = count + next;
            triangles[t++] = i;
            triangles[t++] = count + next;
            triangles[t++] = count + i;
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
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            Debug.LogError("Aucun shader compatible n'a été trouvé.");
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
        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.12f);
        }
        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", 0.12f);
        }

        return material;
    }
}
