using UnityEngine;

public class AudioClips : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip playerTileSound;
    [SerializeField]
    AudioClip aITileSound;
    [SerializeField]
    AudioClip playerWinSound;
    [SerializeField]
    AudioClip aIWinSound;
    [SerializeField]
    AudioClip restartSound;
    [SerializeField]
    AudioClip buttonSound;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = Sound.Instance.Audio;
    }

    public void PlayPlayerSound()
    {
        audioSource.PlayOneShot(playerTileSound);
    }

    public void PlayAISound()
    {
        audioSource.PlayOneShot(aITileSound);
    }

    public void PlayPlayerWinSound()
    {
        audioSource.PlayOneShot(playerWinSound);
    }

    public void PlayAIWinSound()
    {
        audioSource.PlayOneShot(aIWinSound);
    }

    public void PlayRestartSound()
    {
        audioSource.PlayOneShot(restartSound);
    }
    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
}
