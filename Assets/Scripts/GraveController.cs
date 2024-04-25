using System.Collections;
using UnityEngine;

public class GraveController : MonoBehaviour
{   
    // used to designate boss's grave
    public bool bossGrave = false;

    // designate treasure chest game object in inspector
    public GameObject treasureChest;

    // used to change the grave color after it has spawned a treasure chest
    private SpriteRenderer spriteRenderer;

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

    public void Start()
    {
        // initialize sprite renderer to change grave color when treasure chest has been spawned
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        PlayerController player = other.collider.GetComponent<PlayerController>();

        if (player != null && (!bossGrave || (bossGrave && player.bossIsDead)))
        {

            // note: Time.time = time since start of the game
            if (player.isDigging && !hasSpawnedChest && Time.time > lastDigTime + digCooldown)
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

                    // call function to change grave color to designate grave has yielded a chest
                    changeGraveColor(Color.gray);
                }
            }
        }
    }

    // changes grave color after treasure chest is yielded
    public void changeGraveColor(Color newColor)
    {
        if (hasSpawnedChest && spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // grave's color fades out and is then destroyed
    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 3.0f;
        float fadeStep = spriteRenderer.color.a / fadeDuration;

        while (spriteRenderer.color.a > 0)
        {
            Color newColor = spriteRenderer.color;
            newColor.a -= fadeStep * Time.deltaTime;
            spriteRenderer.color = newColor;
            yield return null;
        }

        Destroy(gameObject);
    }
}
