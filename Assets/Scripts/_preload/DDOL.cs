using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene(1);
#if UNITY_EDITOR
        SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
#endif
        
    }
}
