using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    public ParticleSystem smokeEffect;
    AudioSource audioSource;
    bool aggressive = true;

    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;

    // Audio clips and timing
    public AudioClip sound1;
    public AudioClip sound2;
    private float soundTimer = 20.0f; // Timer to count down for sound effect
    private int soundIndex = 0; // Index to cycle between sound1 and sound2

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vertical = false;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        soundTimer -= Time.deltaTime;

        if (!aggressive)
            return;

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
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
        }

        rigidbody2d.MovePosition(position);

        // Check to play sound
        if (soundTimer <= 0)
        {
            if (Random.Range(1, 5) == 1) // 1 in 4 chance
            {
                AudioClip clipToPlay = (soundIndex % 2 == 0) ? sound1 : sound2;
                audioSource.PlayOneShot(clipToPlay);
                soundIndex++;
            }
            soundTimer = 20.0f; // Reset the sound timer
        }
    }

    public void Fix()
    {
        animator.SetTrigger("Fixed");
        aggressive = false;
        audioSource.Stop();
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
