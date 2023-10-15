using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
=======
using UnityEngine.UI;
using UnityEngine.Networking;
>>>>>>> b4a9549a27897d714b29ec849db04eba3daab3d8
using TMPro;

public class FetchUserDetails : MonoBehaviour
{
    public TextMeshProUGUI username;
<<<<<<< HEAD
=======
    public TextMeshProUGUI branchYear;
    public Button AdminPanel;
>>>>>>> b4a9549a27897d714b29ec849db04eba3daab3d8

    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
<<<<<<< HEAD
        username.text = user.name;
=======
     
        if(user.role == "Admin"){
            AdminPanel.gameObject.SetActive(true);
        }         
        username.text = user.name;
        branchYear.text = user.branch + ", " + user.year;
>>>>>>> b4a9549a27897d714b29ec849db04eba3daab3d8
    }
}
