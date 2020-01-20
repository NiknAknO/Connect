using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRun : MonoBehaviour
{
    public static int buildState;
    /*
    public static int[,] blocks;
    public static int[][][,] links;
    public static int[][][,] gravityLines;
    */
    void Start()
    {
        /*
        blocks = new int[64, 64];
        links = new int[1024][1024][,];
        gravityLines = new int[1024][1024][,];
        */
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buildState = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buildState = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buildState = 3;
        }
    }

    public static void revert (int removeID)
    {
        switch (removeID)
        {
            case 1:

                break;

            case 2:

                break;

            case 3:

                break;
        }
    }
}
