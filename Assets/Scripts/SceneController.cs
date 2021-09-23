using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject settingsPanel;

    public void StartMenu()
    {
        Sound.Instance.PlayMenuSong();
        SceneManager.LoadScene(1);
    }

    public void ConnectGame()
    {
        Sound.Instance.PlayConnectFourSong();
        SceneManager.LoadScene(2);
    }

    public void ChessGame()
    {
        Sound.Instance.PlayChessSong();
        SceneManager.LoadScene(3);
    }

    public void OpenGamePanel()
    {
        if (gamePanel != null)
        {
            bool isActive = gamePanel.activeSelf;
            gamePanel.SetActive(!isActive);
        }
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }

    public void CloseGamePanel()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
