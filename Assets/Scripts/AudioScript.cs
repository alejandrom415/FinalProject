using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    public AudioClip backgroundSound;

    public Text gameoverText;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
            audioSource.clip = backgroundSound;
            audioSource.Play();
            audioSource.loop = true;
    }
        
    void Update()
    {
        if (gameoverText.text == "You Win! - A Game by Alejandro Morales Press R to Restart")
        {
            audioSource.Stop();
        }

        if (gameoverText.text == "You Lose! - Press R to Restart or ESC to Quit")
        {
            audioSource.Stop();
        }
    }
}
