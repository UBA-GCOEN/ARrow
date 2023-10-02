using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public string backScene;
    
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
