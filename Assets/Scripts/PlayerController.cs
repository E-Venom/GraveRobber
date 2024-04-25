using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // AudioSource component used to PlayOneShot all the one-time in-game sounds
    AudioSource audiosource;
    public AudioClip impact01;
    public AudioClip impact02;
    public AudioClip impact03;
    public AudioClip damageTaken;
    public AudioClip dig01;
    public AudioClip dig02;
    public AudioClip quip01;
    public AudioClip quip02;
    public AudioClip quip03;
    public AudioClip quip04;

    public AudioClip deathClip;

    private bool lastPlayedDig01 = true; // Flag to track the last played sound

    // used to designate input to have player "talk" to NPC's with Raycast
    public InputAction talkAction;

    // used to check if boss has died
    public bool bossIsDead = false;

    // used to designate input to have player launch shovel
    public InputAction launchAction;

    // used to designate input to have player dig graves
    public InputAction digAction;

    // used to designate input to have player melee attack 
    public InputAction meleeAction;

    // used to get shovel prefab for ranged attack
    public GameObject projectilePrefab;

    // used for activating player's colliders when they melee attack to their left or right
    public Collider2D meleeLeftCollider;
    public Collider2D meleeRightCollider;

    // used to initialize game UI to show how many chests player has collected
    public float treasureChestsCollected = 0.0f;

    // used to update treasure chest game UI to let player know how many treasure chests have been collected
    public float numRegTreasureChestLevel1 = 31.0f;
    public float numRegTreasureChestLevel2 = 15.0f;

    // used to check if final chest is collected, if true, gets passed to Enemy Spawner to 
    // stop regular enemy spawning
    public bool finalChestCollected = false;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);


    // variables related to player temporary invincibility
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damgeCooldown;
    
    // timer to prevent spamming of shooting shovel
    public float shootCooldownTimer = 1.0f;

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
    public bool isDead = false;

    // player not digging
    public bool isDigging = false;

    // player not fighting w/melee
    public bool isMelee = false;

    void Start()
    {
        // start attack collider in enemy animations as disabled
        if (meleeLeftCollider != null)
            meleeLeftCollider.enabled = false;

        // start attack collider in enemy animations as disabled
        if (meleeRightCollider != null)
            meleeRightCollider.enabled = false;

        // AudioSource component used to PlayOneShot, all the one-time in-game sounds
        audiosource = GetComponent<AudioSource>();

        talkAction.Enable();
        talkAction.performed += FindFriend; // if player executes talkAction with corresponding
                                            // input key, call FindFriend()


        launchAction.Enable();
        launchAction.performed += Launch; // if player executes launchAction with corresponding
                                          // input key, call Launch()

        digAction.Enable();
        digAction.performed += Dig; // if player executes digAction with corresponding
                                    // input key, call Dig()

        meleeAction.Enable();
        meleeAction.performed += Melee; // if player executes meleeAction with corresponding
                                        // input key, call Melee()


        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        playerMovement.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {   
        // deduct the time since last frame from the cooldown timer
        if (shootCooldownTimer > 0)
        {
            shootCooldownTimer -= Time.deltaTime;
        }

        // if player isn't dead go about moving them
        if (isDead == false)
        {
            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                moveDirection.Set(move.x, move.y);
                moveDirection.Normalize();
            }

            // set values of parameters in animator to create the proper animation
            animator.SetFloat("Look X", moveDirection.x);
            animator.SetFloat("Look Y", moveDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            // if player state is temporarily invincible after being hit by enemies 
            if (isInvincible)
            {
                // reduce the cooldown 
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
        // otherwise move player normally
        else
        {
            speed = 3.0f;
            Vector2 newPosition1 = rigidbody2d.position + move * speed * Time.deltaTime;
            rigidbody2d.MovePosition(newPosition1);
        }
    }
    // updates player's currentHealth as well as player health gui to reflect player's 
    // currentHealth, kills player if player's currentHealth is 0
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
        AudioManagerScript.Instance.PlaySound(damageTaken);
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
            AudioManagerScript.Instance.PlaySound(deathClip);
            // kill player
            Die();
        }
    }

    public void ChangeTreasure(int amount)
    {
        treasureChestsCollected += amount;

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            // update GUI health bar to reflect change in player's collected treasure on level 1
            UIHandler.instance.SetGoldValue(treasureChestsCollected / numRegTreasureChestLevel1);

            // checks if all treasure chests have been collected
            if (treasureChestsCollected / numRegTreasureChestLevel1 >= 1)
            {
                finalChestCollected = true;
            }
        }
        else
        {
            // update GUI health bar to reflect change in player's collected treasure on level 2
            UIHandler.instance.SetGoldValue(treasureChestsCollected / numRegTreasureChestLevel2);

            // checks if all treasure chests have been collected
            if (treasureChestsCollected / numRegTreasureChestLevel2 >= 1)
            {
                finalChestCollected = true;
            }
        }
    }

    // plays death animation, stops player from moving and removes player's colliders
    public void Die()
    {
        GameObject gameOverScreen = GameObject.Find("Panel"); // Assuming your panel is named "Panel"
        gameOverScreen.SetActive(true);
        isDead = true;
        animator.SetTrigger("Die");

        // stop any current movement
        rigidbody2d.velocity = Vector2.zero;

        // disable player's collider
        GetComponent<Collider2D>().enabled = false;                                                 
    }

    // Launches shovel projectile in direction of player's movement
    void Launch(InputAction.CallbackContext context)
    {
        if (shootCooldownTimer <= 0)
        {
            // Instantiate the projectile at a position slightly above the player
            Vector2 spawnOffset = Vector2.up * 0.25f;
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + spawnOffset, Quaternion.identity);

            // Calculate rotation based on the move direction
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            // Since the sprite faces left by default and right is considered 0 degrees,
            // we need to adjust the angle by adding 180 degrees to flip it horizontally.
            Quaternion rotation = Quaternion.Euler(0, 0, angle + 180);

            // Set projectile rotation
            projectileObject.transform.rotation = rotation;

            // Launch the projectile
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(moveDirection, 300);
            animator.SetTrigger("Launch");

            // reset shootCooldownTimer
            shootCooldownTimer = 1.0f; 
        }
        else
            return;
    }

    // when enemy attacks player, enemy collider is activated in an animations event
    // the collider causes a collision with player's collider, on collision
    // health is taken off of player
    void OnCollisionEnter2D(Collision2D other)
    {

        // gets player object that enemy collided with
        EnemySeeker enemy = other.gameObject.GetComponent<EnemySeeker>();

        // if player object exists
        if (enemy != null && !meleeLeftCollider.enabled && !meleeRightCollider.enabled)
        {
            // call function that takes health off of player
            ChangeHealth(-1);
        }
    }

    public void Dig(InputAction.CallbackContext context)
    {
        if (context.performed) // Ensure the action is only triggered once per input event
        {
            isDigging = true;
            animator.SetBool("IsDigging", true);
            PlayNextDigSound();
            StartCoroutine(StopDigging());
        }
    }

    private void PlayNextDigSound()
    {
        if (lastPlayedDig01)
        {
            AudioManagerScript.Instance.PlaySound(dig02);
            lastPlayedDig01 = false;
        }
        else
        {
            AudioManagerScript.Instance.PlaySound(dig01);
            lastPlayedDig01 = true;
        }
    }
    // stops player animation for digging
    IEnumerator StopDigging()
    {
        // waits for 0.6 seconds till digging animation stops then sets isDiggin bool to false
        yield return new WaitForSeconds(0.6f);
        isDigging = false;
        animator.SetBool("IsDigging", false);
    }

