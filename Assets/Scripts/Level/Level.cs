using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object representing a level.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Level")]
public class Level : ScriptableObject
{
    [SerializeField]
    private int minChestsPerLevel = 1;
    public int MinChestsPerLevel => minChestsPerLevel;

    [SerializeField]
    private int maxChestsPerLevel = 1;
    public int MaxChestsPerLevel => maxChestsPerLevel;

    [SerializeField]
    private int minEnemiesPerTile = 4;
    public int MinEnemiesPerTile => minEnemiesPerTile;

    [SerializeField]
    private int maxEnemiesPerTile = 4;
    public int MaxEnemiesPerTile => maxEnemiesPerTile;

    [SerializeField]
    private List<GameObject> tiles;
    public List<GameObject> Tiles => tiles;

    [SerializeField]
    private List<Entity> meleeEnemies;
    public List<Entity> MeleeEnemies => meleeEnemies;

    [SerializeField]
    private List<Entity> rangedEnemies;
    public List<Entity> RangedEnemies => rangedEnemies;

    [SerializeField]
    private Sound music;
    public Sound Music => music;
}
