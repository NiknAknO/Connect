using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float sensitivity;

    float[] oldMouseCoords;
    float[] newMouseCoords;

    float[] newPositionChange;

    float[] newCameraPosition;
    
    float[] zoomCoords;

    float zoomFactor;

    void Start()
    {
        oldMouseCoords = new float[2];
        newMouseCoords = new float[2];

        newPositionChange = new float[2];

        newCameraPosition = new float[2];

        zoomCoords = new float[2];

        newMouseCoords[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[0];
        newMouseCoords[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[1];

        newMouseCoords.CopyTo(oldMouseCoords, 0);
        
        sensitivity = -1f;
    }

    void Update()
    {
        newMouseCoords[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[0] - transform.position.x;
        newMouseCoords[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[1] - transform.position.y;

        newPositionChange[0] = newMouseCoords[0] - oldMouseCoords[0];
        newPositionChange[1] = newMouseCoords[1] - oldMouseCoords[1];
        
        if (Input.GetKey(KeyCode.Mouse2))
        {
            newCameraPosition[0] = Mathf.Clamp(transform.position.x + sensitivity * (newPositionChange[0]), -5, 5);
            newCameraPosition[1] = Mathf.Clamp(transform.position.y + sensitivity * (newPositionChange[1]), -5, 5);

            transform.position = new Vector3 (newCameraPosition[0], newCameraPosition[1], -1);
        }
        
        if ((0.375f < Camera.main.orthographicSize && Input.mouseScrollDelta.y > 0) || (6 > Camera.main.orthographicSize && Input.mouseScrollDelta.y < 0))
        {
            zoomFactor = Mathf.Pow(2, Input.mouseScrollDelta.y);

            zoomCoords[0] = Mathf.Clamp(transform.position.x + ((zoomFactor - 1) * newMouseCoords[0]) / zoomFactor, -5, 5);
            zoomCoords[1] = Mathf.Clamp(transform.position.y + ((zoomFactor - 1) * newMouseCoords[1]) / zoomFactor, -5, 5);

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize / zoomFactor, 0.375f, 6);
            transform.position = new Vector3(zoomCoords[0], zoomCoords[1], -1);

            newMouseCoords[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[0] - transform.position.x;
            newMouseCoords[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition)[1] - transform.position.y;
        }

        newMouseCoords.CopyTo(oldMouseCoords, 0);
    }
}
