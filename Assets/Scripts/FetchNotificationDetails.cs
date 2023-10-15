using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FetchNotificationDetails : MonoBehaviour
{
    public TextMeshProUGUI notifTitle;
    public TMP_InputField title;

    public TextMeshProUGUI notifDesc;
    public TMP_InputField desc;

    //RadioBtns
    public GameObject everyone;
    public GameObject student;
    public GameObject staff;
    public GameObject faculty;

    public void setTitle()
    {
        notifTitle.text = title.text;
        Debug.Log(notifTitle.text);

    }

    public void setDesc()
    {
        notifDesc.text = desc.text;
        Debug.Log(notifDesc.text);


    }

    public void setRadioBtnAll()
    {
        if (everyone.activeInHierarchy == true)
        {
            everyone.SetActive(false);
            student.SetActive(false);
            staff.SetActive(false);
            faculty.SetActive(false);
        }
        else
        {
            everyone.SetActive(true);
            student.SetActive(true);
            staff.SetActive(true);
            faculty.SetActive(true);
        }

    }

    public void setRadioBtn(GameObject a)
    {
        if (a.activeInHierarchy == true)
        {
            everyone.SetActive(false);
            a.SetActive(false);
            
        }
        else 
        {
            a.SetActive(true);
            everyone.SetActive(false);
        }
    }

    public void sendData()
    {
        ////Function to send all data
    }
}
