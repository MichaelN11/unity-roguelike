using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for dealing with Unity collision layers.
/// </summary>
public class LayerUtil
{
    /// <summary>
    /// The wall layer blocks everything from passing through it.
    /// </summary>
    private const string WallLayerName = "Wall";
    /// <summary>
    /// The blocked ground layer blocks entities from walking on it, but doesn't block
    /// objects from passing over it.
    /// </summary>
    private const string BlockedGroundLayerName = "BlockedGround";

    private static readonly string[] unwalkableLayerNames = { WallLayerName, BlockedGroundLayerName };

    /// <summary>
    /// Gets the layer mask for the unwalkable/unpathable collision layers.
    /// </summary>
    /// <returns> The layer mask for the unwalkable layers as an int</returns>
    public static int GetUnwalkableLayerMask()
    {
        return LayerMask.GetMask(unwalkableLayerNames);
    }

    /// <summary>
    /// Determines if the passed layer is an unwalkable/unpathable layer.
    /// </summary>
    /// <param name="layer">The layer as an int</param>
    /// <returns>true if the layer is unwalkable</returns>
    public static bool IsUnwalkable(int layer)
    {
        String layerName = LayerMask.LayerToName(layer);
        return Array.Exists(unwalkableLayerNames, name => name == layerName);
    }
}
