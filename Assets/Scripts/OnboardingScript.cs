using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class OnboardingScript : MonoBehaviour
{
    private string baseURL = "https://arrowserver.vercel.app";
    // private string baseURL = "http://localhost:5000"; // To run server locally.
    [SerializeField] private string apiEndpoint;
    public TMP_InputField[] inputFields;
    public ToggleGroup toggleGroup;
    public Button nextButton;
    public TextMeshProUGUI serverMsg;
    public TextMeshProUGUI username;
    public GameObject loader;
    public string onBoardingPage;

    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }

    Toggle toggle;

    void Update()
    {
        bool allFieldsFilled = true;

        foreach (TMP_InputField field in inputFields)
        {
            if (string.IsNullOrWhiteSpace(field.text))
            {
                allFieldsFilled = false;
                break;
            }
        }

        nextButton.interactable = allFieldsFilled;
    }

    public void onNextButtonClick()
    {
        if (onBoardingPage == "Ob-1")
        {
            serverMsg.text = "";
        }
        loader.SetActive(true);
        StartCoroutine(UpdateDetails());
    }

    private IEnumerator UpdateDetails()
    {
        if (onBoardingPage == "Ob-1")
        {
            //Email Validation
            string[] emailDomains =
            {
            "@gmail.com",
            "@yahoo.com",
            "@hotmail.com",
            "@aol.com",
            "@outlook.com",
            "@gcoen.ac.in"
            };

            string email = inputFields[1].text;

            bool isValid = emailDomains.Any(domain => email.Contains(domain));

            if (!isValid)
            {
                loader.SetActive(false);
                serverMsg.text = "Invalid Email";
                yield break;
            }
        }
        
        Debug.Log("Check 1");

        // Update Route
        string updateEndpoint = baseURL + apiEndpoint;

        Debug.Log("Check 2:" + updateEndpoint);

        if (onBoardingPage != "Ob-1")
        {
            toggle = toggleGroup.ActiveToggles().FirstOrDefault();
            Debug.Log("Check 3:" + toggle.name);
        }      

        WWWForm form = new WWWForm();

        if (onBoardingPage == "Ob-1")
        {
            string usernametext = inputFields[0].text;
            string clgEmail = inputFields[1].text;
            form.AddField("name", usernametext);
            form.AddField("collegeEmail", clgEmail);

            //local storgae
            UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
            user.name = usernametext;
            user.collegeEmail = clgEmail;

            PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));

            Debug.Log("Check 4: username and collegeId added in local storage");
        }

        else if (onBoardingPage == "Ob-2")
        {
            form.AddField("role", toggle.name);
            if (toggle.name == "Guest")       //Guest Final
            {
                form.AddField("isOnboarded", 1);

                UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                user.isOnboarded = true;

                PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));
            }
        }

        else if (onBoardingPage == "Ob-Faculty") //submit
        {
            form.AddField("designation", toggle.name);
            form.AddField("isOnboarded", 1);
        }

        else if (onBoardingPage == "Ob-Staff")    //submit
        {
            form.AddField("branch", toggle.name);
            form.AddField("isOnboarded", 1);
        }

        else if (onBoardingPage == "Ob-Student")
        {
            form.AddField("year", toggle.name);
        }

        else if (onBoardingPage == "Ob-Student-2")   //submit
        {
            form.AddField("branch", toggle.name);
            form.AddField("isOnboarded", 1);
        }

        Debug.Log("Check 5:" + form);

        string token = PlayerPrefs.GetString("Token");

        UnityWebRequest request = UnityWebRequest.Post(updateEndpoint, form);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        Debug.Log("Check 6:" + request);

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

            if (response.success)
            {
                Debug.Log("Check 7:" + request.downloadHandler.text);
                //Update Local User Data
                if (onBoardingPage == "Ob-2")
                {
                    UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                    user.role = toggle.name;
                    PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));
                }

                else if (onBoardingPage == "Ob-Faculty") //submit
                {
                    UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                    user.designation = toggle.name;
                    user.isOnboarded = true;
                    PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));

                }

                else if (onBoardingPage == "Ob-Staff")    //submit
                {
                    UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                    user.branch = toggle.name;
                    user.isOnboarded = true;
                    PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));
                }

                else if (onBoardingPage == "Ob-Student")
                {
                    UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                    user.year = toggle.name;
                    PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));
                }

                else if (onBoardingPage == "Ob-Student-2")   //submit
                {
                    UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
                    user.branch = toggle.name;
                    user.isOnboarded = true;
                    PlayerPrefs.SetString("UserData", JsonUtility.ToJson(user));
                }

                Debug.Log("Check 8: PlayerPrefs Updated");

                //Go to next Scene
                if (onBoardingPage == "Ob-1")
                {
                    SceneManager.LoadScene("Ob-2");
                }

                else if (onBoardingPage == "Ob-2")
                {
                    if (toggle.name == "Guest")
                    {
                        SceneManager.LoadScene("Home");
                    }
                    else if (toggle.name == "Faculty")
                    {
                        SceneManager.LoadScene("Ob-Faculty");
                    }
                    else if (toggle.name == "Staff")
                    {
                        SceneManager.LoadScene("Ob-Staff");
                    }
                    else if (toggle.name == "Student")
                    {
                        SceneManager.LoadScene("Ob-Student");
                    }
                }

                else if (onBoardingPage == "Ob-Faculty") //submit
                {
                    SceneManager.LoadScene("Home");
                }

                else if (onBoardingPage == "Ob-Staff")    //submit
                {
                    SceneManager.LoadScene("Home");
                }

                else if (onBoardingPage == "Ob-Student")
                {
                    SceneManager.LoadScene("Ob-Student-2");
                }

                else if (onBoardingPage == "Ob-Student-2")   //submit
                {
                    SceneManager.LoadScene("Home");
                }
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
}
