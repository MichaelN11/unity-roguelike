using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Service class for loading game data using Unity's Addressables.
///
/// Uses a synchronous approach since individual game data should not be very large.
/// </summary>
public class AddressableService
{
    private const string EntityAddress = "Assets/Game Data/Entities/";
    private const string EntityFileType = ".asset";

    public static Entity LoadEntity(string name)
    {
        return Addressables.LoadAssetAsync<Entity>(EntityAddress + name + EntityFileType).WaitForCompletion();
    }
}
