using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressHighlight : MonoBehaviour, IPointerDownHandler
{
    Button button;

    public static GameObject clickedObject;
    static bool didClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId == -1)
        {
            clickedObject = gameObject;
            didClick = true;
        }
    }

    void Start()
    {
        button = gameObject.GetComponent<Button>();
    
        didClick = false;
    }

    void Update()
    {
        if (didClick && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (clickedObject != gameObject) button.interactable = false;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            button.interactable = true;
            clickedObject = null;
            didClick = false;
        }
    }
}
