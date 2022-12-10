using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressHighlight : MonoBehaviour, IPointerDownHandler
{
    public static GameObject clickedObject;
    static bool didClick = false;

    static List<Button> highlightables = new List<Button>();

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
        highlightables.Add(gameObject.GetComponent<Button>());
    }

    void Update()
    {
        if (clickedObject == gameObject)
        {
            if (didClick && Input.GetKeyDown(KeyCode.Mouse0))
            {
                foreach(Button highlightable in highlightables)
                {
                    if (highlightable.gameObject != gameObject) highlightable.interactable = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                foreach(Button highlightable in highlightables)
                {
                    highlightable.interactable = true;
                }
                
                clickedObject = null;
                didClick = false;
            }
        }
    }
}
