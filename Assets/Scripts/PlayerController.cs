using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // AudioSource component used to PlayOneShot all the one-time in-game sounds
    AudioSource audiosource;

    public InputAction talkAction;
    public InputAction launchAction;
    public InputAction digAction;
    public GameObject projectilePrefab;
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);


    // variables related to player temporary invincibility
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damgeCooldown;

    // player movement direction
    public InputAction playerMovement;
    Vector2 move;
    Rigidbody2D rigidbody2d;

    // player speed
    public float speed;

    // player health
    public int maxHealth = 5;
    public int health { get{ return currentHealth;}}
    int currentHealth;

    // player alive bool
    bool isDead = false;

    void Start()
    {
        audiosource = GetComponent<AudioSource>();

        
        talkAction.Enable();
        talkAction.performed += FindFriend; // if player executes talkAction with corresponding
                                            // input key, call FindFriend


        launchAction.Enable();
        launchAction.performed += Launch; // if player executes launchAction with corresponding
                                          // input key, call Launch

        digAction.Enable();
        digAction.performed += Dig; // if player executes digAction with corresponding
                                    // input key, call Dig


        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        playerMovement.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == false)
        {
            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                moveDirection.Set(move.x, move.y);
                moveDirection.Normalize();
            }
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (isInvincible)
            {
                damgeCooldown -= Time.deltaTime;
                if (damgeCooldown < 0)
                {
                    isInvincible = false;
                }
            }
            move = playerMovement.ReadValue<Vector2>();
        }
    }
    void FixedUpdate()
    {
        // if player is dead set speed to 0 to prevent movement 
        if (isDead == true)
        {
            speed = 0;
        }
        else
        {
            speed = 3.0f;
            Vector2 newPosition1 = rigidbody2d.position + move * speed * Time.deltaTime;
            rigidbody2d.MovePosition(newPosition1);
        }
    }
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damgeCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        // update player's current health (will get passed -1 for amount)
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        // update GUI health bar to reflect change in player's health
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);

        // Check whether player should die after update of health
        // (player dies when currentHealth = 0)
        if(currentHealth <= 0)
        {
            // kill player
            Die();
        }
    }

    // plays death animation, stops player from moving and removes player's colliders
    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        // stop any current movement
        rigidbody2d.velocity = Vector2.zero;

        // disable player's collider
        GetComponent<Collider2D>().enabled = false;                                                 
    }

    // launches projectile from player
    void Launch(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.25f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
    }

    // activates player animation for digging
    void Dig(InputAction.CallbackContext context)
    {
        animator.SetBool("IsDigging", true);
        StartCoroutine(StopDigging());
    }

    // stops player animation for digging
    IEnumerator StopDigging()
    {
        yield return new WaitForSeconds(0.6f); 
        animator.SetBool("IsDigging", false);
    }
    void FindFriend(InputAction.CallbackContext context)
    {
        // Projects Raycast from player that if collides with NPC opens up dialog
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));

   
        if (hit.collider != null)
        {
            NPC character = hit.collider.GetComponent<NPC>();
            if (character != null)
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }

    // plays sounds one time
    public void PlaySound(AudioClip clip)
    {
        audiosource.PlayOneShot(clip);
    }
}

