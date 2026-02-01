using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton - pozwala na dostęp przez AudioManager.instance
    public static AudioManager instance;

    public AudioSource sfxSource;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip clickSound;

    void Awake()
    {
        // Sprawdzamy, czy instancja już istnieje
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Zapobiega dublowaniu managera
        }
    }

    public void PlayFlip() => sfxSource.PlayOneShot(flipSound);
    public void PlayMatch() => sfxSource.PlayOneShot(matchSound);
    public void PlayClick() => sfxSource.PlayOneShot(clickSound);
}

