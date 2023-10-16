using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class getEvents : MonoBehaviour
{
    public GameObject loader;
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField description;
    [SerializeField] private TMP_InputField eventCoordinator;
    [SerializeField] private TMP_InputField time;
    [SerializeField] private TMP_InputField venue;
    [SerializeField] private TMP_InputField guest;
    public GameObject eventButtonPrefab;
    public Transform sliderContent;
    public GameObject Panel;
    public TextMeshProUGUI EventTitle;
    public TextMeshProUGUI OrganiserName;
    public TextMeshProUGUI OrganiserEmail;
    public TextMeshProUGUI EventDescription;
    public TextMeshProUGUI CheifGuestName;
    public TextMeshProUGUI EventCoordinator;
    public TextMeshProUGUI EventTime;
    public TextMeshProUGUI EventVenue;

    void Start()
    {
        loader.SetActive(true);
        StartCoroutine(GetEvents());
    }

    public void onSaveButtonClicked()
    {
        loader.SetActive(true);
        StartCoroutine(CreateEvents());
    }

    private IEnumerator GetEvents()
    {
        string ApiEndpoint = "https://arrowserver.vercel.app/event/getAllEvents";

        UnityWebRequest request = UnityWebRequest.Get(ApiEndpoint);
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

            List<EventData> events = JsonConvert.DeserializeObject<List<EventData>>(request.downloadHandler.text);

            Debug.Log(events[0].title);

            foreach (EventData eventData in events)
            {
                CreateEventButton(eventData);
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

    void CreateEventButton(EventData eventData)
    {
        // Instantiate a button prefab
        GameObject eventButton = Instantiate(eventButtonPrefab, sliderContent);
        eventButton.transform.SetParent(sliderContent.transform); 

        TextMeshProUGUI EventTitle = eventButton.GetComponentInChildren<TextMeshProUGUI>();
        EventTitle.text = eventData.title;

        Button buttonComponent = eventButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => HandleEventButtonClick(eventData));
    }

    void HandleEventButtonClick(EventData eventData)
    {
        if (Panel.activeInHierarchy == false)
        {
            Panel.SetActive(true);
        }
        else
        {
            Panel.SetActive(false);
        }

        EventTitle.text = eventData.title;
        OrganiserName.text = eventData.organizerName;
        OrganiserEmail.text = eventData.organizerEmail;
        EventDescription.text = eventData.description;
        CheifGuestName.text = eventData.guest;
        EventCoordinator.text = eventData.eventCoordinator;
        EventTime.text = eventData.time;
        EventVenue.text = eventData.venue;
    }



    private IEnumerator CreateEvents()
    {
        string ApiEndpoint = "https://arrowserver.vercel.app/event/createEvent";
        // string ApiEndpoint = "http://localhost:5000/event/createEvent";

        string title = this.title.text;
        string description = this.description.text;
        string eventCoordinator = this.eventCoordinator.text;
        string time = this.time.text;
        string venue = this.venue.text;
        string guest = this.guest.text;

        WWWForm form = new WWWForm();

        form.AddField("title", title);
        form.AddField("description", description);
        form.AddField("eventCoordinator", eventCoordinator);
        form.AddField("time", time);
        form.AddField("venue", venue);
        form.AddField("guest", guest);

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

            // if (response.success)
            // {

            // }
            // else
            // {
            //     // serverMsg.text = response.msg;
            //     Debug.LogError(response.msg);
            // }

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


public class EventData
{
    public string _id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string organizerRole { get; set; }
    public string organizerEmail { get; set; }
    public string organizerName { get; set; }
    public string status { get; set; }
    public string eventCoordinator { get; set; }
    public string time { get; set; }
    public string venue { get; set; }
    public string guest { get; set; }
    public int __v { get; set; }
}



