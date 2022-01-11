using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public static Sprite[] slotIcons;

    bool canScroll;
    
    public void TestThis()
    {
        Debug.Log("Test");
    }

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

                GridClick.lineMaster[0].SetActive(true);
                GridClick.lineMaster[1].SetActive(false);

                if (selectedLink[0] != -1)
                {
                    foreach(GameObject aLineSet in GridClick.lineSet[0])
                    {
                        if (aLineSet != GridClick.lineSet[0][selectedLink[0]]) aLineSet.SetActive(false);
                    }
                }

                GridClick.ToggleGravSlots(false);
                GridClick.ToggleGravEndpoints(false, false);

                break;
            
            case 1:
                buildWindow.SetActive(true);
                linkMaster[1].SetActive(true);

                linkMaster[0].SetActive(false);

                GridClick.lineMaster[0].SetActive(false);
                GridClick.lineMaster[1].SetActive(true);

                if (selectedLink[1] != -1)
                {
                    foreach(GameObject aLineSet in GridClick.lineSet[1])
                    {
                        if (aLineSet != GridClick.lineSet[1][selectedLink[1]]) aLineSet.SetActive(false);
                    }
                }

                GridClick.ToggleGravSlots(true);
                GridClick.ToggleGravEndpoints(true, true);

                GridClick.DeselectConSlot();
                break;
            
            case 2:
                buildWindow.SetActive(false);

                GridClick.lineMaster[0].SetActive(false);
                GridClick.lineMaster[1].SetActive(false);

                GridClick.DeselectConSlot();

                GridClick.ToggleGravSlots(false);
                GridClick.ToggleGravEndpoints(false, false);
                break;
            
            case -1:
                buildWindow.SetActive(false);

                GridClick.lineMaster[0].SetActive(true);

                foreach(GameObject aLineSet in GridClick.lineSet[0])
                {
                    aLineSet.SetActive(true);
                }

                GridClick.lineMaster[1].SetActive(true);

                foreach(GameObject aLineSet in GridClick.lineSet[1])
                {
                    aLineSet.SetActive(true);
                }

                GridClick.ToggleGravSlots(true);
                GridClick.ToggleGravEndpoints(true, false);

                break;
        }

        if (buildMode != newBuildMode) 
        {
            if (PressHighlight.clickedObject != null)
            {
                buildWindow.transform.GetChild(1).GetChild(0).GetComponent<Button>().interactable = false;
                buildWindow.transform.GetChild(1).GetChild(1).GetComponent<Button>().interactable = false;
            }

            GridClick.fillMode = -1;
            GridClick.ExitToolMode();

            GridClick.canSelect = true;
            
            buildMode = newBuildMode;
        }
    }

    public void SelectLinkWindow(int windowNumber)
    {
        GridClick.DeselectConSlot();

        if (selectedLink[buildMode] != -1) 
        {
            linkList[buildMode][selectedLink[buildMode]].GetComponent<Image>().sprite = linkWindowSprites[0];
            linkList[buildMode][selectedLink[buildMode]].GetComponentInChildren<Text>().color = linkWindowTextColors[buildMode];
        }

        if (selectedLink[buildMode] != windowNumber)
        {
            GridClick.fillMode = -1;
            GridClick.ExitToolMode();

            GridClick.canSelect = true;

            selectedLink[buildMode] = windowNumber;
        }

        if (selectedLink[buildMode] != -1) 
        {
            linkList[buildMode][selectedLink[buildMode]].GetComponent<Image>().sprite = linkWindowSprites[buildMode + 1];
            linkList[buildMode][selectedLink[buildMode]].GetComponentInChildren<Text>().color = linkWindowTextColors[2];

            foreach(GameObject aLineSet in GridClick.lineSet[buildMode])
            {
                if (aLineSet != GridClick.lineSet[buildMode][selectedLink[buildMode]])
                { 
                    aLineSet.SetActive(false);
                }
                else
                {
                    aLineSet.SetActive(true);
                }
            }
        }
        else
        {
            foreach(GameObject aLineSet in GridClick.lineSet[buildMode])
            {
                if (selectedLink[buildMode] == -1 || aLineSet != GridClick.lineSet[buildMode][selectedLink[buildMode]]) aLineSet.SetActive(true);
            }
        }

        if (buildMode == 1) GridClick.ToggleGravEndpoints(true, true);

        ScrollToShow(windowNumber);
    }
 
    public void AddLinkWindow()
    {
        if (linkList[buildMode].Count < 1024)
        {
            if (buildMode == 0)
            {
                GridClick.connections.Add(new List<HashSet<(int, int)>>{});
            }
            else if (buildMode == 1)
            {
                GridClick.gravLines.Add(new List<(int, int)>{});
            }

            Instantiate(linkWindow[buildMode], linkMaster[buildMode].transform);
            linkList[buildMode].Add(linkMaster[buildMode].transform.GetChild(linkList[buildMode].Count).gameObject);

            linkList[buildMode][linkList[buildMode].Count-1].transform.localPosition = new Vector3(0, -125 * (linkList[buildMode].Count-1), 0);

            if (buildMode == 0)
            {
                linkList[buildMode][linkList[buildMode].Count-1].GetComponentInChildren<Text>().text = "Con Set " + linkList[buildMode].Count;
            }
            else if (buildMode == 1)
            {
                linkList[buildMode][linkList[buildMode].Count-1].GetComponentInChildren<Text>().text = "Grav Line " + linkList[buildMode].Count;
            }

            GridClick.lineSet[buildMode].Add(new GameObject("Line Set"));
            GridClick.lineSet[buildMode][GridClick.lineSet[buildMode].Count-1].transform.parent = GridClick.lineMaster[buildMode].transform;

            SelectLinkWindow(linkList[buildMode].Count-1);
        }
    }

    public void RemoveLinkWindow()
    {
        if (selectedLink[buildMode] != -1 && linkList[buildMode].Count != 0)
        {
            if (buildMode == 1)
            {
                foreach((int, int) gravSlot in GridClick.gravLines[BuildUI.selectedLink[1]])
                {
                    GridClick.board.transform.GetChild(gravSlot.Item2).GetChild(gravSlot.Item1).GetComponent<Image>().sprite = slotIcons[0];
                    GridClick.board.transform.GetChild(gravSlot.Item2).GetChild(gravSlot.Item1).GetChild(0).GetComponent<GridClick>().hasGravSlot = false;
                }

                GridClick.ToggleGravEndpoints(false, false);
            }
            if (buildMode == 0) 
            {
                GridClick.connections.RemoveAt(selectedLink[buildMode]);
            }
            else if (buildMode == 1)
            {
                GridClick.gravLines.RemoveAt(selectedLink[buildMode]);
            }

            Destroy(linkList[buildMode][linkList[buildMode].Count-1]);
            linkList[buildMode].RemoveAt(linkList[buildMode].Count-1);

            Destroy(GridClick.lineSet[buildMode][selectedLink[buildMode]]);
            GridClick.lineSet[buildMode].RemoveAt(selectedLink[buildMode]);

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

    public void BuildBuild(HashSet<(int, int)> lSlots, List<List<HashSet<(int, int)>>> lConnections, List<List<(int, int)>> lGravLines)
    {
        GridClick.slots = lSlots;
        GridClick.connections = lConnections;
        GridClick.gravLines = lGravLines;

        foreach ((int, int) slot in GridClick.slots)
        {
            GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetComponent<Image>().enabled = true;
            GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetChild(0).GetComponent<GridClick>().filled = true;
        }

        for (int i = 0; i < GridClick.connections.Count; i++)
        {
            Instantiate(linkWindow[0], linkMaster[0].transform);
            linkList[0].Add(linkMaster[0].transform.GetChild(linkList[0].Count).gameObject);

            linkList[0][linkList[0].Count-1].transform.localPosition = new Vector3(0, -125 * (linkList[0].Count-1), 0);

            linkList[0][linkList[0].Count-1].GetComponentInChildren<Text>().text = "Link Set " + linkList[0].Count;

            GridClick.lineSet[0].Add(new GameObject("Line Set"));
            GridClick.lineSet[0][GridClick.lineSet[0].Count-1].transform.parent = GridClick.lineMaster[0].transform;

            foreach (HashSet<(int, int)> connection in GridClick.connections[i])
            {
                GameObject newLine = (GameObject) Instantiate(GridClick.line[0], GridClick.lineSet[0][i].transform);
                newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(-790.5f + 51*connection.First().Item1, 790.5f - 51*connection.First().Item2, -0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-790.5f + 51*connection.Last().Item1, 790.5f - 51*connection.Last().Item2, -0.5f));
            }
        }

        for (int i = 0; i < GridClick.gravLines.Count; i++)
        {
            Instantiate(linkWindow[1], linkMaster[1].transform);
            linkList[1].Add(linkMaster[1].transform.GetChild(linkList[1].Count).gameObject);

            linkList[1][linkList[1].Count-1].transform.localPosition = new Vector3(0, -125 * (linkList[1].Count-1), 0);

            linkList[1][linkList[1].Count-1].GetComponentInChildren<Text>().text = "Link Set " + linkList[1].Count;

            GridClick.lineSet[1].Add(new GameObject("Line Set"));
            GridClick.lineSet[1][GridClick.lineSet[1].Count-1].transform.parent = GridClick.lineMaster[1].transform;

            for (int j = 0; j < GridClick.gravLines[i].Count; j++)
            {
                if (j != GridClick.gravLines[i].Count-1)
                {
                    GameObject newLine = (GameObject) Instantiate(GridClick.line[1], GridClick.lineSet[1][i].transform);
                    newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(-790.5f + 51*GridClick.gravLines[i][j].Item1, 790.5f - 51*GridClick.gravLines[i][j].Item2, -0.5f));
                    newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-790.5f + 51*GridClick.gravLines[i][j+1].Item1, 790.5f - 51*GridClick.gravLines[i][j+1].Item2, -0.5f));
                }

                GridClick.board.transform.GetChild(GridClick.gravLines[i][j].Item2).GetChild(GridClick.gravLines[i][j].Item1).GetChild(0).GetComponent<GridClick>().hasGravSlot = true;
            }
        }

        GridClick.ToggleGravSlots(true);
        GridClick.ToggleGravEndpoints(true, false);
    }

    HashSet<(int, int)> a = new HashSet<(int, int)>{(0, 0), (0, 1), (1, 0), (1, 1)};
    List<List<HashSet<(int, int)>>> b = new List<List<HashSet<(int, int)>>>{ new List<HashSet<(int, int)>>{ new HashSet<(int, int)>{(0, 0), (0, 1)}, new HashSet<(int, int)>{(1, 0), (1, 1)} }, new List<HashSet<(int, int)>>{ new HashSet<(int, int)>{(0, 0), (1, 0)}, new HashSet<(int, int)>{(0, 1), (1, 1)} } };
    List<List<(int, int)>> c = new List<List<(int, int)>>{ new List<(int, int)>{(0, 0), (0, 1)}, new List<(int, int)>{(1, 0), (1, 1)} };

    void Start()
    {
        buildMode = -1;

        selectedLink = new int[2] {-1, -1};
        
        linkList = new List<GameObject>[2]{ new List<GameObject>{}, new List<GameObject>{}};

        linkWindowTextColors = new Color[3] {Color.black, new Color32(0, 160, 0, 255), Color.white};

        canScroll = false;

        buildWindow.SetActive(false);

        slotIcons = new Sprite[]{Resources.Load<Sprite>("UI/Sprites/Grid/Slots/BuildSlotIcon"), Resources.Load<Sprite>("UI/Sprites/Grid/Slots/GravSlotIcon")};

        GridClick.board = (GameObject) Instantiate(Resources.Load<GameObject>("UI/Prefabs/BuildMode/Grids/Very Large Grid"), GameObject.Find("Board").transform);
        GridClick.board.transform.SetAsFirstSibling();
        
        GridClick.canSelect = true;

        GridClick.fillMode = -1;
        GridClick.toolMode = -1;

        GridClick.slots = new HashSet<(int, int)>();

        GridClick.connections = new List<List<HashSet<(int, int)>>>{};
        GridClick.gravLines = new List<List<(int, int)>>{};

        GridClick.selectedSlot = null;

        GridClick.lineMaster = new GameObject[]{GameObject.Find("Connect Lines"), GameObject.Find("Gravity Lines")};
        GridClick.lineMaster[0].SetActive(true);
        GridClick.lineMaster[1].SetActive(true);

        GridClick.lineSet = new List<GameObject>[]{new List<GameObject>(), new List<GameObject>()};

        GridClick.line = new GameObject[]{Resources.Load<GameObject>("UI/Prefabs/BuildMode/Lines/ConLine"), Resources.Load<GameObject>("UI/Prefabs/BuildMode/Lines/GravLine")};

        //BuildBuild(a, b, c);
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

        if (buildMode == 0 || buildMode == 1)
        {
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

            if (Input.GetKeyDown(KeyCode.Equals))
            {
                AddLinkWindow();
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                RemoveLinkWindow();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((buildMode == 0 || buildMode == 1) && GridClick.toolMode == -1)
            {
                if (buildMode == 0 && GridClick.selectedSlot != null)
                {
                    GridClick.DeselectConSlot();
                }
                else if (selectedLink[buildMode] != -1) 
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

        if (GridClick.fillMode == -1 && !Input.GetKey(KeyCode.Mouse2) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
            {
                GridClick.fillMode = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse0))
            {
                GridClick.fillMode = 1;
            }
        }
        if ((GridClick.fillMode == 0 && Input.GetKeyUp(KeyCode.Mouse0)) || (GridClick.fillMode == 1 && Input.GetKeyUp(KeyCode.Mouse1)))
        {
            GridClick.fillMode = -1;
        }

        if (buildMode == 0 && GridClick.fillMode == -1 && GridClick.toolMode == -1 && !Input.GetKey(KeyCode.Mouse2) && GridClick.selectedSlot == null)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                GridClick.toolMode = 0;
                GridClick.canSelect = false;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
            {
                GridClick.toolMode = 1;
                GridClick.canSelect = false;
            }
        }
        if ((GridClick.toolMode == 0 && Input.GetKeyUp(KeyCode.LeftShift)) || (GridClick.toolMode == 1 && Input.GetKeyUp(KeyCode.LeftControl)) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
        {
            GridClick.ExitToolMode();
        }

        if (GridClick.toolMode == -1 && GridClick.selectedSlot == null && !Input.GetKey(KeyCode.Mouse1))
        {
            GridClick.canSelect = true;
        }

        if (GridClick.toolMode == 0 && GridClick.selectedSlot != null && Input.GetKeyUp(KeyCode.Mouse0))
        {
            GridClick.selectedSlot.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            GridClick.selectedSlot = null;
        }
    }
}
