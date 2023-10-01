using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    // private string baseURL = "https://arrowserver.vercel.app"; 
    private string baseURL = "http://localhost:5000"; // To run server locally.
    [SerializeField] private string apiEndpoint;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField confirmPassword;
    public TextMeshProUGUI serverMsg;
    public GameObject loader;
    public void onLoginButtonClick()
    {
        loader.SetActive(true);
        StartCoroutine(TryLogin());
    }

    public void onRegisterButtonClick()
    {
        loader.SetActive(true);
        StartCoroutine(TryRegister());
    }

    private IEnumerator TryLogin()
    {
        string LoginEndpoint = baseURL + apiEndpoint;
        string email = this.email.text;
        string password = this.password.text;

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(LoginEndpoint, form);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            loader.SetActive(false);
            SigninResponse response = JsonUtility.FromJson<SigninResponse>(request.downloadHandler.text);
            Debug.LogError(response.success);
            Debug.LogError(response.result);
            Debug.LogError(response.token);
            Debug.LogError(response.msg);

            if (response.success)
            {
                PlayerPrefs.SetString("UserName", response.result);
                SceneManager.LoadScene("Home");
            }
            else
            {
                serverMsg.text = response.msg;
                Debug.LogError(response.msg);
            }

        }
        else
        {
            loader.SetActive(false);
            Debug.LogError("Error: " + request);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }

        yield return null;
    }

    private IEnumerator TryRegister()
    {
        string registerEndpoint = baseURL + apiEndpoint;
        string email = this.email.text;
        string password = this.password.text;
        string name = "Sidd";
        string confirmPassword = this.confirmPassword.text;

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("confirmPassword", confirmPassword);

        UnityWebRequest request = UnityWebRequest.Post(registerEndpoint, form);
        var handler = request.SendWebRequest();

        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;

            if (startTime > 10.0f)
            {
                break;
            }

            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            loader.SetActive(false);
            SignupResponse response = JsonUtility.FromJson<SignupResponse>(request.downloadHandler.text);

            if (response.success)
            {
                serverMsg.text = response.msg;
                this.email.text = "";
                this.password.text = "";
                this.confirmPassword.text = "";
            }
            else
            {
                serverMsg.text = response.msg;
            }

            Debug.LogError(response.msg);

        }
        else
        {
            loader.SetActive(false);
            SignupResponse response = JsonUtility.FromJson<SignupResponse>(request.downloadHandler.text);
            serverMsg.text = response.msg;
            Debug.LogError("Error: " + request);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }

        yield return null;
    }
}
