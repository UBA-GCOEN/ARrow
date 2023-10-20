using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Notifications : MonoBehaviour
{
    public GameObject loader;
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField description;
    [SerializeField] private TMP_InputField role;
    public GameObject everyone;
    public GameObject student;
    public GameObject staff;
    public GameObject faculty;
    public GameObject eventButtonPrefab;
    public Transform sliderContent;
    public Button AdminPanel;
    void Start()
    {
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));
     
        if(user.role == "Admin"){
            AdminPanel.gameObject.SetActive(true);
        } 

        
        loader.SetActive(true);
        StartCoroutine(GetNotification());
    }

    public void onSendButtonClicked()
    {
        loader.SetActive(true);
        StartCoroutine(SendNotification());
    }

    private IEnumerator GetNotification()
    {
        string ApiEndpoint = "https://arrowserver.vercel.app/notification/get";

        string token = PlayerPrefs.GetString("Token");
        UnityWebRequest request = UnityWebRequest.Get(ApiEndpoint);
        request.SetRequestHeader("Authorization", "Bearer " + token);
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

            List<NotificationsData> events = JsonConvert.DeserializeObject<List<NotificationsData>>(request.downloadHandler.text);

            Debug.Log(request.downloadHandler.text);

            foreach (NotificationsData notiData in events)
            {
                CreateNotification(notiData);
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

    void CreateNotification(NotificationsData eventData)
    {
        // Instantiate a button prefab
        GameObject eventButton = Instantiate(eventButtonPrefab, sliderContent);
        eventButton.transform.SetParent(sliderContent.transform);


        TextMeshProUGUI NotificationTitle = eventButton.transform.Find("NT").GetComponent<TextMeshProUGUI>();
        NotificationTitle.text = eventData.title;

        TextMeshProUGUI notificationDescription = eventButton.transform.Find("ND").GetComponent<TextMeshProUGUI>();
        notificationDescription.text = eventData.message;



        // Button buttonComponent = eventButton.GetComponent<Button>();
        // buttonComponent.onClick.AddListener(() => HandleEventButtonClick(eventData));
    }

    private IEnumerator SendNotification()
    {
        // string ApiEndpoint = "https://arrowserver.vercel.app/event/createEvent";
        string ApiEndpoint = "https://arrowserver.vercel.app/notification/send";

        string title = this.title.text;
        string description = this.description.text;
        // string role = this.role.text;

        WWWForm form = new WWWForm();

        form.AddField("title", title);
        form.AddField("message", description);

        string[] roles = new string[4];

        // Debug.Log(everyone.SetActive());

        if (everyone.activeInHierarchy)
        {
            roles = new string[] { "Admin", "Student", "Staff", "Faculty" };
        }
        else if (student.activeInHierarchy)
        {
            roles = new string[] { "Student" };
        }
        else if (staff.activeInHierarchy)
        {
            roles = new string[] { "Staff" };
        }
        else if (faculty.activeInHierarchy)
        {
            roles = new string[] { "Faculty" };
        }

        string json = JsonConvert.SerializeObject(roles);
        Debug.Log(json);

        form.AddField("receiverRole", json);


        string token = PlayerPrefs.GetString("Token");
        UnityWebRequest request = UnityWebRequest.Post(ApiEndpoint, form);
        request.SetRequestHeader("Authorization", "Bearer " + token);
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
            // SigninResponse response = JsonUtility.FromJson<SigninResponse>(request.downloadHandler.text);

            Debug.Log(request.downloadHandler.text);

            SceneManager.LoadScene("Notification");

        }
        else
        {
            loader.SetActive(false);
            Debug.LogError("Error: " + request);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }

        yield return null;
    }

    public class NotificationsData
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public string senderEmail { get; set; }
        public string senderName { get; set; }
        public string senderRole { get; set; }
        // public List<string> receiverRole { get; set; }
        // public List<object> receiverBranch { get; set; }
        // public List<object> receiverYear { get; set; }
        // public DateTime createdAt { get; set; }
        // public int __v { get; set; }
    }
}





