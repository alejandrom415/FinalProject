using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    float timerDisplay;
    public Text gameoverText;
    public Text progressText;
    public AudioClip activateSound;
    AudioSource audioSource;

    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);

        audioSource.clip = activateSound;
        audioSource.Play();
    }
}