public void Melee(InputAction.CallbackContext context)
{
    if (context.performed)  // Check to ensure this only triggers once per input
    {
        isMelee = true;
        animator.SetBool("IsMelee", true);

        // Get the current value of the "Look X" parameter from the animator
        float lookX = animator.GetFloat("Look X");

        if (lookX < -0.1f)
        {
            // If looking left, enable the left attack collider and disable the right one
            meleeLeftCollider.enabled = true;
            meleeRightCollider.enabled = false;
        }
        else if (lookX > 0.1f)
        {
            // If looking right, enable the right attack collider and disable the left one
            meleeRightCollider.enabled = true;
            meleeLeftCollider.enabled = false;
        }

        PlayRandomQuip();
        StartCoroutine(StopMelee());
    }
}

private void PlayRandomQuip()
{
    AudioClip[] quips = new AudioClip[] { quip01, quip02, quip03, quip04 };
    int randomIndex = UnityEngine.Random.Range(0, quips.Length);
    AudioManagerScript.Instance.PlaySound(quips[randomIndex]);
}


    IEnumerator StopMelee()
    {
        // waits 0.1 seconds till Melee animation ends then sets isMelee to false
        // and disables both colliders
        yield return new WaitForSeconds(0.1f);
        isMelee = false;
        animator.SetBool("IsMelee", false);
        meleeLeftCollider.enabled = false;
        meleeRightCollider.enabled = false;
    }

    // when player attacks enemy with melee
void OnTriggerEnter2D(Collider2D other)
{
    // Get enemy component that collided with player's melee collider
    EnemySeeker enemy = other.GetComponent<EnemySeeker>();
    BossController boss = other.GetComponent<BossController>();

    // If enemy exists and left or right melee colliders are enabled
    if ((enemy || boss) && (meleeLeftCollider.enabled || meleeRightCollider.enabled))
    {
        if (enemy)
        {
            // Apply damage to the enemy
            enemy.enemyChangeHealth(-5);
        }
        if (boss)
        {
            // Apply damage to the enemy
            boss.enemyChangeHealth(-5);
        }

        // Play a random impact sound
        PlayRandomImpactSound();
    }
}

private void PlayRandomImpactSound()
{
    AudioClip[] impacts = new AudioClip[] { impact01, impact02, impact03 };
    int randomIndex = UnityEngine.Random.Range(0, impacts.Length);
    AudioManagerScript.Instance.PlaySound(impacts[randomIndex]);
}


    // player displays npc dialogue  
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

    // Called when the BossController is enabled
    void OnEnable()
    {
        BossController.bossDied += OnBossDied;
    }

    // Called when the BossController is disabled
    void OnDisable()
    {
        BossController.bossDied -= OnBossDied;
    }

    // activated if boss is found to have died 
    private void OnBossDied()
    {
        // boss dying triggers this
        bossIsDead = true;
    }

    // plays sounds one time
    public void PlaySound(AudioClip clip)
    {
        audiosource.PlayOneShot(clip);
    }

    
}

