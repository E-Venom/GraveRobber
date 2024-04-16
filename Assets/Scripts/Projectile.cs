using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
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
        rigidbody2D.AddForce(direction * force);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Shovel collide with: " + other);
        EnemyController enemy = other.collider.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.Fix();
        }

        Destroy(gameObject);
    }
}
