using UnityEngine;

public class Sound : MonoBehaviour
{
    public static Sound Instance { get; set; }

    private float globalVolume;
    private float effectsVolume;
    private float musicVolume;

    [SerializeField]
    private new AudioSource audio;
    [SerializeField]
    private AudioClips ac;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.Log("Warning: multiple " + this + " in scene!");
        if (audio == null)
            audio = GetComponent<AudioSource>();
        if (ac == null)
            ac = GetComponent<AudioClips>();
        Instance.GlobalVolume = 0.5f;
        Instance.EffectsVolume = 1.0f;
        Instance.MusicVolume = 1.0f;
        UpdateMusicVolume();
    }

    public void UpdateMusicVolume()
    {
        audio.volume = MusicVolume;
    }

    public AudioSource Audio
    {
        get => audio;
    }

    public float GlobalVolume
    {
        get => globalVolume;
        set => globalVolume = value;
    }

    public float EffectsVolume
    {
        get => effectsVolume * globalVolume;
        set => effectsVolume = value;
    }

    public float EffectsVolumeRaw
    {
        get => effectsVolume;
    }

    public float MusicVolume
    {
        get => musicVolume * globalVolume;
        set => musicVolume = value;
    }

    public float MusicVolumeRaw
    {
        get => musicVolume;
    }

    public AudioClips AudioClips
    {
        get => ac;
    }
}
