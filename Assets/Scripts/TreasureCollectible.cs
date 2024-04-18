using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCollectible : MonoBehaviour
{
    // make selection of this audio clip in the inspector window
    public AudioClip collectedClip;

    // Built in Unity function that is called on in the first frame when the physics system detects a
    // GameObject with a Rigidbody component hitting the GameObject collider that is a trigger. 
    // in other words it "triggers" when other (player) "enters" the health collectible collision box
    void OnTriggerEnter2D(Collider2D other)
    {
        // get player component
        PlayerController controller = other.GetComponent<PlayerController>();

        // get treasure chest component to check chestCollectTimer
        TreasureChest treasureChest = GetComponent<TreasureChest>();

        if (controller != null && treasureChest.chestCollectTimer <= 0)
        {
            // call function in PlayerController to update gold collected and UI
            controller.ChangeTreasure(1);
            
            // play sound effect
            controller.PlaySound(collectedClip);

            // destroy treasure chest
            Destroy(gameObject);

        }
    }
}
