using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class FetchUserDetails : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI branchYear;
    public Button AdminPanel;

    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
     
        if(user.role == "Admin"){
            AdminPanel.gameObject.SetActive(true);
        }         
        username.text = user.name;
        branchYear.text = user.branch + ", " + user.year;
    }
}
