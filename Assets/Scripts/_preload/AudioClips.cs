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
    AudioClip PlayerWinSound;


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
