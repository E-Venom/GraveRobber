using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    // Built in Unity function that is called on in the first frame when the physics system detects a
    // GameObject with a Rigidbody component hitting the GameObject collider that is a trigger. 
    // in other words it "triggers" when other (player) "enters" the health collectible collision box
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            controller.ChangeHealth(1);
            controller.PlaySound(collectedClip);
            Destroy(gameObject);

        }
    }
}

