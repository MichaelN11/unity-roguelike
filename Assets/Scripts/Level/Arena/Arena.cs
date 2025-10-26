using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling an arena where the player is trapped until all enemies are defeated.
/// </summary>
public class Arena : MonoBehaviour
{
    [field: SerializeField]
    public GameObject TriggerArea { get; private set; }

    [SerializeField]
    private List<Chest> rewardChests;

    [SerializeField]
    private List<ArenaStage> arenaStages;

    [SerializeField]
    private ArenaEncounterTable encounterTable;

    /// <summary>
    /// Whether to only spawn enemies when the player enters the trigger area.
    /// If false, enemies will spawn immediately on level load.
    /// </summary>
    [SerializeField]
    private bool spawnOnlyOnTrigger = false;

    [SerializeField]
    private float enemySpawnTime = 2f;

    [SerializeField]
    private float chestSpawnTime = 1f;

    [SerializeField]
    private bool stopMusic = true;

    [SerializeField]
    private float musicStopDelay = 1.0f;

    [SerializeField]
    private GameObject spawnEffect;

    private GameObject player = null;
    private Bounds triggerBounds;
    private bool readyToTrigger = true;
    private EventWall[] eventWalls;
    private ArenaSpawner[] spawners;
    private List<ArenaSpawner> availableSpawners;
    private int enemiesRemaining = 0;
    private List<GameObject> spawnedEnemies = new();
    private List<Entity> enemiesToSpawn;
    private bool enemiesAggroed = false;
    private int currentWaveIndex = 0;
    private IEnumerator spawnCoroutine = null;
    private List<ArenaWave> waves = new();

    /// <summary>
    /// Workaround for Unity not initializing serialized lists with default values.
    /// https://issuetracker.unity3d.com/issues/serializefield-list-objects-are-not-initialized-with-class-slash-struct-default-values-when-adding-objects-in-the-inspector-window
    /// </summary>
    private void OnValidate()
    {
        if (arenaStages == null || arenaStages.Count == 0)
        {
            arenaStages = new List<ArenaStage> { new ArenaStage() };
        }
    }

    void Start()
    {
        InitializeWaves();

        if (TriggerArea != null)
        {
            triggerBounds = new Bounds();
            triggerBounds.SetMinMax(TriggerArea.transform.position,
                TriggerArea.transform.position + new Vector3(TriggerArea.transform.localScale.x, TriggerArea.transform.localScale.y));
        }
        else
        {
            Debug.LogWarning("Trigger area object not set for Arena.");
        }

        eventWalls = GetComponentsInChildren<EventWall>();
        spawners = GetComponentsInChildren<ArenaSpawner>();
        if (!spawnOnlyOnTrigger)
        {
            SpawnEnemies(false);
        }

        foreach (Chest chest in rewardChests)
        {
            chest.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.gameObject;
            Debug.Log("Player found for arena trigger.");
        }

        if (player != null
            && triggerBounds != null
            && readyToTrigger
            && triggerBounds.Contains(player.transform.position))
        {
            Debug.Log("Player entered arena trigger area.");
            foreach (EventWall eventWall in eventWalls)
            {
                eventWall.TriggerWall();
            }
            readyToTrigger = false;
            if (spawnOnlyOnTrigger)
            {
                SpawnEnemies(true);
            }
            AggroEnemies();
        }
    }

    private void SpawnEnemies(bool delayed)
    {
        ArenaWave currentWave = waves[currentWaveIndex];
        List<EntitySpawn> waveSpawns = currentWave.EnemySpawns;
        if (waveSpawns == null || waveSpawns.Count == 0 || currentWave.MaxInitialEnemies <= 0)
        {
            Debug.LogWarning("No enemy spawns defined for arena wave " + currentWaveIndex);
            CompleteWave();
            return;
        }
        enemiesToSpawn = new();
        foreach (EntitySpawn entitySpawn in waveSpawns)
        {
            for (int i = 0; i < entitySpawn.Amount; i++)
            {
                enemiesToSpawn.Add(entitySpawn.Entity);
            }
        }
        availableSpawners = new(spawners);

        if (availableSpawners.Count > enemiesToSpawn.Count 
            || availableSpawners.Count > currentWave.MaxInitialEnemies)
        {
            // Shuffle spawner locations to get random selection
            for (int i = 0; i < availableSpawners.Count; i++)
            {
                int randomIndex = Random.Range(0, availableSpawners.Count);
                ArenaSpawner temp = availableSpawners[i];
                availableSpawners[i] = availableSpawners[randomIndex];
                availableSpawners[randomIndex] = temp;
            }
        }
        foreach (ArenaSpawner spawner in availableSpawners)
        {
            if (enemiesToSpawn.Count == 0
                || enemiesRemaining >= currentWave.MaxInitialEnemies)
            {
                break;
            }
            SpawnEnemy(spawner, delayed);
        }
        if (currentWave.EnemySpawnInterval > 0f)
        {
            spawnCoroutine = SpawnEnemyOnTimer(currentWave.EnemySpawnInterval, currentWave.MaxEnemiesAtOnce);
            StartCoroutine(spawnCoroutine);
        }
    }

