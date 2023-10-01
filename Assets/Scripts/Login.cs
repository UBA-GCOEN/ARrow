using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
    [SerializeField] private string apiEndpoint = "https://arrowserver.vercel.app/userAdmin/signup";
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;

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
        // string email = email.text;
        // string password = password.text;

        WWWForm form = new WWWForm();
        form.AddField("email", email.text);
        form.AddField("password", password.text);

        UnityWebRequest request = UnityWebRequest.Post(apiEndpoint, form);
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
            string response = JsonUtility.FromJson<string>(request.downloadHandler.text);
            Debug.LogError(response);
            
        }
        else
        {
           Debug.LogError("Error: " + request.error);
        }


        yield return null;
    }

    private IEnumerator TryRegister()
    {
        string email = this.email.text;
        string password = this.password.text;
        string name = "Sidd";

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("confirmPassword", password);

        UnityWebRequest request = UnityWebRequest.Post(apiEndpoint, form);
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
            string response = JsonUtility.FromJson<string>(request.downloadHandler.text);
            Debug.LogError(response);
            
        }
        else
        {
           Debug.LogError("Error: " + request);
        }

        yield return null;
    }
}
