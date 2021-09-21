using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public GameObject go;
    public void OnMouseEnter() { 
    
        go.SetActive(true);
    }
    public void OnMouseExit()
    {
        go.SetActive(false);
    }
}
