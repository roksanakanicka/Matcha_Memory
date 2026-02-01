using UnityEngine;

public class MatchEffectPlayer : MonoBehaviour
{
    public ParticleSystem leafSystem;
    public ParticleSystem steamSystem;

    public void PlayEffect(Vector3 position)
    {
        // Przesuwamy cały obiekt w miejsce karty
        transform.position = position;

        // Odpalamy oba systemy cząsteczek
        if (leafSystem != null) leafSystem.Play();
        if (steamSystem != null) steamSystem.Play();
    }
}
