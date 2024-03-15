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

    void Start()
    {
        audiosource = GetComponent<AudioSource>();

        // if player executes talkAction, call FindFriend
        talkAction.performed += FindFriend;

        talkAction.Enable();
        launchAction.Enable();
        launchAction.performed += Launch;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        playerMovement.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
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
        UnityEngine.Debug.Log(move);
    }
    void FixedUpdate()
    {
        speed = 3.0f;
        Vector2 newPosition1 = rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(newPosition1);
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
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth); 
    }
    void Launch(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
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

