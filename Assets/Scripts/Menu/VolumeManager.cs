using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider globalVolSlider;
    public Slider musicVolSlider;
    public Slider fXVolSlider;

    public void SetGlobalVolume()
    {
        Sound.Instance.GlobalVolume = globalVolSlider.value;
        Sound.Instance.UpdateMusicVolume();
        Sound.Instance.UpdateFXVolume();
    }

    public void SetMusicVolume()
    {
        Sound.Instance.MusicVolume = musicVolSlider.value;
        Sound.Instance.UpdateMusicVolume();
    }

    public void SetFXVolume()
    {
        Sound.Instance.EffectsVolume = fXVolSlider.value;
        Sound.Instance.UpdateFXVolume();
    }
}
