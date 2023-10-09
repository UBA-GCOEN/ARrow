using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class FetchUserDetailsProfile : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI branchYear;

    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
        username.text = user.name;
        branchYear.text = user.branch + ", " + user.year;
    }
}
