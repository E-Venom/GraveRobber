using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BossController : MonoBehaviour
{
    // accesses sprite library to get enemy boss sprite 
    public SpriteLibrary spriteLibrary;

    // accesses enemy sprites and enemy animations in sprite library
    // note: boss is sprite index 4 need to scale him up and change his sprite color to 
    // purple or red when he performs his abilities
    public SpriteLibraryAsset[] enemyrefs;

    // used when enemy attacks player
    public Collider2D attackCollider;

    // get's players position
    public Transform player;

    // enemy move speed
    public float moveSpeed = 0.25f;

    // boss enemy health
    public int enemyHealth = 100;

    // range from player to have boss enemy start their melee attack
    public float meleeAttackRange = 1f;

    // timer used for allowing boss to execute magical ranged attack
    public float magicalRangedAttackTimer = 4f;

    // timer used for allowing boss to execute summoning enemies in a circle around player attack
    public float summonsAttackTimer = 8f;

    // boss enemy attack cooldown
    public float attackCooldown = 2f;

    // keeps track of boss enemy's last attack in game time
    private float lastAttackTime;

    // used to stop boss enemy from moving when dead or attacking
    private bool isMovingAllowed = true;

    // gets boss enemy animator info
    private Animator animator;

    // used to check boss enemy instance's alive state
    public bool isDead = false;

    // used for fading boss enemy sprite when enemy dies
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // start attack collider in boss enemy animations as disabled
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
        // if player doesn't exist or boss enemy is dead, or moving isn't allowed return and 
        // go no further
        if (player == null || isDead || !isMovingAllowed) return;

        // if boss enemy is to far from player to attack execute function to move towards player
        // if boss enemy is close enough for attack execute function to attack player
        if (Vector2.Distance(transform.position, player.position) > meleeAttackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            CheckForAttack();
        }
    }

    // Moves boss enemy towards players position execute proper enemy animations for moving
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator.SetFloat("MoveX", Mathf.Sign(direction.x));
        animator.SetBool("isMoving", true);
        animator.SetBool("isIdle", false);
    }

    // Checks if enough time has passed to allow boss enemy to attack player again
    // Using to prevent spamming of boss enemy attacks
    void CheckForAttack()
    {
        if ((Time.time - lastAttackTime) >= attackCooldown)
        {
            Attack();
        }
    }

    // Causes boss enemy to attack player, resets last attack time to current elapsed game time
    // plays proper animations for boss enemy attacking, stops boss enemy from transforming its 
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

        // 5 second delay before allowing boss enemy movement after attacking
        StartCoroutine(AllowMovementAfterDelay(5f));
    }

    IEnumerator AllowMovementAfterDelay(float delay)
    {
        // boss enemy now allowed to move again boss enemy animation changed to moving
        yield return new WaitForSeconds(delay);
        isMovingAllowed = true;
        animator.SetBool("isAttacking", false);
        DisableAttackCollider();
        animator.SetBool("isIdle", false);
        animator.SetBool("isMoving", true);
    }

    // Causes boss enemy to fade out and then be destroyed
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

        // get boss enemy sprites original color to fade
        Color originalColor = spriteRenderer.color;

        // while loop to fade out boss enemy sprite before destroy its instance
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

    // used to set boss enemy animation to idle after rising
    public void OnRiseComplete()
    {
        animator.SetBool("isRising", false);
        animator.SetBool("isIdle", true);
    }

    // calls boss enemy animation event and activates enemy 2D collider used for attack
    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    // calls bos enemy animation event and deactivates enemy 2D collider used for attack
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    // changes health of boss enemy, if boss enemy's health is less than or equal to zero 
    // kills boss enemy
    public void enemyChangeHealth(int changeHealthAmount)
    {
        enemyHealth += changeHealthAmount;
        if (enemyHealth <= 0)
        {
            Die();
        }
    }
}
