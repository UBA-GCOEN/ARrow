using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FetchUserDetails : MonoBehaviour
{
    public TextMeshProUGUI username;

    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
        username.text = user.name;
    }
}
