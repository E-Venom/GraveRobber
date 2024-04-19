using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigController : MonoBehaviour
{
    public ParticleSystem digEffect;
    public AudioSource audioSource;    // Reference to the AudioSource component
    public AudioClip diggingSound;     // The digging sound clip

    // Start dig particle system effect and play digging sound
    public void PlayDigEffect()
    {
        if (digEffect != null)
        {
            digEffect.Stop();
            digEffect.Play();
        }

        if (audioSource != null && diggingSound != null)
        {
            audioSource.Stop();       // Stop current audio to play from the start
            audioSource.clip = diggingSound; // Set the clip to play
            audioSource.Play();       // Play the digging sound
        }
    }
}
