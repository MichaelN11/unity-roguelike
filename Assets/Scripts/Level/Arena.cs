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
    private List<Entity> enemies = new();

    [SerializeField]
    private bool spawnOnlyOnTrigger = false;

    [SerializeField]
    private bool stopMusic = true;

    [SerializeField]
    private float musicStopDelay = 1.0f;

    private GameObject player = null;
    private Bounds triggerBounds;
    private bool readyToTrigger = true;
    private EventWall[] eventWalls;
    private ArenaSpawner[] spawners;
    private int enemiesRemaining = 0;
    private List<GameObject> spawnedEnemies = new();

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
            SpawnEnemies();
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
                SpawnEnemies();
            }
            AggroEnemies();
        }
    }

    private void SpawnEnemies()
    {
        List<Entity> enemiesToSpawn = new(enemies);
        List<ArenaSpawner> spawnerLocations = new(spawners);
        if (spawnerLocations.Count > enemiesToSpawn.Count)
        {
            for (int i = 0; i < spawnerLocations.Count; i++)
            {
                int randomIndex = Random.Range(0, spawnerLocations.Count);
                ArenaSpawner temp = spawnerLocations[i];
                spawnerLocations[i] = spawnerLocations[randomIndex];
                spawnerLocations[randomIndex] = temp;
            }
        }
        foreach (ArenaSpawner spawner in spawnerLocations)
        {
            if (enemiesToSpawn.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            Entity enemy = enemiesToSpawn[randomIndex];
            GameObject enemyObject = EntityFactory.CreateEnemy(enemy, spawner.transform.position);
            enemyObject.GetComponent<EntityState>().OnDeath += EnemyDeath;
            spawnedEnemies.Add(enemyObject);
            enemiesRemaining++;
            enemiesToSpawn.RemoveAt(randomIndex);
        }
    }

    private void AggroEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            AIController aiController = enemy.GetComponent<AIController>();
            aiController?.AggroPermanently();
        }
    }

    private void EnemyDeath(DeathContext deathContext)
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
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
    }
    
    private IEnumerator StopMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.StopMusic();
    }
}
