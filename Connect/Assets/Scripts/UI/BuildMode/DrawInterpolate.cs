using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawInterpolate : MonoBehaviour
{
    float[] oldMouseCoords;
    float[] newMouseCoords;

    int[] test = new int[2];

    void Start()
    {
        oldMouseCoords = new float[2];
        newMouseCoords = new float[2];
        
        oldMouseCoords = FixRound(new float[2] {Camera.main.ScreenToWorldPoint(Input.mousePosition)[0], Camera.main.ScreenToWorldPoint(Input.mousePosition)[1]});
    }
    
    void Update()
    {
        newMouseCoords = FixRound(new float[2] {Camera.main.ScreenToWorldPoint(Input.mousePosition)[0], Camera.main.ScreenToWorldPoint(Input.mousePosition)[1]});

        if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)) && !Input.GetKey(KeyCode.Mouse2)) FillLine(oldMouseCoords, newMouseCoords);

        newMouseCoords.CopyTo(oldMouseCoords, 0);
    }
    
    void FillColumn(float[] oldCoords, float[] newCoords)
    {
        int direction = 0;
        float[][] iterCoords = new float[2][];

        if (oldCoords[1] < newCoords[1])
        {
            direction = 1;
        }
        else if (oldCoords[1] > newCoords[1])
        {
            direction = -1;
        }
        else if (oldCoords[1] == newCoords[1]) 
        {
            TriggerManualPointerEnter(oldCoords, newCoords);
            return;
        }

        float dx = (newCoords[0] - oldCoords[0])/(newCoords[1] - oldCoords[1]);

        iterCoords[0] = new float[2] {oldCoords[0], oldCoords[1]};
        iterCoords[1] = FixRound(new float[2] {dx*(direction*Mathf.Min(direction*(51*(Mathf.Floor(oldCoords[1]/51) + (direction+1)/2)), direction*newCoords[1]) - oldCoords[1]) + oldCoords[0], direction*Mathf.Min(direction*(51*(Mathf.Floor(oldCoords[1]/51) + (direction+1)/2)), direction*newCoords[1])});
        
        while (iterCoords[1][1] != newCoords[1] || (direction == 1 && Mathf.Floor(iterCoords[0][1]/51) != Mathf.Floor(iterCoords[1][1]/51) && Mathf.Floor(iterCoords[0][0]/51) == Mathf.Floor(iterCoords[1][0]/51)))
        {
            TriggerManualPointerEnter(iterCoords[0], iterCoords[1]);

            iterCoords[0] = new float[2] {iterCoords[1][0], iterCoords[1][1]};
            iterCoords[1] = FixRound(new float[2] {dx*(direction*Mathf.Min(direction*iterCoords[0][1] + 51, direction*newCoords[1]) - iterCoords[0][1]) + iterCoords[0][0], direction*Mathf.Min(direction*iterCoords[0][1] + 51, direction*newCoords[1])});
        }
        
        TriggerManualPointerEnter(iterCoords[0], iterCoords[1]);
    }

    void FillLine(float[] oldCoords, float[] newCoords)
    {
        int direction = 0;
        float[][] iterCoords = new float[2][];

        if (oldCoords[0] < newCoords[0])
        {
            direction = 1;
        }
        else if (oldCoords[0] > newCoords[0])
        {
            direction = -1;
        }
        else if (oldCoords[0] == newCoords[0]) 
        {
            FillColumn(oldCoords, newCoords);
            return;
        }
        
        float dy = (newCoords[1] - oldCoords[1])/(newCoords[0] - oldCoords[0]);

        iterCoords[0] = new float[2] {oldCoords[0], oldCoords[1]};
        iterCoords[1] = FixRound(new float[2] {direction*Mathf.Min(direction*(51*(Mathf.Floor(oldCoords[0]/51) + (direction+1)/2)), direction*newCoords[0]), dy*(direction*Mathf.Min(direction*(51*(Mathf.Floor(oldCoords[0]/51) + (direction+1)/2)), direction*newCoords[0]) - oldCoords[0]) + oldCoords[1]});
        
        while (iterCoords[1][0] != newCoords[0] || (direction == 1 && Mathf.Floor(iterCoords[0][0]/51) != Mathf.Floor(iterCoords[1][0]/51)))
        {
            FillColumn(iterCoords[0], iterCoords[1]);

            iterCoords[0] = new float[2] {iterCoords[1][0], iterCoords[1][1]};
            iterCoords[1] = FixRound(new float[2] {direction*Mathf.Min(direction*iterCoords[0][0] + 51, direction*newCoords[0]), dy*(direction*Mathf.Min(direction*iterCoords[0][0] + 51, direction*newCoords[0]) - iterCoords[0][0]) + iterCoords[0][1]});
        }
        
        FillColumn(iterCoords[0], iterCoords[1]);
    }

    void TriggerManualPointerEnter(float[] oldCoords, float[] newCoords)
    {
        float[] averageCoords = new float[2] {(oldCoords[0] + newCoords[0])/2, (oldCoords[1] + newCoords[1])/2};
        int[] slot = new int[2] {Mathf.FloorToInt(averageCoords[0]/51) + 16, 15 - Mathf.FloorToInt(averageCoords[1]/51)};
        
        if ((Mathf.Floor(averageCoords[0]/51) != Mathf.Floor(oldMouseCoords[0]/51) || Mathf.Floor(averageCoords[1]/51) != Mathf.Floor(oldMouseCoords[1]/51)) && !OnUI(oldCoords, newCoords) && slot[0] >= 0 && slot[0] <= 31 && slot[1] >= 0 && slot[1] <= 31) GridClick.board.transform.GetChild(slot[1]).GetChild(slot[0]).GetChild(0).GetComponent<GridClick>().ManualPointerEnter();
    }

    bool OnUI(float[] oldCoords, float[] newCoords)
    {
        float[] oldScreenCoords = {Camera.main.WorldToScreenPoint(new Vector3(oldCoords[0], oldCoords[1], 0)).x * 1920/Screen.width - 960, Camera.main.WorldToScreenPoint(new Vector3(oldCoords[0], oldCoords[1], 0)).y * 1080/Screen.height - 540};
        float[] newScreenCoords = {Camera.main.WorldToScreenPoint(new Vector3(newCoords[0], newCoords[1], 0)).x * 1920/Screen.width - 960, Camera.main.WorldToScreenPoint(new Vector3(newCoords[0], newCoords[1], 0)).y * 1080/Screen.height - 540};

        float[][] uiBoxes = {new float[]{-928, -528, -450, 350}, new float[]{-928, -828, 408, 508}, new float[]{-778, -678, 408, 508}, new float[]{-628, -528, 408, 508}, new float[]{783, 845.5f, -508, -445.5f}, new float[]{865.5f, 928, -508, -445.5f} };
        
        foreach(float[] uiBox in uiBoxes)
        {
            if (!uiBox.Equals(uiBoxes[0]) || BuildUI.buildMode == 0 || BuildUI.buildMode == 1)
            {
                if (oldScreenCoords[0] > uiBox[0] && oldScreenCoords[0] < uiBox[1] && oldScreenCoords[1] > uiBox[2] && oldScreenCoords[1] < uiBox[3] && newScreenCoords[0] > uiBox[0] && newScreenCoords[0] < uiBox[1] && newScreenCoords[1] > uiBox[2] && newScreenCoords[1] < uiBox[3]) return true;
            }
        }
        
        return false;
    }

    float[] FixRound(float[] coords)
    {
        return new float[2] {Mathf.Round(coords[0] * 10000)/10000, Mathf.Round(coords[1] * 10000)/10000};
    }
}
