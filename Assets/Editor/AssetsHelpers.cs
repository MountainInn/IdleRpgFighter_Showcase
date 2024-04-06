#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AssetsHelpers
{
    private const string BAKED_PREF = "Baked_";
    private const string LIT_PREF = "Lit_";
    const string UnityPostfix = " (Instance)";

    private static string LogPath => Directory.GetCurrentDirectory() + "/utils/logs/";
    private const string LIT_MATERIALS_PATH = "Materials/Lit/";
    private const string BAKED_MATERIALS_PATH = "Materials/Baked/";

    [MenuItem("Tools/Utils/ List all paths for meshes")]
    public static void LogAssetsPathAtCurrentScene()
    {
        int sceneCount = SceneManager.sceneCount;
        StringBuilder sb = new StringBuilder(1000);
        for (int i = 0; i < sceneCount; i++)
        {

            List<MeshFilter> meshes = new List<MeshFilter>();
            Scene curScene = SceneManager.GetSceneAt(i);
            var allObjects = curScene.GetRootGameObjects();
            foreach (var item in allObjects)
            {
                meshes.AddRange(item.GetComponentsInChildren<MeshFilter>(true));
            }
            sb.AppendLine($"------{curScene.name}------");

            foreach (var item in meshes)
            {
                string path = AssetDatabase.GetAssetPath(item.sharedMesh);
                sb.AppendLine($"GO: {item.gameObject.name} Mesh Path: {path}");
            }
        }
        Debug.Log(LogPath + "meshes.txt");
        Utils.FileUtils.WriteAllText(LogPath + "meshes.txt", sb.ToString());
    }

    [MenuItem("Tools/Utils/ Switch materials to Baked")]
    public static void SwitchMaterialToBaked()
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {

            List<MeshRenderer> meshes = new List<MeshRenderer>();
            Scene curScene = SceneManager.GetSceneAt(i);
            var allObjects = curScene.GetRootGameObjects();
            foreach (var item in allObjects)
            {
                meshes.AddRange(item.GetComponentsInChildren<MeshRenderer>(true));
            }
            ReplcaseMaterials(BAKED_MATERIALS_PATH, LIT_PREF, BAKED_PREF, meshes);

        }
        Debug.Log("Done");
    }

    [MenuItem("Tools/Utils/ Switch materials to Lit")]
    public static void SwitchMaterialToLit()
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {

            List<MeshRenderer> meshes = new List<MeshRenderer>();
            Scene curScene = SceneManager.GetSceneAt(i);
            var allObjects = curScene.GetRootGameObjects();
            foreach (var item in allObjects)
            {
                meshes.AddRange(item.GetComponentsInChildren<MeshRenderer>(true));
            }
            ReplcaseMaterials(LIT_MATERIALS_PATH, BAKED_PREF, LIT_PREF, meshes);
        }
        Debug.Log("Done");
    }

    [MenuItem("Tools/Utils/ Test")]
    public static void Test()
    {
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {

            List<MeshRenderer> meshes = new List<MeshRenderer>();
            Scene curScene = SceneManager.GetSceneAt(i);
            var allObjects = curScene.GetRootGameObjects();
            foreach (var item in allObjects)
            {
                var mr = item.GetComponentsInChildren<MeshRenderer>(true).WhereCast<MeshRenderer>().Where(m => !m.material.name.StartsWith(BAKED_PREF) || !m.material.name.StartsWith(LIT_PREF));
                meshes.AddRange(mr);

            }
            string path = AssetDatabase.GetAssetPath(meshes[0].sharedMaterial);
            path = AssetDatabase.GetAssetPath(meshes[0].material);
            var mates = new List<Material>();
            meshes[0].GetSharedMaterials(mates);
            path = AssetDatabase.GetAssetPath(mates[0]);

            
        }
    }


[MenuItem("Tools/Utils/ Change materials to our materials")]
public static void SwichAllMatToBaked()
{
    int sceneCount = SceneManager.sceneCount;
    for (int i = 0; i < sceneCount; i++)
    {

        List<MeshRenderer> meshes = new List<MeshRenderer>();
        Scene curScene = SceneManager.GetSceneAt(i);
        var allObjects = curScene.GetRootGameObjects();
        foreach (var item in allObjects)
        {
            var mr = item.GetComponentsInChildren<MeshRenderer>(true).WhereCast<MeshRenderer>().Where(m => !m.sharedMaterial.name.StartsWith(BAKED_PREF) || !m.sharedMaterial.name.StartsWith(LIT_PREF));
            meshes.AddRange(mr);

        }
        var materials = Resources.LoadAll(BAKED_MATERIALS_PATH).WhereCast<Material>().ToList();

        foreach (var item in meshes)
        {
            var curMats = item.sharedMaterials;
            List<Material> newMaterials = new List<Material>(curMats);
            for (int j = 0; j < curMats.Length; j++)
            {

                var oldMat = curMats[j];
                string oldName = oldMat.name;
                if (oldName.StartsWith(BAKED_PREF)) continue;
                if (oldName.StartsWith(LIT_PREF))
                {
                    oldName = oldName.Replace(LIT_PREF, BAKED_PREF);
                }
                string name = oldName.Remove(oldName.IndexOf(UnityPostfix)).Insert(0, BAKED_PREF);
                var mater = materials.Where(m => m.name == name && m.mainTexture == oldMat.mainTexture)?
                         .FirstOrDefault();

                if (mater == null)
                {
                    Debug.LogError($"Cant find material with name {name}");
                    continue;
                }
                Debug.Log($"Loaded material for {item.gameObject.name}");
                newMaterials[i] = mater;
            }

            item.SetMaterials(newMaterials);
        }



    }
    Debug.Log("Done");
}
private static void ReplcaseMaterials(string loadPath, string cutMatPref, string matPref, List<MeshRenderer> instanses)
{
    var materials = Resources.LoadAll(loadPath).WhereCast<Material>().ToList();

    foreach (var item in instanses)
    {
        var curMats = item.sharedMaterials;
        List<Material> newMaterials = new List<Material>(curMats);
        for (int i = 0; i < curMats.Length; i++)
        {
            var oldMat = curMats[i];
            if (oldMat.name.StartsWith(matPref)) continue;
            string name = oldMat.name.Replace(cutMatPref, matPref);
            name = name.Remove(name.IndexOf(UnityPostfix));
            var mater = materials.Where(m => m.name == name && m.mainTexture == oldMat.mainTexture)?
                     .FirstOrDefault();

            if (mater == null)
            {
                Debug.LogError($"Cant find material with name {name}");
                continue;
            }
            Debug.Log($"Loaded material for {item.gameObject.name}");
            newMaterials[i] = mater;
        }

        item.SetMaterials(newMaterials);
    }
}
}

#endif
