using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    // designate the specific audio clip in the inspector window
    public AudioClip collectedClip;

    // Built in Unity function that is called on in the first frame when the physics system detects a
    // GameObject with a Rigidbody component hitting the GameObject collider that is a trigger. 
    // in other words it "triggers" when other (player) "enters" the health collectible collision box
    void OnTriggerEnter2D(Collider2D other)
    {
        // get player component
        PlayerController player = other.GetComponent<PlayerController>();

        // if player exists and their health is less than their maximum health
        if (player != null && player.health < player.maxHealth)
        {
            // change player's health by 1
            player.ChangeHealth(1);

            // play sound effect
            player.PlaySound(collectedClip);

            // destroy health collectible
            Destroy(gameObject);

        }
    }
}

