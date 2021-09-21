using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
    }
}
