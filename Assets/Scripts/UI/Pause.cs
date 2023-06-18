using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject deathMenu;

    [SerializeField]
    GameObject System;

    [SerializeField]
    GameObject Base;

    [SerializeField]
    GameObject countIn;

    [SerializeField]
    GameObject comboHub;

    [SerializeField]
    float moveSpeed;

    [SerializeField]
    Slider master;

    [SerializeField]
    Slider music;

    [SerializeField]
    Slider sfx;

    [SerializeField]
    PlayerController player;

    [SerializeField]
    Spine spine;

    public GameSFX soundfx;

    Coroutine resumeCountdownCoroutine;

    private TMP_Text countInText;

    private Vector3 originalPosition;

    private Image hub;

    public void Start()
    {
        originalPosition = comboHub.transform.position;
        hub = comboHub.GetComponent<Image>();

        master.value = PlayerPrefs.GetFloat("master_volume");
        music.value = PlayerPrefs.GetFloat("music_volume");
        sfx.value = PlayerPrefs.GetFloat("sfx_volume");
    }

    void Update()
    {
        if (
            SceneManager.GetActiveScene().name == "Game" &&
            Input.GetKeyDown(KeyCode.Escape)
        )
        {
            toPause();
        }

        if(player.curHealth <= 0 && !player.isDead)
        {
            toDeath();
            player.isDead = true;
            player.switching = true;
        }
    }

    public void toPause()
    {
        pauseMenu.SetActive(true);
        System.SetActive(false);
        AudioListener.pause = true;
        hub.enabled = false;
        Time.timeScale = 0f;
        StopResumeCountdownCoroutine(); // Stop the coroutine if it's already running

        // Add the following code to move the comboHub down
        if (comboHub.activeSelf)
        {
            StartCoroutine(MoveComboHubCoroutine(originalPosition,
            new Vector3(originalPosition.x,
                originalPosition.y - 860.0f,
                originalPosition.z)));
        }
    }

    public void toResume()
    {
        pauseMenu.SetActive(false);
        countIn.SetActive(true);
        hub.enabled = true;
        countInText = countIn.GetComponent<TMP_Text>();
        resumeCountdownCoroutine = StartCoroutine(ResumeCountdownCoroutine()); // Start the coroutine

        // Add the following code to move the comboHub up
        if (comboHub.activeSelf)
        {
            StartCoroutine(MoveComboHubCoroutine(new Vector3(originalPosition.x,
                originalPosition.y - 100f,
                originalPosition.z),
            originalPosition));
        }
    }

    public void toSystem()
    {
        Base.SetActive(false);
        System.SetActive(true);
    }

    public void toBase()
    {
        System.SetActive(false);
        Base.SetActive(true);
    }

    public void toDeath()
    {
        deathMenu.SetActive(true);
        spine.GameOverStart();
        player.anim.SetTrigger("die");
        player.anim2.SetTrigger("die");
        player.anim3.SetTrigger("die");
        player.anim3.SetBool("isDead", true);
    }

    public void toRestart()
    {
        SceneManager.LoadScene("Game");
    }

    IEnumerator ResumeCountdownCoroutine()
    {
        const float countdownTime = 3.0f; // Countdown time in seconds
        float remainingTime = countdownTime;

        // Set the initial scale of the text to 0
        Vector3 initialScale = countInText.transform.localScale;
        countInText.transform.localScale = Vector3.zero;

        while (remainingTime > 0.5f)
        {
            // Calculate the scale of the text based on the remaining time
            float scale =
                1f + (0.8f * (countdownTime - remainingTime)) / countdownTime;
            countInText.transform.localScale = new Vector3(scale, scale, scale);

            string prev_count = countInText.text;

            // Check if the remaining time has changed since the last frame
            if (
                Mathf.FloorToInt(remainingTime + 0.48f) !=
                Mathf
                    .FloorToInt(remainingTime +
                    0.48f -
                    Time.unscaledDeltaTime) &&
                remainingTime > 1.0f
            )
            {
                // If the remaining time has changed, reset the scale of the text
                countInText.transform.localScale = initialScale;
            }

            countInText.text = remainingTime.ToString("F0");

/*            if (!string.Equals(prev_count, countInText.text))
            {
                if (countInText.text != "1")
                {
                    soundfx.Playcount();
                }
                else
                {
                    soundfx.Playcount_done();
                }
            }*/
            

            yield return new WaitForSecondsRealtime(0.1f); // Update the remaining time every 0.1 seconds
            remainingTime -= 0.1f;
        }

        countIn.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    IEnumerator
    MoveComboHubCoroutine(Vector3 startPosition, Vector3 endPosition)
    {
        float t = 0f;
        while (t < 1f)
        {
            comboHub.transform.position =
                Vector3.Lerp(startPosition, endPosition, t);
            t += Time.unscaledDeltaTime * moveSpeed;
            yield return null;
        }
        comboHub.transform.position = endPosition;
    }

    void StopResumeCountdownCoroutine()
    {
        if (resumeCountdownCoroutine != null)
        {
            StopCoroutine (resumeCountdownCoroutine);
        }
    }

    public void toStart(int id)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(id);
    }
}
