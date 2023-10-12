// Original source: https://forum.unity.com/threads/serialized-variable-lost-its-value-after-removing-formerserializedas.621055/
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for forcing serialization. Useful for renaming variables in scriptable objects with the FormerlySerializedAs attribute.
/// This script can be called and then the attribute can be removed.
/// </summary>
internal class EditorForceSaveAll : Editor
{
    [MenuItem("Tools/Force Save All")]
    private static void ForceSaveAll()
    {
        // Gets the guid of assets present in the Assets folder only, assets in the Packages folder are immutable
        // and trying to save the logs errors out even though it does not prevent the saving process to complete.
        var guids = AssetDatabase.FindAssets(string.Empty, new[] { "Assets" });

        Debug.Log($"Forcefully re-saving {guids.Length} assets.");

        // Caching the length of the collection would be more relevant with a List<T>, but the array can be very long.
        for (int i = 0, length = guids.Length; i < length; i++)
        {
            if (string.IsNullOrEmpty(guids[i]))
                continue;

            var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guids[i]));
            if (asset == null)
                continue;

            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
    }
}
