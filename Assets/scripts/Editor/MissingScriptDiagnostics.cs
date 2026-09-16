#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MissingScriptDiagnostics
{
    [MenuItem("Tools/SpellWorkshop/Report Missing Scripts In Open Scenes")]
    public static void ReportMissingScriptsInOpenScenes()
    {
        int totalMissing = 0;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
            {
                continue;
            }

            foreach (var root in scene.GetRootGameObjects())
            {
                totalMissing += CollectMissingScripts(root, scene.path);
            }
        }

        if (totalMissing == 0)
        {
            Debug.Log("MissingScriptDiagnostics: aucun composant Missing (Mono Script) détecté dans les scènes ouvertes.");
            return;
        }

        Debug.LogWarning($"MissingScriptDiagnostics: {totalMissing} composant(s) Missing (Mono Script) détecté(s). Consultez les logs détaillés.");
    }

    private static int CollectMissingScripts(GameObject root, string scenePath)
    {
        int count = 0;
        var stack = new Stack<Transform>();
        stack.Push(root.transform);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            var components = current.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    continue;
                }

                count++;
                Debug.LogError($"MissingScriptDiagnostics: script manquant dans '{GetTransformPath(current)}' (scene: {scenePath}).", current.gameObject);
            }

            for (int i = 0; i < current.childCount; i++)
            {
                stack.Push(current.GetChild(i));
            }
        }

        return count;
    }

    private static string GetTransformPath(Transform transform)
    {
        var names = new Stack<string>();
        var current = transform;

        while (current != null)
        {
            names.Push(current.name);
            current = current.parent;
        }

        return string.Join("/", names.ToArray());
    }
}
#endif
