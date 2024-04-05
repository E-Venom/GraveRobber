using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigController : MonoBehaviour
{
    public ParticleSystem digEffect; 

    // start dig particle system effect
    public void PlayDigEffect()
    {
        if (digEffect != null)
        {
            digEffect.Stop();
            digEffect.Play();
        }
    }

}
