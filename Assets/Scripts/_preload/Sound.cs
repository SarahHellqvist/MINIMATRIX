using UnityEngine;

public class Sound : MonoBehaviour
{
    public static Sound Instance { get; set; }

    private float globalVolume;
    private float effectsVolume;
    private float musicVolume;

    [SerializeField]
    private AudioSource fXSource;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioClips ac;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.Log("Warning: multiple " + this + " in scene!");
        if (fXSource == null)
            fXSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = GetComponentInChildren<AudioSource>();
        if (ac == null)
            ac = GetComponent<AudioClips>();
        Instance.GlobalVolume = 1.0f;
        Instance.EffectsVolume = 0.25f;
        Instance.MusicVolume = 0.25f;
        UpdateMusicVolume();
    }

    public void UpdateMusicVolume()
    {
        musicSource.volume = MusicVolume;
    }

    public void UpdateFXVolume()
    {
        fXSource.volume = EffectsVolume;
    }

    public AudioSource Audio
    {
        get => fXSource;
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
