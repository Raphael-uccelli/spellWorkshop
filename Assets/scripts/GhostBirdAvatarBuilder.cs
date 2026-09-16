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
    private const string MaterialFolder = "Assets/Materials/GhostBird";
#endif

    [SerializeField] private bool autoBuildOnStart = true;
    [SerializeField] private Transform modelRoot;

    [Header("Concept palette")]
    [SerializeField] private Color bodyColor = new Color(0.91f, 0.90f, 0.92f, 1f);
    [SerializeField] private Color lightColor = new Color(0.98f, 0.97f, 0.98f, 1f);
    [SerializeField] private Color darkColor = new Color(0.043f, 0.043f, 0.059f, 1f);
    [SerializeField] private Color shadowColor = new Color(0.18f, 0.16f, 0.23f, 1f);
    [SerializeField] private Color eyeColor = Color.white;

    private void Reset() => BuildAvatar();

    private void Awake()
    {
        if (autoBuildOnStart)
            BuildAvatar();
    }

    [ContextMenu("Build Avatar")]
    public void BuildAvatar()
    {
        EnsureModelRoot();
        if (modelRoot == null)
            return;

        var body = ResolveMaterial("GhostBird_Body", bodyColor);
        var light = ResolveMaterial("GhostBird_Light", lightColor);
        var dark = ResolveMaterial("GhostBird_Dark", darkColor);
        var shadow = ResolveMaterial("GhostBird_Shadow", shadowColor);
        var eye = ResolveMaterial("GhostBird_Eye", eyeColor);

        if (body == null || light == null || dark == null || shadow == null || eye == null)
            return;

        ClearModelRoot();

        // The character faces +Z, which is also the direction used by PlayerAim/FirePoint.
        CreateRingBody("Body", light, new Vector3(0f, -0.02f, 0f));
        CreateEllipsoid("BodyShade", shadow, new Vector3(0.31f, 0.30f, 0.34f), new Vector3(0f, 0.02f, 0.24f));
        CreateEllipsoid("Head", dark, new Vector3(0.38f, 0.39f, 0.33f), new Vector3(0f, 0.73f, 0.02f));
        CreateCone("HeadPoint", dark, new Vector3(0f, 1.00f, -0.01f), new Vector3(0f, 0.27f, 0.18f));
        CreateBeak("Beak", dark, new Vector3(0f, 0.73f, 0.27f));

        CreateEllipsoid("LeftEye", eye, new Vector3(0.075f, 0.13f, 0.055f), new Vector3(-0.145f, 0.80f, 0.305f));
        CreateEllipsoid("RightEye", eye, new Vector3(0.075f, 0.13f, 0.055f), new Vector3(0.145f, 0.80f, 0.305f));

        // Separate rounded side tufts and five rear/bottom points make the silhouette readable from all angles.
        CreateCone("LeftSideTuft", light, new Vector3(-0.34f, 0.18f, 0f), new Vector3(-0.28f, 0.38f, 0.03f));
        CreateCone("RightSideTuft", light, new Vector3(0.34f, 0.18f, 0f), new Vector3(0.28f, 0.38f, 0.03f));
        CreateCone("FrontTendril", light, new Vector3(0f, -0.82f, 0.22f), new Vector3(0f, 0.40f, 0.02f));
        CreateCone("LeftTendril", light, new Vector3(-0.24f, -0.75f, 0.10f), new Vector3(0f, 0.35f, 0.02f));
        CreateCone("RightTendril", light, new Vector3(0.24f, -0.75f, 0.10f), new Vector3(0f, 0.35f, 0.02f));
        CreateCone("BackTendril", body, new Vector3(0f, -0.74f, -0.20f), new Vector3(0f, 0.31f, 0.02f));

        var animation = GetComponent<GhostBirdTurnAnimation>();
        if (animation == null)
            animation = gameObject.AddComponent<GhostBirdTurnAnimation>();
        animation.AssignModelRoot(modelRoot);
    }

    private void EnsureModelRoot()
    {
        var roots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == "GhostModel")
                DestroyObject(child.gameObject);
            else if (child.name == ModelRootName)
                roots.Add(child);
        }

        if (modelRoot == null || modelRoot.parent != transform)
            modelRoot = roots.Count > 0 ? roots[0] : null;

        for (int i = 1; i < roots.Count; i++)
            DestroyObject(roots[i].gameObject);

        if (modelRoot == null)
        {
            var root = new GameObject(ModelRootName);
            modelRoot = root.transform;
            modelRoot.SetParent(transform, false);
        }

        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.identity;
        modelRoot.localScale = Vector3.one;
    }

    private void ClearModelRoot()
    {
        for (int i = modelRoot.childCount - 1; i >= 0; i--)
            DestroyObject(modelRoot.GetChild(i).gameObject);
    }

    private void CreateRingBody(string name, Material material, Vector3 position)
    {
        float[] heights = { -0.84f, -0.68f, -0.40f, -0.08f, 0.22f, 0.48f };
        float[] radiiX = { 0.05f, 0.22f, 0.39f, 0.46f, 0.40f, 0.27f };
        float[] radiiZ = { 0.05f, 0.18f, 0.30f, 0.35f, 0.31f, 0.22f };
        const int segments = 16;

        var vertices = new Vector3[heights.Length * segments];
        var triangles = new List<int>();
        for (int y = 0; y < heights.Length; y++)
        {
            for (int s = 0; s < segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                vertices[y * segments + s] = new Vector3(
                    Mathf.Cos(angle) * radiiX[y],
                    heights[y],
                    Mathf.Sin(angle) * radiiZ[y]);
            }
        }

        for (int y = 0; y < heights.Length - 1; y++)
        {
            for (int s = 0; s < segments; s++)
            {
                int next = (s + 1) % segments;
                int a = y * segments + s;
                int b = y * segments + next;
                int c = (y + 1) * segments + next;
                int d = (y + 1) * segments + s;
                triangles.Add(a); triangles.Add(d); triangles.Add(c);
                triangles.Add(a); triangles.Add(c); triangles.Add(b);
            }
        }

        CreateMeshPart(name, BuildMesh(name, vertices, triangles.ToArray()), material, position);
    }

    private void CreateEllipsoid(string name, Material material, Vector3 scale, Vector3 position)
    {
        const int rings = 10;
        const int segments = 20;
        var vertices = new Vector3[(rings + 1) * segments];
        var triangles = new List<int>();

        for (int r = 0; r <= rings; r++)
        {
            float phi = Mathf.PI * r / rings;
            for (int s = 0; s < segments; s++)
            {
                float theta = Mathf.PI * 2f * s / segments;
                vertices[r * segments + s] = new Vector3(
                    Mathf.Sin(phi) * Mathf.Cos(theta) * scale.x,
                    Mathf.Cos(phi) * scale.y,
                    Mathf.Sin(phi) * Mathf.Sin(theta) * scale.z);
            }
        }

        for (int r = 0; r < rings; r++)
        {
            for (int s = 0; s < segments; s++)
            {
                int next = (s + 1) % segments;
                int a = r * segments + s;
                int b = r * segments + next;
                int c = (r + 1) * segments + next;
                int d = (r + 1) * segments + s;
                triangles.Add(a); triangles.Add(c); triangles.Add(d);
                triangles.Add(a); triangles.Add(b); triangles.Add(c);
            }
        }

        CreateMeshPart(name, BuildMesh(name, vertices, triangles.ToArray()), material, position);
    }

    private void CreateCone(string name, Material material, Vector3 start, Vector3 direction)
    {
        float length = direction.magnitude;
        if (length < 0.01f)
            return;

        const int segments = 8;
        float radius = Mathf.Clamp(length * 0.42f, 0.035f, 0.18f);
        var vertices = new Vector3[segments + 1];
        var triangles = new int[segments * 6];
        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            vertices[i] = new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
        }
        vertices[segments] = Vector3.up * length;
        for (int i = 0; i < segments; i++)
        {
            int n = (i + 1) % segments;
            int t = i * 6;
            triangles[t] = i; triangles[t + 1] = n; triangles[t + 2] = segments;
            triangles[t + 3] = n; triangles[t + 4] = i; triangles[t + 5] = 0;
        }

        var go = CreateMeshPart(name, BuildMesh(name, vertices, triangles), material, start);
        go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    private void CreateBeak(string name, Material material, Vector3 position)
    {
        var vertices = new[]
        {
            new Vector3(-0.16f, 0.10f, 0f), new Vector3(0.16f, 0.10f, 0f),
            new Vector3(0f, -0.10f, 0f), new Vector3(-0.035f, 0.015f, 0.86f),
            new Vector3(0.035f, 0.015f, 0.86f), new Vector3(0f, -0.025f, 0.86f)
        };
        var triangles = new[] { 0, 1, 2, 3, 5, 4, 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 2, 5, 3, 2, 3, 0 };
        CreateMeshPart(name, BuildMesh(name, vertices, triangles), material, position);
    }

    private GameObject CreateMeshPart(string name, Mesh mesh, Material material, Vector3 position)
    {
        var go = new GameObject(name);
        go.transform.SetParent(modelRoot, false);
        go.transform.localPosition = position;
        var filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        return go;
    }

    private static Mesh BuildMesh(string name, Vector3[] vertices, int[] triangles)
    {
        var mesh = new Mesh { name = name + "Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private Material ResolveMaterial(string name, Color color)
    {
        var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("GhostBirdAvatarBuilder: shader URP introuvable.", this);
            return null;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EnsureMaterialFolder();
            string path = $"{MaterialFolder}/{name}.mat";
            var asset = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (asset == null)
            {
                asset = new Material(shader);
                AssetDatabase.CreateAsset(asset, path);
            }
            ApplyColor(asset, color);
            EditorUtility.SetDirty(asset);
            return asset;
        }
#endif

        var runtimeMaterial = new Material(shader);
        ApplyColor(runtimeMaterial, color);
        return runtimeMaterial;
    }

    private static void ApplyColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }

#if UNITY_EDITOR
    private static void EnsureMaterialFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
            AssetDatabase.CreateFolder("Assets/Materials", "GhostBird");
    }
#endif

    private static void DestroyObject(Object target)
    {
        if (target == null) return;
        if (Application.isPlaying) Destroy(target);
        else DestroyImmediate(target);
    }
}
