using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioSource music;

    public AudioClip backgroundMusic;

    private void Start()
    {
        music.clip = backgroundMusic;
        music.Play();
    }

}
