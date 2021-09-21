using UnityEngine;

public class AudioClips : MonoBehaviour
{
    [SerializeField]
    AudioClip playerTileSound;
    [SerializeField]
    AudioClip aITileSound;
    [SerializeField]
    AudioClip PlayerWinSound;

    [SerializeField]
    AudioSource audioSource;

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
        audioSource.PlayOneShot(PlayerWinSound);
    }
}
