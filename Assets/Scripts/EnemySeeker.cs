using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeeker : MonoBehaviour
{
    // used when enemy attacks player
    public Collider2D attackCollider;

    public AudioClip sound1;
    public AudioClip sound2;
    private AudioSource audioSource;
    private float soundTimer = 10.0f;  // Timer to track when to potentially play sound


    // get's players position
    public Transform player;

    // enemy move speed
    public float moveSpeed = 0.25f;

    // regular enemy health
    public int enemyHealth = 2;
    
    // range from player to have enemy start their attack
    public float attackRange = 1f;

    // enemy attack cooldown
    public float attackCooldown = 2f;

    // keeps track of enemies last attack in game time
    private float lastAttackTime;

    // used to stop enemy from moving when dead or attacking
    private bool isMovingAllowed = true; 

    // gets enemy animator info
    private Animator animator;  

    // used to check enemy instance's alive state
    public bool isDead = false;
    
    // used for fading enemy sprite when enemy dies
    private SpriteRenderer spriteRenderer;

void Start()
{
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }

    if (attackCollider != null)
        attackCollider.enabled = false;

    animator = GetComponent<Animator>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    if (player == null)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    animator.SetBool("isRising", true);
}


void Update()
{
    if (player == null || isDead || !isMovingAllowed) return;

    if (Vector2.Distance(transform.position, player.position) > attackRange)
    {
        MoveTowardsPlayer();
    }
    else
    {
        CheckForAttack();
    }

    // Sound effect logic
    soundTimer -= Time.deltaTime;
    if (soundTimer <= 0)
    {
        if (Random.Range(1, 5) == 1) // 1 in 4 chance to play a sound
        {
            AudioClip clipToPlay = (Random.Range(0, 2) == 0) ? sound1 : sound2;
            audioSource.PlayOneShot(clipToPlay);
        }
        soundTimer = 20.0f; // Reset the timer
    }
}


    // Moves enemy towards players position execute proper enemy animations for moving
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator.SetFloat("MoveX", Mathf.Sign(direction.x));
        animator.SetBool("isMoving", true);
        animator.SetBool("isIdle", false);
    }

    // Checks if enough time has passed to allow enemy to attack player again
    // Using to prevent spamming of enemy attacks
    void CheckForAttack()
    {
        if ((Time.time - lastAttackTime) >= attackCooldown)
        {
            Attack();
        }
    }

    // Causes enemy to attack player, resets last attack time to current elapsed game time
    // plays proper animations for enemy attacking, stops enemy from transforming its 
    // position to the player
    void Attack()
    {
        animator.SetBool("isAttacking", true);
        EnableAttackCollider();
        lastAttackTime = Time.time;
        animator.SetBool("isMoving", false);
        animator.SetBool("isIdle", true);

        // Stop moving and start a delay before moving again
        isMovingAllowed = false;

        // 5 second delay before allowing enemy movement after attacking
        StartCoroutine(AllowMovementAfterDelay(5f));  
    }

    IEnumerator AllowMovementAfterDelay(float delay)
    {
        // enemy now allowed to move again enemy animation changed to moving
        yield return new WaitForSeconds(delay);
        isMovingAllowed = true;
        animator.SetBool("isAttacking", false);
        DisableAttackCollider();
        animator.SetBool("isIdle", false);
        animator.SetBool("isMoving", true);
    }

    // Causes enemy to fade out and then be destroyed
    public void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", false);
        isMovingAllowed = false;

        // Disable all colliders on this enemy
        foreach (var collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }

        // Coroutine to fade out and destroy
        StartCoroutine(FadeOutAndDestroy(5f));  
    }

    IEnumerator FadeOutAndDestroy(float duration)
    {
        float elapsed = 0;

        // get enemy sprites original color to fade
        Color originalColor = spriteRenderer.color;

        // while loop to fade out enemy sprite before destroy its instance
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        Destroy(gameObject);
    }

    // used to set enemy animation to idle after rising
    public void OnRiseComplete()
    {
        animator.SetBool("isRising", false);
        animator.SetBool("isIdle", true);
    }

    // calls enemy animation event and activates enemy 2D collider used for attack
    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    // calls enemy animation event and deactivates enemy 2D collider used for attack
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    public void enemyChangeHealth(int changeHealthAmount)
    {
        enemyHealth += changeHealthAmount;
        if (enemyHealth <= 0)
        {
            Die();
        }
    }
}