using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FillRect : MonoBehaviour
{
    static int[] oldSlot;
    static int[] newSlot;

    public static int[] selectedSlot;
    
    static List<(int, int)> filledSlots;

    void Start()
    {
        oldSlot = new int[2];
        newSlot = new int[2];

        filledSlots = new List<(int, int)>{};
    }

    void Update()
    {
        newSlot[0] = 16 + Mathf.FloorToInt(DrawInterpolate.newMouseCoords[0]/51);
        newSlot[1] = 15 - Mathf.FloorToInt(DrawInterpolate.newMouseCoords[1]/51);
        
        if (BuildUI.buildMode == 2 && GridClick.toolMode == 0)
        {   
            if ((Input.GetKeyUp(KeyCode.Mouse1) || (Input.GetKeyUp(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1)) || Input.GetKeyDown(KeyCode.Mouse1)) && selectedSlot != null)
            {
                ApplyFill(true);
            }

            if (!Input.GetKey(KeyCode.Mouse2) && !newSlot.SequenceEqual(oldSlot) && selectedSlot != null)
            {
                RectFill(selectedSlot, newSlot);
            }

            if (((GridClick.fillMode == 0 && Input.GetKeyDown(KeyCode.Mouse0)) || (GridClick.fillMode == 1 && Input.GetKeyDown(KeyCode.Mouse1))) && (!EventSystem.current.IsPointerOverGameObject() || GridClick.clickedBoard) && selectedSlot == null)
            {
                selectedSlot = new int[] {newSlot[0], newSlot[1]};
                RectFill(selectedSlot, selectedSlot);
            }
        }

        newSlot.CopyTo(oldSlot, 0);
    }

    static void RectFill(int[] corner1, int[] corner2)
    {
        int[] minCorner = new int[] {Mathf.Min(corner1[0], corner2[0]), Mathf.Min(corner1[1], corner2[1])};
        int[] maxCorner = new int[] {Mathf.Max(corner1[0], corner2[0]), Mathf.Max(corner1[1], corner2[1])};

        List<(int, int)> newFilledSlots = new List<(int, int)>{};

        if (GridClick.fillMode == 0)
        {
            foreach ((int, int) slot in filledSlots)
            {
                if (slot.Item1 < minCorner[0] || slot.Item1 > maxCorner[0] || slot.Item2 < minCorner[1] || slot.Item2 > maxCorner[1]) GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetComponent<Image>().enabled = false;
            }

            for (int i = minCorner[1]; i <= maxCorner[1]; i++)
            {
                for (int j = minCorner[0]; j <= maxCorner[0]; j++)
                {
                    if (j >= 0 && j < 32 && i >= 0 && i < 32 && (filledSlots.Contains((j, i)) || !GridClick.board.transform.GetChild(i).GetChild(j).GetChild(0).GetComponent<GridClick>().filled))
                    {
                        GridClick.board.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = true;
                        newFilledSlots.Add((j, i));
                    }
                }
            }
        }
        else if (GridClick.fillMode == 1)
        {
            foreach ((int, int) slot in filledSlots)
            {
                if (slot.Item1 < minCorner[0] || slot.Item1 > maxCorner[0] || slot.Item2 < minCorner[1] || slot.Item2 > maxCorner[1]) GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetComponent<Image>().enabled = true;
            }

            for (int i = minCorner[1]; i <= maxCorner[1]; i++)
            {
                for (int j = minCorner[0]; j <= maxCorner[0]; j++)
                {
                    if (j >= 0 && j < 32 && i >= 0 && i < 32 && (filledSlots.Contains((j, i)) || GridClick.board.transform.GetChild(i).GetChild(j).GetChild(0).GetComponent<GridClick>().filled))
                    {
                        GridClick.board.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                        newFilledSlots.Add((j, i));
                    }
                }
            }
        }

        filledSlots = newFilledSlots;
    }

    public static void ApplyFill(bool save)
    {
        if (save)
        {
            foreach ((int, int) slot in filledSlots)
            {
                if (GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetChild(0).GetComponent<GridClick>().filled)
                {
                    GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetChild(0).GetComponent<GridClick>().RemoveSlot();
                }
                else
                {
                    GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetChild(0).GetComponent<GridClick>().AddSlot();
                }
            }
        }
        else
        {
            foreach ((int, int) slot in filledSlots)
            {
                GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetComponent<Image>().enabled = GridClick.board.transform.GetChild(slot.Item2).GetChild(slot.Item1).GetChild(0).GetComponent<GridClick>().filled;
            }
        }

        filledSlots = new List<(int,int)>{};
        selectedSlot = null;
    }
}
