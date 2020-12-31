using System.Collections.Generic;
using UnityEditor;

public static class ManuallyReserializeAssets
{
    [MenuItem("Tools/Reserialize Prefabs")]
    public static void ReserializePrefabs()
    {
        var prefabGuids = AssetDatabase.FindAssets("t:prefab");
        var prefabs = new List<string>();
        foreach (var guid in prefabGuids)
        {
            prefabs.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        AssetDatabase.ForceReserializeAssets(prefabs);
    }
}