using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public string backScene;
    private void Start()
    {
        // if (IsUserAuthenticated())
        // {
        //     if (IsUserOnboarded())
        //     {
        //         SceneManager.LoadScene("Home");
        //     }
        //     else
        //     {
        //         SceneManager.LoadScene("[Ob]Start");
        //     }
        // }
        // else
        // {
        //     SceneManager.LoadScene("Startup(login)");
        // }
    }

    void Update()
    {
        // Check for the Android back button press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the previous scene
            SceneManager.LoadScene(backScene);
        }
    }
    public void SceneName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
