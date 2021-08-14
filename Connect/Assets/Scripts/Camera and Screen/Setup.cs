using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        QualitySettings.vSyncCount = 0;
    }
}
