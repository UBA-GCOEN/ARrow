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
    private string baseURL = "https://arrowserver.vercel.app";
    [SerializeField] private string apiEndpoint;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField confirmPassword;
    public void onLoginButtonClick()
    {
        StartCoroutine(TryLogin());
    }

    public void onRegisterButtonClick()
    {
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
            SigninResponse response = JsonUtility.FromJson<SigninResponse>(request.downloadHandler.text);
            Debug.LogError(response.msg);
            
            SceneManager.LoadScene("Home");
        }
        else
        {
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
            SignupResponse response = JsonUtility.FromJson<SignupResponse>(request.downloadHandler.text);
            Debug.LogError(response.msg);
            
        }
        else
        {
           Debug.LogError("Error: " + request);
           Debug.LogError("Response: " + request.downloadHandler.text);
        }

        yield return null;
    }
}