    private void AggroEnemies()
    {
        enemiesAggroed = true;
        foreach (GameObject enemy in spawnedEnemies)
        {
            AIController aiController = enemy.GetComponent<AIController>();
            aiController?.AggroPermanently();
        }
    }

    private void EnemyDeath(DeathContext deathContext)
    {
        enemiesRemaining--;
        if (waves[currentWaveIndex].SpawnMoreOnKill && enemiesToSpawn.Count > 0)
        {
            StartCoroutine(SpawnEnemyAtAvailableSpawner());
        }
        else if (enemiesRemaining <= 0)
        {
            CompleteWave();
        }
    }

    private IEnumerator SpawnEnemyAtAvailableSpawner()
    {
        while (availableSpawners.Count == 0)
        {
            Debug.Log("No available spawners, waiting...");
            yield return new WaitForSeconds(1f);
        }
        
        // Add check for enemies remaining to spawn
        if (enemiesToSpawn.Count == 0)
        {
            Debug.Log("No more enemies to spawn");
            yield break;
        }
        
        int randomSpawnerIndex = Random.Range(0, availableSpawners.Count);
        ArenaSpawner spawner = availableSpawners[randomSpawnerIndex];
        availableSpawners.RemoveAt(randomSpawnerIndex);
        SpawnEnemy(spawner, true);
    }

    private void SpawnEnemy(ArenaSpawner spawner, bool delayed)
    {
        // Safety check
        if (enemiesToSpawn.Count == 0)
        {
            Debug.LogWarning("Attempted to spawn enemy with empty enemies list");
            return;
        }

        Entity enemy;
        if (waves[currentWaveIndex].randomOrder)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            enemy = enemiesToSpawn[randomIndex];
            enemiesToSpawn.RemoveAt(randomIndex);
        }
        else
        {
            enemy = enemiesToSpawn[0];
            enemiesToSpawn.RemoveAt(0);
        }
        enemiesRemaining++;

