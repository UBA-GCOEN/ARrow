using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class NotifyInfo : MonoBehaviour
{
    public GameObject loader;
    public GameObject eventButtonPrefab;
    public Transform sliderContent;

    void Start()
    {
        loader.SetActive(true);
        StartCoroutine(GetNotification());
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





