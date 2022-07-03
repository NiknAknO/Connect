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
        
        oldMouseCoords = new float[2] {Camera.main.ScreenToWorldPoint(Input.mousePosition)[0], Camera.main.ScreenToWorldPoint(Input.mousePosition)[1]};
    }
    
    void Update()
    {
        newMouseCoords = new float[2] {Camera.main.ScreenToWorldPoint(Input.mousePosition)[0], Camera.main.ScreenToWorldPoint(Input.mousePosition)[1]};

        if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)) && !Input.GetKey(KeyCode.Mouse2)) FillLine(oldMouseCoords, newMouseCoords);

        newMouseCoords.CopyTo(oldMouseCoords, 0);
    }

    static void FillLine(float[] oldCoords, float[] newCoords)
    {
        float[] currentStep = {oldCoords[0], oldCoords[1]};
        float[] nextStep = new float[2];

        int[] currentSlot = {16 + Mathf.FloorToInt(oldCoords[0]/51), 15 - Mathf.FloorToInt(oldCoords[1]/51)};
        int[] endSlot = {16 + Mathf.FloorToInt(newCoords[0]/51), 15 - Mathf.FloorToInt(newCoords[1]/51)};

        int[] direction = {endSlot[0].CompareTo(currentSlot[0]), currentSlot[1].CompareTo(endSlot[1])};

        float slope = 0;
        float iSlope = 0;

        if (direction[0] != 0 || direction[1] != 0)
        {
            if (direction[0] != 0 && direction[1] != 0)
            {
                slope = (newCoords[1] - oldCoords[1])/(newCoords[0] - oldCoords[0]);
                iSlope = (newCoords[0] - oldCoords[0])/(newCoords[1] - oldCoords[1]);

                currentStep[0] = direction[0] * Mathf.Min(direction[0] * 51 * (currentSlot[0] - 16 + (direction[0]+1)/2), direction[0] * (iSlope*(51*(15 - currentSlot[1] + (direction[1]+1)/2) - oldCoords[1]) + oldCoords[0]));
                currentStep[1] = direction[1] * Mathf.Min(direction[1] * 51 * (15 - currentSlot[1] + (direction[1]+1)/2), direction[1] * (slope*(51*(currentSlot[0] - 16 + (direction[0]+1)/2) - oldCoords[0]) + oldCoords[1]));
            }
            else if (direction[0] != 0)
            {
                slope = (newCoords[1] - oldCoords[1])/(newCoords[0] - oldCoords[0]);
                
                currentStep[0] = 51*(currentSlot[0] - 16 + (direction[0]+1)/2);
                currentStep[1] = slope * (currentStep[0] - oldCoords[0]) + oldCoords[1];
            }
            else
            {
                iSlope = (newCoords[0] - oldCoords[0])/(newCoords[1] - oldCoords[1]);

                currentStep[1] = 51*(15 - currentSlot[1] + (direction[1]+1)/2);
                currentStep[0] = iSlope*(currentStep[1] - oldCoords[1]) + oldCoords[0];
            }

            while (!currentSlot.SequenceEqual(endSlot))
            {
                if (direction[0] != 0 && direction[1] != 0)
                {
                    nextStep[0] = direction[0] * Mathf.Min(51*(Mathf.Floor(direction[0]*currentStep[0]/51) + 1), direction[0] * (iSlope*(direction[1]*51*(Mathf.Floor(direction[1]*currentStep[1]/51) + 1) - oldCoords[1]) + oldCoords[0]), direction[0] * newCoords[0]);
                    nextStep[1] = direction[1] * Mathf.Min(51*(Mathf.Floor(direction[1]*currentStep[1]/51) + 1), direction[1] * (slope*(direction[0]*51*(Mathf.Floor(direction[0]*currentStep[0]/51) + 1) - oldCoords[0]) + oldCoords[1]), direction[1] * newCoords[1]);
                }
                else if (direction[0] != 0)
                {
                    nextStep[0] = direction[0] * Mathf.Min(51*(Mathf.Floor(direction[0]*currentStep[0]/51) + 1), direction[0] * newCoords[0]);
                    nextStep[1] = nextStep[0] == newCoords[0] ? slope * (nextStep[0] - oldCoords[0]) + oldCoords[1] : newCoords[1];
                }
                else
                {
                    nextStep[1] = direction[1] * Mathf.Min(51*(Mathf.Floor(direction[1]*currentStep[1]/51) + 1), direction[1] * newCoords[1]);
                    nextStep[0] = nextStep[1] == newCoords[1] ? slope * (nextStep[1] - oldCoords[1]) + oldCoords[0] : newCoords[0];
                }

                currentSlot[0] = Mathf.FloorToInt((currentStep[0]+nextStep[0])/102f) + 16;
                currentSlot[1] = 15 - Mathf.FloorToInt((currentStep[1]+nextStep[1])/102f);

                if (!OnUI(currentStep, nextStep)) TriggerManualPointerEnter(currentSlot);

                nextStep.CopyTo(currentStep, 0);
            }
        }
    }

    static void TriggerManualPointerEnter(int[] slot)
    {   
        if (slot[0] >= 0 && slot[0] < 32 && slot[1] >= 0 && slot[1] < 32) GridClick.board.transform.GetChild(slot[1]).GetChild(slot[0]).GetChild(0).GetComponent<GridClick>().ManualPointerEnter();
    }

    static bool OnUI(float[] oldCoords, float[] newCoords)
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
}