using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 

    // time between spawns
    public float spawnInterval = 5f; 

    public Transform playerTransform; 

    // used to assign map layer to avoid spawning on non map layered tiles
    public LayerMask mapLayer; 

    // bool used to stop spawning enemies
    public bool finalChestCollected;

    // timer used for spawning interval
    private float timer;

    // used to spawn enemies in camera view
    private Camera mainCamera;

    // used to access enemy sprite variations from sprite library
    public SpriteLibrary spriteLibrary;

    // use to create an array of enemy sprite variations from sprite library
    public SpriteLibraryAsset[] enemyRefs;

    // enemies alive state
    public bool isDead = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Only spawn enemies if the final chest has not been collected
        if (!finalChestCollected) 
        {
            timer += Time.deltaTime;
            if (timer > spawnInterval)
            {
                SpawnEnemy();
                timer = 0;
            }
        }
    }

    // Spawns enemies in random locations on the game map
    void SpawnEnemy()
    {
        // used to stop invalid spawns at invalid locations
        bool validPosition = false;
        Vector2 spawnPosition = Vector2.zero;

        // keeps track of spawn attempts
        int attempts = 0;

        // Prevents infinite loops
        while (!validPosition && attempts < 100) 
        {
            // Determine spawn position within camera view
            Vector2 viewPortPosition = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(viewPortPosition.x, viewPortPosition.y, mainCamera.nearClipPlane));

            // Check distance from the player
            if (Vector2.Distance(spawnPosition, playerTransform.position) >= 4)
            {
                // Check for collision with map objects
                if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, mapLayer))
                {
                    validPosition = true;
                }
            }

            attempts++;
        }
        
        // spawns enemy if a valid position on the game map is found
        if (validPosition)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // calls function to generate one of a variety of enemy sprite variations
            RandomizeEnemySprite(enemy);
        }
    }

    void RandomizeEnemySprite(GameObject enemy)
    {
        // gets the enemy sprite library component
        var spriteLibrary = enemy.GetComponent<SpriteLibrary>();

        // if final treasure chest hasn't been collected
        if (spriteLibrary != null && !finalChestCollected)
        {
            // Choose a random enemy variant from the array of enemies in the sprite library
            // -1 is used as the Boss sprite is the last sprite in the array and don't want
            // the boss spawned untill all chests have been collected
            SpriteLibraryAsset variant = enemyRefs[Random.Range(0, enemyRefs.Length - 1)];
            // Change the enemy sprite to the randomly generated variant
            spriteLibrary.spriteLibraryAsset = variant; 

        }
    }
}