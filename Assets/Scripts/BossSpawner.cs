using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // plays sounds with function PlaySound for boss
    public AudioSource audioSource;

    // boss's spawn sound to play in audiosource
    public AudioClip bossSpawnSound;

    // Assign the specific spawn location in the editor
    public Transform spawnPoint;

    // Assign prefab of boss enemy
    public GameObject bossPrefab; 

    // Gets player's position
    public Transform player;

    // Distance from which the boss can spawn
    public float spawnDistanceThreshold = 5f;

    // Used to check if final treasure chest has been collected
    public EnemySpawner enemySpawner;

    void Update()
    {
        // Check if final chest has been collected and player is close enough to boss's lair
        if (enemySpawner.finalChestCollected && Vector2.Distance(player.position, spawnPoint.position) <= spawnDistanceThreshold)
        {
            StartCoroutine(SpawnBossAfterDelay());
            enabled = false;  // Disables this script after spawning the boss to prevent multiple spawns
        }
    }

    IEnumerator SpawnBossAfterDelay()
    {
        PlaySound(bossSpawnSound);  // Play the boss spawn sound
        yield return new WaitForSeconds(3.0f);  // Wait for 3 seconds

        // Instantiate the boss at the spawn point
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        // Scale the boss sprite up 5 times its normal size
        boss.transform.localScale = new Vector3(4, 4, 4);  
    }

    // Plays the specified sound once
    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
