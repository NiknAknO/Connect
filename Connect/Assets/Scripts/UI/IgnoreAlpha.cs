using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IgnoreAlpha : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Image> ().alphaHitTestMinimumThreshold = 1;
    }
}
