using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeeker : MonoBehaviour
{
    // get's players position
    public Transform player;

    // enemy move speed
    public float moveSpeed = 0.25f;
    
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
    private bool isDead = false;
    
    // used for fading enemy sprite when enemy dies
    private SpriteRenderer spriteRenderer;

    void Start()
    {
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
        // if player doesn't exist or enemy is dead, or moving isn't allowed return and 
        // go no further
        if (player == null || isDead || !isMovingAllowed) return;

        // if enemy is to far from player to attack execute function to move towards player
        // if enemy is close enough for attack execute function to attack player
        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            CheckForAttack();
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

        StartCoroutine(FadeOutAndDestroy(5f));  // Coroutine to fade out and destroy
    }

    IEnumerator FadeOutAndDestroy(float duration)
    {
        float elapsed = 0;
        Color originalColor = spriteRenderer.color;

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
}