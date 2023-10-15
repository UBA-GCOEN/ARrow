using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Corousel : MonoBehaviour
{
    public void onSkipButtonClick()
    {
        PlayerPrefs.SetString("AboutApp", "true");
        SceneManager.LoadScene("Startup(login)");
    }

    public void onGetStartedButtonClick()
    {
        PlayerPrefs.SetString("AboutApp", "true");
        SceneManager.LoadScene("Startup(login)");
    }
}
