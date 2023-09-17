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
    /// <summary>
    /// Entities currently use this layer.
    /// </summary>
    private const string DefaultLayerName = "Default";

    private static readonly string[] unwalkableLayerNames = { WallLayerName, BlockedGroundLayerName };

    /// <summary>
    /// Gets the layer mask for the entity collision layers.
    /// </summary>
    /// <returns> The layer mask for the entity layers as an int</returns>
    public static int GetEntityLayerMask()
    {
        return LayerMask.GetMask(DefaultLayerName);
    }

    /// <summary>
    /// Gets the layer mask for the unwalkable/unpathable collision layers.
    /// </summary>
    /// <returns> The layer mask for the unwalkable layers as an int</returns>
    public static int GetUnwalkableLayerMask()
    {
        return LayerMask.GetMask(unwalkableLayerNames);
    }

    /// <summary>
    /// Gets the layer mask for the wall collision layer.
    /// </summary>
    /// <returns> The layer mask for the wall layer as an int</returns>
    public static int GetWallLayerMask()
    {
        return LayerMask.GetMask(WallLayerName);
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

    /// <summary>
    /// Determines if the passed layer is a wall layer.
    /// </summary>
    /// <param name="layer">The layer as an int</param>
    /// <returns>true if the layer is a wall</returns>
    public static bool IsWall(int layer)
    {
        String layerName = LayerMask.LayerToName(layer);
        return layerName == WallLayerName;
    }
}
