using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{




    public void startMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void connectGame()
    {
        SceneManager.LoadScene(1);
    }

    public void chessGame()
    {
        SceneManager.LoadScene(2);
    }
    
}
