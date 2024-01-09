using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBegin : MonoBehaviour
{
    public bool begin;
    private void OnMouseDown()
    {
        begin = true;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;

    }
}
