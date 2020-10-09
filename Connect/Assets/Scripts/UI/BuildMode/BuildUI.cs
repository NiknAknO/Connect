using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    public static int buildMode;
    public static int[] selectedLink;

    [SerializeField] GameObject[] hotbar;
    [SerializeField] Sprite[] hotbarSprites;

    [SerializeField] GameObject buildWindow;

    [SerializeField] GameObject[] linkWindow;

    [SerializeField] GameObject[] linkMaster;
    List<GameObject>[] linkList;

    [SerializeField] Sprite[] linkWindowSprites;
    Color[] linkWindowTextColors;

    bool canScroll;
    
    public void SetHotbar(int newBuildMode)
    {
        if (buildMode != -1) hotbar[buildMode].GetComponent<Image>().sprite = hotbarSprites[2*buildMode];
        if (newBuildMode != -1) hotbar[newBuildMode].GetComponent<Image>().sprite = hotbarSprites[2*newBuildMode + 1];

        switch (newBuildMode)
        {
            case 0:
                buildWindow.SetActive(true);
                linkMaster[0].SetActive(true);

                linkMaster[1].SetActive(false);

                GridClick.fillMode = -1;
                break;

            case 1:
                buildWindow.SetActive(true);
                linkMaster[1].SetActive(true);

                linkMaster[0].SetActive(false);

                GridClick.fillMode = -1;
                GridClick.selectedSlot = null;
                break;
            
            default:
                buildWindow.SetActive(false);

                GridClick.selectedSlot = null;
                break;
        }

        buildMode = newBuildMode;
    }

    public void SelectLinkWindow(int windowNumber)
    {
        if (selectedLink[buildMode] != -1) 
        {
            linkList[buildMode][selectedLink[buildMode]].GetComponent<Image>().sprite = linkWindowSprites[0];
            linkList[buildMode][selectedLink[buildMode]].GetComponentInChildren<Text>().color = linkWindowTextColors[buildMode];
        }

        selectedLink[buildMode] = windowNumber;

        if (selectedLink[buildMode] != -1) 
        {
            linkList[buildMode][selectedLink[buildMode]].GetComponent<Image>().sprite = linkWindowSprites[buildMode + 1];
            linkList[buildMode][selectedLink[buildMode]].GetComponentInChildren<Text>().color = linkWindowTextColors[2];
        }

        if (selectedLink[buildMode] != -1) ScrollToShow(windowNumber);
    }

    public void AddLinkWindow()
    {
        Instantiate(linkWindow[buildMode], linkMaster[buildMode].transform);
        linkList[buildMode].Add(linkMaster[buildMode].transform.GetChild(linkList[buildMode].Count).gameObject);

        linkList[buildMode][linkList[buildMode].Count-1].transform.localPosition = new Vector3(0, -125 * (linkList[buildMode].Count-1), 0);

        SelectLinkWindow(linkList[buildMode].Count-1);
        linkList[buildMode][linkList[buildMode].Count-1].GetComponentInChildren<Text>().text = "Link Set " + linkList[buildMode].Count;
        
    }

    public void RemoveLinkWindow()
    {
        if (selectedLink[buildMode] != -1 && linkList[buildMode].Count != 0)
        {
            Destroy(linkList[buildMode][linkList[buildMode].Count-1]);
            linkList[buildMode].RemoveAt(linkList[buildMode].Count-1);

            if (selectedLink[buildMode] == linkList[buildMode].Count) selectedLink[buildMode]--;
            SelectLinkWindow(selectedLink[buildMode]);
        }
    }

    public void ScrollToTopOf(int windowNumber)
    {
        int scrollLimit = Mathf.Clamp(-5 + linkList[buildMode].Count, 0, int.MaxValue);

        linkMaster[buildMode].transform.localPosition = new Vector3(0, Mathf.Clamp(250 + 125*windowNumber, 250, 250 + 125*scrollLimit), 0);
    }

    public void ScrollToShow(int windowNumber)
    {
        int scrollPosition = (int)((linkMaster[buildMode].transform.localPosition.y-250)/125);

        if (scrollPosition > windowNumber)
        {
            ScrollToTopOf(windowNumber);
        }
        else if (scrollPosition < windowNumber - 4)
        {
            ScrollToTopOf(windowNumber - 4);
        }
        else
        {
            ScrollToTopOf(scrollPosition);
        }
    }

    public void SetScroll(bool updatedScroll)
    {
        canScroll = updatedScroll;
    }

    void Start()
    {
        buildMode = -1;

        selectedLink = new int[2] {-1, -1};
        
        linkList = new List<GameObject>[2]{ new List<GameObject>{}, new List<GameObject>{}};

        linkWindowTextColors = new Color[3] {Color.black, new Color32(0, 160, 0, 255), Color.white};

        canScroll = false;

        buildWindow.SetActive(false);

        GridClick.fillMode = -1;
        GridClick.slots = new HashSet<List<int>>();

        GridClick.selectedSlot = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SetHotbar(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetHotbar(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetHotbar(1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectLinkWindow(Mathf.Clamp(selectedLink[buildMode]+1, 0, linkList[buildMode].Count-1));
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedLink[buildMode] != -1)
            {
                SelectLinkWindow(Mathf.Clamp(selectedLink[buildMode]-1, 0, linkList[buildMode].Count-1));
            }
            else
            {
                SelectLinkWindow(linkList[buildMode].Count-1);
            }
        }

        if (canScroll && Input.mouseScrollDelta.y != 0)
        {
            ScrollToTopOf((int)((linkMaster[buildMode].transform.localPosition.y-250)/125 - Input.mouseScrollDelta.y));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buildMode == 0 || buildMode == 1)
            {
                if (selectedLink[buildMode] != -1) 
                {
                    SelectLinkWindow(-1);
                }
                else
                {
                    SetHotbar(-1);
                }
            }
            else if (buildMode == 2)
            {
                SetHotbar(-1);
            }
        }

        if (BuildUI.buildMode == 2 && GridClick.fillMode == -1 && !Input.GetKey(KeyCode.Mouse2))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GridClick.fillMode = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                GridClick.fillMode = 1;
            }
        }

        if ((GridClick.fillMode == 0 && Input.GetKeyUp(KeyCode.Mouse0)) || (GridClick.fillMode == 1 && Input.GetKeyUp(KeyCode.Mouse1)))
        {
            GridClick.fillMode = -1;
        }
    }
}
