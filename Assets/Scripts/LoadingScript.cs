using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScript : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    // Adjust this value to control the loading duration (in seconds)
    public float loadingDuration = 2.0f;

    void Start()
    {
        StartCoroutine(SimulateLoading());
    }

    IEnumerator SimulateLoading()
    {
        float startTime = Time.time;
        float endTime = startTime + loadingDuration;

        while (Time.time < endTime)
        {
            // Calculate the progress as a ratio of time
            float progress = Mathf.Clamp01((Time.time - startTime) / loadingDuration);

            // Update the loading bar and progress text
            loadingBar.value = progress;
            progressText.text = Mathf.Round(progress * 100f) + "%";

            yield return null;
        }

        // Authentication
        string token = PlayerPrefs.GetString("Token");
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));

        if (token != "" && user != null)
        {
            if (user.isOnboarded)
            {
                SceneManager.LoadScene("Home");
            }
            else
            {
                SceneManager.LoadScene("[Ob]Start");
            }
        }

        else
        {
            PlayerPrefs.DeleteKey("Token");
            SceneManager.LoadScene("Startup(login)");
        }
    }
}
