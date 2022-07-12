using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSelectable : MonoBehaviour
{
    BuildUI selecter;

    int index;

    void Start()
    {
        selecter = GameObject.Find("Script Manager").GetComponent<BuildUI>();
        index = gameObject.transform.GetSiblingIndex();
    }

    public void SelectWindow()
    {
        if (index != BuildUI.selectedLink[BuildUI.buildMode]) selecter.SelectLinkWindow(index);
    }
}