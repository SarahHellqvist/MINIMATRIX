using UnityEngine;

public class AudioClips : MonoBehaviour
{
    [SerializeField]
    AudioClip playerTileSound;
    [SerializeField]
    AudioClip aITileSound;

    public void PlayPlayerSound(AudioSource source)
    {
        source.PlayOneShot(playerTileSound);
    }

    public void PlayAISound(AudioSource source)
    {
        source.PlayOneShot(aITileSound);
    }
}
