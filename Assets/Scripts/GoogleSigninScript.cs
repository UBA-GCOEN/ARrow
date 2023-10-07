using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;


public class GoogleSigninScript : MonoBehaviour
{
    // private string baseURL = "https://arrowserver.vercel.app";
    private string baseURL = "http://localhost:5000"; // To run server locally.
    [SerializeField] private string apiEndpoint;
    public GameObject loader;
    public TextMeshProUGUI serverMsg;
    public void onGoogleButtonClick()
    {
        // serverMsg.text = "";
        loader.SetActive(true);
        StartCoroutine(TryGoogleLogin());
    }
    private IEnumerator TryGoogleLogin()
    {
        loader.SetActive(false);
        Application.OpenURL("http://localhost:5000/auth/google");
        yield return null;
    }
}
