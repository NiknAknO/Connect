using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
   public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Build()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit() 
    {
        Application.Quit();
    }

    void Start() {
        //Screen.SetResolution(2560, 1440, true);
    }
}
