using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NotifButton : MonoBehaviour
{
    public GameObject Panel;
public void displayPanel()
    {
        if (Panel.activeInHierarchy == false)
        {
            Panel.SetActive(true);
        }
        else
        {
            Panel.SetActive(false);
        }
    }
public void nextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
