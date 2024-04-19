using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;

    // Adjust this to match the maximum allowed distance from the game world's center
    public float boundaryLimit = 100.0f;

    // Optional: Adjust this if your game world center is not at Vector3.zero
    public Vector3 gameWorldCenter = Vector3.zero;

    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        Debug.Log("Projectile Awake - Rigidbody2D component acquired.");
    }

    void Update()
    {
        // Using gameWorldCenter to check the distance from a specific point
        if ((transform.position - gameWorldCenter).magnitude > boundaryLimit)
        {
            Debug.Log("Projectile Update - Exceeded boundary limits, destroying projectile.");
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        // Visual debug to show the force direction and magnitude
        Debug.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y, 0) * 10, Color.red, 5f);
        Debug.Log($"Launching projectile with direction {direction} and force {force}.");

        myRigidbody2D.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"Projectile OnCollisionEnter2D - Collided with: {other.gameObject.name}");

        BossController boss = other.collider.GetComponent<BossController>();

        EnemySeeker enemy = other.collider.GetComponent<EnemySeeker>();

        if (enemy != null)
        {
            Debug.Log("Projectile Collision - Enemy hit.");
            if (!enemy.isDead)
            {
                enemy.enemyChangeHealth(-1);
                Debug.Log($"Projectile Collision - Enemy health changed. New health: {enemy.enemyHealth}");

                // If enemy health <= zero, kill enemy
                if (enemy.enemyHealth <= 0)
                {
                    Debug.Log("Projectile Collision - Enemy health is zero or less, calling Die.");
                    enemy.Die();
                }
            }
        }
        if (boss != null)
        {
            Debug.Log("Projectile Collision - Enemy hit.");
            if (!boss.isDead)
            {
                boss.enemyChangeHealth(-1);
                Debug.Log($"Projectile Collision - Enemy health changed. New health: {boss.enemyHealth}");

                // If enemy health <= zero, kill enemy
                if (boss.enemyHealth <= 0)
                {
                    Debug.Log("Projectile Collision - Enemy health is zero or less, calling Die.");
                    boss.Die();
                }
            }
        }
        // Destroy projectile after collision
        Destroy(gameObject);
    }
}
