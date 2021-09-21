using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject Panel;
    public void StartMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void ConnectGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ChessGame()
    {
        SceneManager.LoadScene(3);
    }   

    public void OpenPanel()
    {
        if(Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }

    public void ClosePanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }
}
