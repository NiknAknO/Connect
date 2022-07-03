using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridClick : MonoBehaviour, IPointerDownHandler
{
    public static GameObject board;

    public static GameObject selectedSlot;
    public static bool canSelect;

    (int, int) slotLocation;
    
    public bool filled;
    public bool hasGravSlot;

    public static int fillMode;
    public static int toolMode;

    public static HashSet<(int, int)> slots;

    public static List<List<HashSet<(int, int)>>> connections;
    public static List<List<(int, int)>> gravLines;

    public static GameObject[] lineMaster;
    public static List<GameObject>[] lineSet;
    public static GameObject[] line;

    void AddSlot()
    {
        filled = true;
        slots.Add(slotLocation);
    
        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = true;
    }

    void RemoveSlot()
    {
        filled = false;
        slots.Remove(slotLocation);

        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = false;

        RemoveConSlot(false);

        for (int i = 0; i < gravLines.Count; i++)
        {
            if (gravLines[i].Contains(slotLocation)) RemoveGravLink(i);
        }
    }

    void AddConLink()
    {
        if (gameObject != selectedSlot)
        {
            HashSet<(int, int)> newConLink = new HashSet<(int, int)>{slotLocation, selectedSlot.GetComponent<GridClick>().slotLocation};
        
            bool isDuplicate = false;

            foreach (HashSet<(int, int)> conLink in connections[BuildUI.selectedLink[0]])
            {
                if (newConLink.SetEquals(conLink))
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                connections[BuildUI.selectedLink[0]].Add(newConLink);
            
                GameObject newLine = (GameObject) Instantiate(line[0], lineSet[0][BuildUI.selectedLink[0]].transform);
                newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(-790.5f + 51*slotLocation.Item1, 790.5f - 51*slotLocation.Item2, -0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-790.5f + 51*selectedSlot.GetComponent<GridClick>().slotLocation.Item1, 790.5f - 51*selectedSlot.GetComponent<GridClick>().slotLocation.Item2, -0.5f));
            }
        }
    }

    void RemoveConLink()
    {
        if (gameObject != selectedSlot)
        {
            HashSet<(int, int)> deletedConLink = new HashSet<(int, int)>{slotLocation, selectedSlot.GetComponent<GridClick>().slotLocation};

            foreach (HashSet<(int, int)> conLink in connections[BuildUI.selectedLink[0]])
            {
                if (deletedConLink.SetEquals(conLink))
                {
                    Destroy(lineSet[0][BuildUI.selectedLink[0]].transform.GetChild(connections[BuildUI.selectedLink[0]].IndexOf(conLink)).gameObject);

                    connections[BuildUI.selectedLink[0]].Remove(conLink);
                    break;
                }
            }
        }
    }

    void RemoveConSlot(bool local)
    {
        int[] linkRange;

        List<HashSet<(int, int)>> toRemove;
        List<Transform> linesToRemove;

        if (local)
        {
            linkRange = new int[]{BuildUI.selectedLink[0]};
        }
        else
        {
            linkRange = new int[connections.Count];
            for (int i = 0; i < connections.Count; i++) {
                linkRange[i] = i;
            }
        }

        foreach (int i in linkRange)
        {
            toRemove = new List<HashSet<(int, int)>>{};
            linesToRemove = new List<Transform>();

            for (int j = 0; j < connections[i].Count; j++)
            {
                if (connections[i][j].Contains(slotLocation))
                {
                    linesToRemove.Add(lineSet[0][i].transform.GetChild(j));
                    toRemove.Add(connections[i][j]);
                }
            }

            foreach(HashSet<(int, int)> removedConnection in toRemove)
            {
                connections[i].Remove(removedConnection);
            }
            foreach(Transform conLine in linesToRemove)
            {
                Destroy(conLine.gameObject);
                conLine.parent = null;
            }
        }
    }

    void SelectConSlot()
    {
        gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                    
        for (int i = 0; i < lineSet[0][BuildUI.selectedLink[0]].transform.childCount; i++)
        {
            if (!connections[BuildUI.selectedLink[0]][i].Contains(slotLocation)) lineSet[0][BuildUI.selectedLink[0]].transform.GetChild(i).gameObject.SetActive(false);
        }

        canSelect = false;

        selectedSlot = gameObject;
    }

    public static void DeselectConSlot()
    {
        if (selectedSlot != null)
        {
            selectedSlot.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            
            if (BuildUI.selectedLink[0] != -1)
            {
                for (int i = 0; i < lineSet[0][BuildUI.selectedLink[0]].transform.childCount; i++)
                {
                    lineSet[0][BuildUI.selectedLink[0]].transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            selectedSlot = null;
        }
    }

    public static void ExitToolMode()
    {
        if (selectedSlot != null)
        {
            selectedSlot.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = false;
            selectedSlot = null;
        }

        toolMode = -1;
    }

    void AddGravLink()
    {
        gravLines[BuildUI.selectedLink[1]].Add(slotLocation);
        
        if (gravLines[BuildUI.selectedLink[1]].Count != 1)
        {
            (int, int) previousLocation = gravLines[BuildUI.selectedLink[1]][gravLines[BuildUI.selectedLink[1]].Count-2];

            GameObject newLine = (GameObject) Instantiate(line[1], lineSet[1][BuildUI.selectedLink[1]].transform);
            newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(-790.5f + 51*slotLocation.Item1, 790.5f - 51*slotLocation.Item2, -0.5f));
            newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-790.5f + 51*previousLocation.Item1, 790.5f - 51*previousLocation.Item2, -0.5f));

            board.transform.GetChild(previousLocation.Item2).GetChild(previousLocation.Item1).GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(false);
            
            board.transform.GetChild(previousLocation.Item2).GetChild(previousLocation.Item1).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
            RotateGravDirection(board.transform.GetChild(previousLocation.Item2).GetChild(previousLocation.Item1).GetChild(1).GetChild(1).GetChild(1).GetComponent<LineRenderer>(), Mathf.Rad2Deg*Mathf.Atan2(previousLocation.Item2 - slotLocation.Item2, slotLocation.Item1 - previousLocation.Item1));
        }
        
        //gameObject.transform.parent.GetComponent<Image>().sprite = BuildUI.slotIcons[1];
        gameObject.transform.parent.GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;
        gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(true);

        hasGravSlot = true;
    }

    void RemoveGravLink(int selectedLink)
    {
        hasGravSlot = false; 

        int slotIndex = gravLines[selectedLink].IndexOf(slotLocation);

        if (BuildUI.buildMode == 1 && selectedLink == BuildUI.selectedLink[1])
        {
            gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
            gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(false);
        }

        //gameObject.transform.parent.GetComponent<Image>().sprite = BuildUI.slotIcons[0];
        gameObject.transform.parent.GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;

        List<Transform> toRemove = new List<Transform>();

        if (slotIndex != 0)
        {
            Destroy(lineSet[1][selectedLink].transform.GetChild(slotIndex-1).gameObject);
            toRemove.Add(lineSet[1][selectedLink].transform.GetChild(slotIndex-1));
        }
        if (slotIndex != gravLines[selectedLink].Count-1)
        {
            Destroy(lineSet[1][selectedLink].transform.GetChild(slotIndex).gameObject);
            toRemove.Add(lineSet[1][selectedLink].transform.GetChild(slotIndex));
        }

        if (slotIndex != 0 && slotIndex != gravLines[selectedLink].Count-1)
        {
            GameObject newLine = (GameObject) Instantiate(line[1], lineSet[1][selectedLink].transform);
            newLine.transform.SetSiblingIndex(slotIndex-1);
        
            newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(-790.5f + 51*gravLines[selectedLink][slotIndex-1].Item1, 790.5f - 51*gravLines[selectedLink][slotIndex-1].Item2, -0.5f));
            newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(-790.5f + 51*gravLines[selectedLink][slotIndex+1].Item1, 790.5f - 51*gravLines[selectedLink][slotIndex+1].Item2, -0.5f));
        
            RotateGravDirection(board.transform.GetChild(gravLines[selectedLink][slotIndex-1].Item2).GetChild(gravLines[selectedLink][slotIndex-1].Item1).GetChild(1).GetChild(1).GetChild(1).GetComponent<LineRenderer>(), Mathf.Rad2Deg*Mathf.Atan2(gravLines[selectedLink][slotIndex-1].Item2 - gravLines[selectedLink][slotIndex+1].Item2, gravLines[selectedLink][slotIndex+1].Item1 - gravLines[selectedLink][slotIndex-1].Item1));
        }

        gravLines[selectedLink].Remove(slotLocation);

        if (BuildUI.buildMode == 1 && selectedLink == BuildUI.selectedLink[1] && gravLines[selectedLink].Count != 0)
        {
            board.transform.GetChild(gravLines[selectedLink][gravLines[selectedLink].Count-1].Item2).GetChild(gravLines[selectedLink][gravLines[selectedLink].Count-1].Item1).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
            board.transform.GetChild(gravLines[selectedLink][gravLines[selectedLink].Count-1].Item2).GetChild(gravLines[selectedLink][gravLines[selectedLink].Count-1].Item1).GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(true);
        }

        foreach(Transform removed in toRemove)
        {
            removed.parent = null;
        }
    }

    public static void ToggleGravSlots(bool state)
    {
        foreach(List<(int, int)> gravSlotSet in gravLines)
        {
            foreach((int, int) gravSlot in gravSlotSet)
            {
                board.transform.GetChild(gravSlot.Item2).GetChild(gravSlot.Item1).GetChild(1).GetChild(0).GetComponent<Image>().enabled = state;
            }
        }
    }

    public static void ToggleGravEndpoints(bool state, bool local)
    {
        foreach(List<(int, int)> gravSlotSet in gravLines)
        {
            if (gravSlotSet.Count != 0)
            {
                if (state && (!local || BuildUI.selectedLink[1] == -1 || gravSlotSet == gravLines[BuildUI.selectedLink[1]]))
                {
                    for (int i = 0; i < gravSlotSet.Count-1; i++)
                    {
                        board.transform.GetChild(gravSlotSet[i].Item2).GetChild(gravSlotSet[i].Item1).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
                    }
                    board.transform.GetChild(gravSlotSet[gravSlotSet.Count-1].Item2).GetChild(gravSlotSet[gravSlotSet.Count-1].Item1).GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    for (int i = 0; i < gravSlotSet.Count-1; i++)
                    {
                        board.transform.GetChild(gravSlotSet[i].Item2).GetChild(gravSlotSet[i].Item1).GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(false);
                    }
                    board.transform.GetChild(gravSlotSet[gravSlotSet.Count-1].Item2).GetChild(gravSlotSet[gravSlotSet.Count-1].Item1).GetChild(1).GetChild(1).GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }

    public static void RotateGravDirection(LineRenderer gravDirection, float direction)
    {
        float cosDirection = Mathf.Cos(Mathf.Deg2Rad*(direction + 45));
        float sinDirection = Mathf.Sin(Mathf.Deg2Rad*(direction + 45));

        gravDirection.SetPosition(0, new Vector3(cosDirection*3 - sinDirection*4.5f, sinDirection*3 + cosDirection*4.5f, 0));
        gravDirection.SetPosition(1, new Vector3(cosDirection*3 + sinDirection*3, sinDirection*3 - cosDirection*3, 0));
        gravDirection.SetPosition(2, new Vector3(-cosDirection*4.5f + sinDirection*3, -sinDirection*4.5f - cosDirection*3, 0));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.Mouse2))
        {
            if (fillMode == -1)
            {
                if (eventData.pointerId == -1)
                {
                    fillMode = 0;
                }
                else if (eventData.pointerId == -2)
                {
                    fillMode = 1;
                }
            }
            
            if (BuildUI.buildMode == 0 && BuildUI.selectedLink[0] != -1 && filled)
            {
                switch (toolMode)
                {
                    case -1:
                        if (selectedSlot == null)
                        {
                            if (fillMode == 0 && eventData.pointerId == -1)
                            {
                                SelectConSlot();
                            }
                            else if (fillMode == 1 && eventData.pointerId == -2)
                            {
                                RemoveConSlot(true);
                            }
                        }
                        else
                        {
                            if (fillMode == 0 && eventData.pointerId == -1)
                            {
                                AddConLink();
                            }
                            else if (fillMode == 1 && eventData.pointerId == -2)
                            {
                                RemoveConLink();
                            }
                            
                            DeselectConSlot();
                        }
                        break;
                    
                    case 0:
                        if (eventData.pointerId == -1)
                        {
                            gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                            selectedSlot = gameObject;
                        }
                        break;

                    case 1:
                        if (eventData.pointerId == -1)
                        {
                            if (selectedSlot != null)
                            {
                                selectedSlot.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = false;

                                AddConLink();
                            }

                            gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                            selectedSlot = gameObject;
                        }
                        break;
                }
            }
            else if (BuildUI.buildMode == 0 && BuildUI.selectedLink[0] != -1)
            {
                DeselectConSlot();
            }

            if (BuildUI.buildMode == 1 && BuildUI.selectedLink[1] != -1 && filled)
            {
                if (fillMode == 0 && !hasGravSlot)
                {
                    AddGravLink();
                }
                else if (fillMode == 1 && gravLines[BuildUI.selectedLink[1]].Contains(slotLocation))
                {
                    RemoveGravLink(BuildUI.selectedLink[1]);
                }
            }

            if (BuildUI.buildMode == 2)
            {
                if (fillMode == 0)
                {
                    AddSlot();
                }
                else if (fillMode == 1)
                {
                    RemoveSlot();
                }
            }
        }
    }

    public void ManualPointerEnter()
    {
        if (BuildUI.buildMode == 0 && BuildUI.selectedLink[0] != -1 && filled)
        {
            if (fillMode == 1 && canSelect)
            {
                RemoveConSlot(true);
            }
            
            if (fillMode == 0 && toolMode == 0)
            {
                if (selectedSlot != null)
                {
                    selectedSlot.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = false;
                    AddConLink();
                }

                gameObject.transform.parent.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                selectedSlot = gameObject;
            }
        }

        if (BuildUI.buildMode == 1 && BuildUI.selectedLink[1] != -1 && filled)
        {
            if (fillMode == 0 && !Input.GetKeyDown(KeyCode.Mouse0) && !hasGravSlot)
            {
                AddGravLink();
            }
            else if (fillMode == 1 && !Input.GetKeyDown(KeyCode.Mouse1) && gravLines[BuildUI.selectedLink[1]].Contains(slotLocation))
            {
                RemoveGravLink(BuildUI.selectedLink[1]);
            }
        }

        if (BuildUI.buildMode == 2)
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
    }

    void Start()
    {
        slotLocation = (gameObject.transform.parent.GetSiblingIndex(), gameObject.transform.parent.parent.GetSiblingIndex());
    }
}

