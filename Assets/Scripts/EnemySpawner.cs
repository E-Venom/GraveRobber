using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnemySpawner : MonoBehaviour
{
    // Assign the prefab of the enemy to spawn
    public GameObject enemyPrefab; 

    // Time interval between spawns
    public float spawnInterval = 5f;

    // Transform of the player, used to calculate distance for spawn
    public Transform playerTransform;

    // Layer mask to ensure enemies do not spawn in non-playable areas
    public LayerMask mapLayer;

    // Bool to control whether spawning should stop (true if final chest is collected)
    public bool finalChestCollected;

    // Timer to manage spawn interval
    private float timer;

    // Reference to the main camera, used to determine spawn positions within the viewport
    private Camera mainCamera;

    // Reference to the sprite library for accessing different enemy sprites
    public SpriteLibrary spriteLibrary;

    // Array of sprite library assets to allow for different enemy variants
    public SpriteLibraryAsset[] enemyRefs;

    // Indicates if the enemy is dead
    public bool isDead = false;

    // Reference to the PlayerController component on the player GameObject
    private PlayerController player;

    private void Start()
    {
        // Use the current time to set the seed for random for greater unpredictability
        int seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);

        mainCamera = Camera.main; // Get reference to the main camera

        // Find the player GameObject by the tag "Player" and retrieve the PlayerController component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<PlayerController>();
            if (player == null)
            {
                Debug.LogError("PlayerController component not found on the player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found, please check the tag.");
        }
    }

    private void Update()
    {
        // Check if the player controller is successfully retrieved
        if (player == null)
        {
            // Stop execution if no player controller is found
            return; 
        }

        // Update the local bool based on the player's finalChestCollected status
        finalChestCollected = player.finalChestCollected;

        // Continue to spawn enemies only if the final chest has not been collected
        if (!finalChestCollected)
        {
            timer += Time.deltaTime; // Increment timer by the elapsed time since last frame
            if (timer > spawnInterval) // Check if enough time has passed to spawn another enemy
            {
                SpawnEnemy(); // Spawn an enemy
                timer = 0; // Reset the timer
            }
        }
    }

    // Method to spawn enemies at random locations within the camera's viewport
    void SpawnEnemy()
    {
        bool validPosition = false; // Flag to check if the found position is valid for spawning
        Vector2 spawnPosition = Vector2.zero; // Initialize spawn position
        int attempts = 0; // Counter to limit the number of spawn attempts

        // Try to find a valid spawn position, limiting to 100 attempts to prevent infinite loops
        while (!validPosition && attempts < 100)
        {
            Vector2 viewPortPosition = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(viewPortPosition.x, viewPortPosition.y, mainCamera.nearClipPlane));

            // Ensure the spawn position is at a sufficient distance from the player
            if (Vector2.Distance(spawnPosition, playerTransform.position) >= 4)
            {
                // Check if the position does not collide with map objects
                if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, mapLayer))
                {
                    validPosition = true; // Mark position as valid
                }
            }

            attempts++; // Increment attempts counter
        }

        // Instantiate the enemy at the valid position if found
        if (validPosition)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            RandomizeEnemySprite(enemy); // Randomize the enemy's sprite from available variants
        }
    }

    // Method to apply a random sprite to the spawned enemy
    void RandomizeEnemySprite(GameObject enemy)
    {
        var spriteLibrary = enemy.GetComponent<SpriteLibrary>(); // Get the SpriteLibrary component from the enemy
        if (spriteLibrary != null)
        {
            SpriteLibraryAsset variant = enemyRefs[Random.Range(0, enemyRefs.Length)];
            spriteLibrary.spriteLibraryAsset = variant; // Assign the random variant to the enemy
        }
    }

    // allows for spawn enabling even if final treasure chest is collected
    public void EnableSpawning(bool enable)
    {
        finalChestCollected = !enable;  // Enable if true, disable if false
    }

    // spawns enemies at the exact position given
    public void SpawnEnemyAtPosition(Vector2 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        RandomizeEnemySprite(enemy);
    }
}