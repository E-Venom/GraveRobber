using System.Collections;
using UnityEngine;

public class GraveController : MonoBehaviour
{
    public GameObject treasureChest;

    // time in seconds between digs
    public float digCooldown = 0.5f;  

    // amount of digs needed to spawn treasure chest
    private int digsRequired = 5;

    // initial starting digs
    private int currentDigs = 0;

    // bool used to allow for only one spawn of a treasure chest per grave
    private bool hasSpawnedChest = false;

    // used with digCooldown to space out allowable digging by player on graves
    private float lastDigTime;

    private void OnCollisionStay2D(Collision2D other)
    {
        PlayerController player = other.collider.GetComponent<PlayerController>();
        // note: Time.time = time since start of the game
        if (player != null && player.isDigging && !hasSpawnedChest && Time.time > lastDigTime + digCooldown)
        {
            // update lastDigTime to current time in game that has passed since its start
            lastDigTime = Time.time;

            // add one to current digs
            currentDigs++;

            // create treasure chest collectible if currentDigs == digsRequired on grave
            if (currentDigs >= digsRequired)
            {
                Instantiate(treasureChest, transform.position, Quaternion.identity);

                // update grave bool hasSpawnedChest to true so no more chests can be spawned
                // at this grave
                hasSpawnedChest = true;
                Debug.Log("Treasure Chest spawned");
            }
        }
    }
}