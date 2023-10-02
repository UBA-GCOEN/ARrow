using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class RadioButtonController : MonoBehaviour
{
    ToggleGroup toggleGroup;

    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }

    public void onNextClick()
    {
        Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
        string nextSceneName = "";
        if (toggle.name != "Guest")
        {
            nextSceneName = "Ob-" + toggle.name;
        }
        else
        {
            nextSceneName = "Home";
        }

        Debug.Log(nextSceneName);

        SceneManager.LoadScene(nextSceneName);
    }
}