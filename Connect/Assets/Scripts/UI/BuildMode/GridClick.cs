using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridClick : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    List<int> slotLocation;

    public static int fillMode;
    public static HashSet<List<int>> slots;

    public static GameObject selectedSlot;

    void AddSlot()
    {
        slots.Add(slotLocation);
    
        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = true;
    }

    void RemoveSlot()
    {
        slots.Remove(slotLocation);

        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.Mouse2))
        {
            if (BuildUI.buildMode == 2 && fillMode == -1)
            {
                if (eventData.pointerId == -1)
                {
                    fillMode = 0;
                    AddSlot();

                    foreach(List<int> pos in slots)
                    {
                        Debug.Log(pos[0] + ", " + pos[1]);
                    }
                }
                else if (eventData.pointerId == -2)
                {
                    fillMode = 1;
                    RemoveSlot();
                }
            }
            else if (BuildUI.buildMode == 0)
            {
                
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fillMode == 0 && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            AddSlot();
        }
        else if (fillMode == 1 && !Input.GetKeyDown(KeyCode.Mouse1))
        {
            RemoveSlot();
        }
    }

    void Start()
    {
        slotLocation = new List<int>{gameObject.transform.parent.GetSiblingIndex(), gameObject.transform.parent.parent.GetSiblingIndex()};

        HashSet<int> a = new HashSet<int>{4};
        HashSet<int> b = new HashSet<int>{4};
        
        HashSet<HashSet<int>> test = new HashSet<HashSet<int>>{a, b};
        Debug.Log(test.Count);
    }
}

