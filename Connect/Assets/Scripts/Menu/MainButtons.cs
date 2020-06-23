using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButtons : MonoBehaviour
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
}
