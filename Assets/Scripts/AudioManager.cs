using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class AudioManager : MonoBehaviour
    {
        public AudioSource sfxSource;
        public AudioClip flipSound;
        public AudioClip matchSound;
        public AudioClip clickSound;

        public void PlayFlip() => sfxSource.PlayOneShot(flipSound);
        public void PlayMatch() => sfxSource.PlayOneShot(matchSound);
        public void PlayClick() => sfxSource.PlayOneShot(clickSound);
    }

