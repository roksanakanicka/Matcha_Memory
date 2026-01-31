using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEffectPlayer : MonoBehaviour
{
    public ParticleSystem leafSystem;
    public ParticleSystem steamSystem;

    public void PlayEffect(Vector3 position)
    {
        transform.position = position;
        leafSystem.Play();
        steamSystem.Play();
    }
}
