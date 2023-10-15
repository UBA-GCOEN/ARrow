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

    /// <summary>
    //added by harshal
    public TextMeshProUGUI branch;
    public TextMeshProUGUI year;
    public TextMeshProUGUI email;
    public TextMeshProUGUI bio;

    /// </summary>
    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
        username.text = user.name;
        branchYear.text = user.branch + ", " + user.year;

        /// <summary>
        //added by harshal
        branch.text = user.branch;
        year.text = user.year;
        email.text = user.email;
        bio.text = user.bio;


        /// </summary>
    }
}
