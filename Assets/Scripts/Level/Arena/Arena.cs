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
    private List<ArenaWave> waves;

    /// <summary>
    /// Whether to only spawn enemies when the player enters the trigger area.
    /// If false, enemies will spawn immediately on level load.
    /// </summary>
    [SerializeField]
    private bool spawnOnlyOnTrigger = false;

    [SerializeField]
    private float enemySpawnTime = 2f;

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

    /// <summary>
    /// Workaround for Unity not initializing serialized lists with default values.
    /// https://issuetracker.unity3d.com/issues/serializefield-list-objects-are-not-initialized-with-class-slash-struct-default-values-when-adding-objects-in-the-inspector-window
    /// </summary>
    private void OnValidate()
    {
        if (waves == null || waves.Count == 0)
        {
            waves = new List<ArenaWave> { new ArenaWave() };
        }
    }

    void Start()
    {
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
        int randomSpawnerIndex = Random.Range(0, availableSpawners.Count);
        ArenaSpawner spawner = availableSpawners[randomSpawnerIndex];
        availableSpawners.RemoveAt(randomSpawnerIndex);
        SpawnEnemy(spawner, true);
    }

    private void SpawnEnemy(ArenaSpawner spawner, bool delayed)
    {
        int randomIndex = Random.Range(0, enemiesToSpawn.Count);
        Entity enemy = enemiesToSpawn[randomIndex];
        enemiesRemaining++;
        enemiesToSpawn.RemoveAt(randomIndex);

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
    }
    
    private IEnumerator StopMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.StopMusic();
    }
}
