using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        TreasureChest treasureChest = GetComponent<TreasureChest>();

        if (controller != null)
        {
            // boss chest logic
            if (treasureChest.isBossChest)
            {
                // Disable all colliders on the boss chest
                foreach (Collider2D collider in GetComponents<Collider2D > ())
                {
                    collider.enabled = false;
                }
                // Make the chest invisible
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }

                // play boss chest sound
                controller.PlaySound(collectedClip);

                // Start the coroutine to wait before loading the scene
                StartCoroutine(WaitAndLoadScene("Level2"));
            }
            // regular chest logic
            else if (treasureChest.chestCollectTimer <= 0)
            {
                // Regular chest collection logic
                controller.ChangeTreasure(1);
                controller.PlaySound(collectedClip);
                Destroy(gameObject);
            }
        }
    }

    // waits 2 seconds before loading Scene 2
    IEnumerator WaitAndLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2); // Wait for two seconds
        SceneManager.LoadScene(sceneName); // Load the next level
    }
}