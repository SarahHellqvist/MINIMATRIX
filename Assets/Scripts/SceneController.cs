using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject Panel;
    public void StartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ConnectGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ChessGame()
    {
        SceneManager.LoadScene(2);
    }   

    public void OpenPanel()
    {
        if(Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
