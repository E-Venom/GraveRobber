using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    // variable to control smoke emitting from enemy
    public ParticleSystem smokeEffect;

    // variable to control sound of robot walking
    AudioSource audioSource;
    bool aggressive = true;

    // Public variables
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    // Private variables
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vertical = false;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

    }


    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (!aggressive)
        {
            return;
        }

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
            UnityEngine.Debug.Log(direction);
        }

        Vector2 position = rigidbody2d.position;
        

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("MoveX", 0); // Set Move X to 0 when moving vertically
            animator.SetFloat("MoveY", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }

        rigidbody2d.MovePosition(position);
    }
    public void Fix()
    {
        animator.SetTrigger("Fixed");
        aggressive = false;

        // stops audio of robot walking
        audioSource.Stop();
        
        // stops smoke emitting from robot
        smokeEffect.Stop();

        rigidbody2d.simulated = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}