        if (delayed)
        {
            IEnumerator coroutine = CreateEnemyAfterDelay(enemy, spawner, enemySpawnTime);
            StartCoroutine(coroutine);

            if (spawnEffect != null)
            {
                GameObject effect = Instantiate(spawnEffect, spawner.transform.position, Quaternion.identity);
                effect.GetComponent<DestroyTimer>().Duration = enemySpawnTime;
            }
        }
        else
        {
            CreateEnemy(enemy, spawner.transform.position);
        }
    }

    private IEnumerator CreateEnemyAfterDelay(Entity enemy, ArenaSpawner spawner, float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateEnemy(enemy, spawner.transform.position);

        if (!availableSpawners.Contains(spawner)) {
            availableSpawners.Add(spawner);
        }
    }

    private void CreateEnemy(Entity enemy, Vector2 position)
    {
        GameObject enemyObject = EntityFactory.CreateEnemy(enemy, position);
        enemyObject.GetComponent<EntityState>().OnDeath += EnemyDeath;
        spawnedEnemies.Add(enemyObject);

        if (enemiesAggroed)
        {
            enemyObject.GetComponent<AIController>()?.AggroPermanently();
        }
    }

    private IEnumerator SpawnEnemyOnTimer(float interval, int maxAtOnce)
    {
        while (enemiesToSpawn.Count > 0 && enemiesRemaining < maxAtOnce)
        {
            yield return new WaitForSeconds(interval);
            StartCoroutine(SpawnEnemyAtAvailableSpawner());
        }
    }

    private void CompleteWave()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (currentWaveIndex < waves.Count - 1)
        {
            currentWaveIndex++;
            SpawnEnemies(true);
        }
        else
        {
            EndArena();
        }
    }

    private void EndArena()
    {
        if (stopMusic)
        {
            StartCoroutine(StopMusicAfterDelay(musicStopDelay));
        }
        foreach (EventWall eventWall in eventWalls)
        {
            eventWall.RetractWall();
        }

        if (spawnEffect != null)
        {
            foreach (Chest chest in rewardChests)
            {
                GameObject effect = Instantiate(spawnEffect, chest.transform.position, Quaternion.identity);
                effect.GetComponent<DestroyTimer>().Duration = chestSpawnTime;
            }
        }
        StartCoroutine(SpawnChestsAfterDelay(chestSpawnTime));
    }

    private IEnumerator SpawnChestsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Chest chest in rewardChests)
        {
            chest.gameObject.SetActive(true);
            ItemDrop randomItemDrop = ItemDropUtil.GetRandomItemDrop(GameManager.Instance.ShuffledRareDrops, true);
            if (randomItemDrop != null)
            {
                chest.AddInventoryItem(randomItemDrop.InventoryItem);
            } else
            {
                Debug.LogWarning("No item drop available for arena reward chest.");
            }
        }
    }

    private IEnumerator StopMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.StopMusic();
    }
    
    private void InitializeWaves()
    {
        List<ArenaWave> veryEasyWaves = null;
        List<ArenaWave> easyWaves = null;
        List<ArenaWave> mediumWaves = null;
        List<ArenaWave> hardWaves = null;
        List<ArenaWave> veryHardWaves = null;

        if (encounterTable != null)
        {
            veryEasyWaves = new(encounterTable.VeryEasyWaves);
            easyWaves = new(encounterTable.EasyWaves);
            mediumWaves = new(encounterTable.MediumWaves);
            hardWaves = new(encounterTable.HardWaves);
            veryHardWaves = new(encounterTable.VeryHardWaves);
        }

        foreach(ArenaStage stage in arenaStages)
        {
            if (stage.Difficulty == WaveDifficulty.Static)
            {
                if (stage.StaticWave != null)
                {
                    waves.Add(stage.StaticWave);
                }
                else
                {
                    Debug.LogWarning("Static wave not defined for static arena stage.");
                }
            }
            else
            {
                if (encounterTable == null)
                {
                    Debug.LogWarning("Encounter table not defined for arena with non-static stage.");
                    continue;
                }

                List<ArenaWave> tempWaveList = null;
                List<ArenaWave> fullWaveList = null;
                switch (stage.Difficulty)
                {
                    case WaveDifficulty.VeryEasy:
                        tempWaveList = veryEasyWaves;
                        fullWaveList = encounterTable.VeryEasyWaves;
                        break;
                    case WaveDifficulty.Easy:
                        tempWaveList = easyWaves;
                        fullWaveList = encounterTable.EasyWaves;
                        break;
                    case WaveDifficulty.Medium:
                        tempWaveList = mediumWaves;
                        fullWaveList = encounterTable.MediumWaves;
                        break;
                    case WaveDifficulty.Hard:
                        tempWaveList = hardWaves;
                        fullWaveList = encounterTable.HardWaves;
                        break;
                    case WaveDifficulty.VeryHard:
                        tempWaveList = veryHardWaves;
                        fullWaveList = encounterTable.VeryHardWaves;
                        break;
                    default:
                        Debug.LogWarning("Unknown wave difficulty for arena stage.");
                        break;
                }

                if (tempWaveList != null && tempWaveList.Count > 0)
                {
                    int randomIndex = Random.Range(0, tempWaveList.Count);
                    ArenaWave selectedWave = tempWaveList[randomIndex];
                    waves.Add(selectedWave);
                    tempWaveList.RemoveAt(randomIndex);
                    if (tempWaveList.Count == 0)
                    {
                        Debug.Log("All waves used for difficulty " + stage.Difficulty + ", resetting wave list.");
                        tempWaveList.AddRange(fullWaveList);
                    }
                }
                else
                {
                    Debug.LogWarning("No waves available for arena stage difficulty " + stage.Difficulty);
                }
            }
        }
    }
}
