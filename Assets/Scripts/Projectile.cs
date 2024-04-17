using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;

    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // destroy projectile if moved beyond 100.0f from position = center of the game world
    void Update()
    {
        if(transform.position.magnitude > 50.0f)
        {
            Destroy(gameObject);
        }
    }
    public void Launch(Vector2 direction, float force)
    {
        myRigidbody2D.AddForce(direction * force);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Projectile collided with: " + other.gameObject.name);
        EnemySeeker enemy = other.collider.GetComponent<EnemySeeker>();

        if (enemy != null)
        {
            if (!enemy.isDead)
            {
                enemy.enemyChangeHealth(-1);
                // if enemy health <= zero, kill enemy
                if (enemy.enemyHealth <= 0)  

                {
                    enemy.Die();
                }
            }
        }
        // destroy projectile
        Destroy(gameObject);  

    }
}
