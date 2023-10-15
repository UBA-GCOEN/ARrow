using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickPanel : MonoBehaviour, IPointerClickHandler
{
    public GameObject Panel;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Panel.activeInHierarchy == false)
        {
            Panel.SetActive(true);
        }
        else
        {
            Panel.SetActive(false);
        }
    }
}
