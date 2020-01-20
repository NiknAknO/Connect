using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickResponse : MonoBehaviour
{
    public void whenClicked()
    {
        RaycastHit2D hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (hit = Physics2D.Raycast(ray.origin, new Vector2(0, 0)))
        {
            Debug.Log(hit.collider.name);
        }
    }
}
