using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign in inspector
    public float spawnInterval = 2f; // Time between spawns
    public Transform playerTransform; // Assign player transform in inspector
    public LayerMask mapLayer; // Assign map layer to avoid spawning on map objects

    private float timer;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Assuming you use the main camera
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            SpawnEnemy();
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        bool validPosition = false;
        Vector2 spawnPosition = Vector2.zero;
        int attempts = 0;

        while (!validPosition && attempts < 100) // Prevents infinite loops
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

        if (validPosition)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
