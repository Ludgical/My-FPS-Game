using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource backgroundMusicSource;

    private SliderSetting[] volumeCategories;
    
    private Coroutine changeVolumeRoutine;
    
    private void Start()
    {
        refs = References.Refs;

        volumeCategories = new[]
        {
            Settings.Volume.MasterVolume,
            Settings.Volume.MusicVolume,
            Settings.Volume.GameVolume,
            Settings.Volume.PlayerVolume,
            Settings.Volume.DroneVolume,
            Settings.Volume.OtherVolume
        };
        
        StartMusic();
        SetMixerVolumes();
        
        refs.gameLogic.onPlay += StopMusic;
    }

    private void StartMusic()
    {
        //Stop the previous routine and fade in the music
        if (changeVolumeRoutine != null)
            StopCoroutine(changeVolumeRoutine);
        changeVolumeRoutine = StartCoroutine(ChangeVolume(1, 7));
    }
    
    private void StopMusic()
    {
        //Stop the previous routine and fade out the music
        if (changeVolumeRoutine != null)
            StopCoroutine(changeVolumeRoutine);
        changeVolumeRoutine = StartCoroutine(ChangeVolume(0, 1));
    }

    private IEnumerator ChangeVolume(float newVolume, float fadeDurationSeconds)
    {
        //Start the music if it's not getting turned off
        if (newVolume > 0f)
            backgroundMusicSource.Play();
        
        //The amount of steps to take to get to the new volume
        var stepCount = (int)(fadeDurationSeconds * 50);
        //The amount the volume changes by on a step
        var volumeChangePerStep = (newVolume - backgroundMusicSource.volume) / stepCount;
        
        for (var i = 0; i < stepCount; i++)
        {
            //Move the volume closer to the goal and wait for the next step
            backgroundMusicSource.volume += volumeChangePerStep;
            yield return new WaitForSeconds(fadeDurationSeconds / stepCount);
        }
        
        //Set the volume to the correct volume
        backgroundMusicSource.volume = newVolume;

        //Stop the music if it's getting turned off
        if (newVolume == 0)
            backgroundMusicSource.Stop();
    }

    private void SetMixerVolumes()
    {
        foreach (var category in volumeCategories)
        {
            SetMixerVolume(category);
            category.onUpdated += () =>
                SetMixerVolume(category);
        }
    }
    
    private void SetMixerVolume(SliderSetting setting)
    {
        //-80 db is silent
        //Divide by 50 instead of 100 to make louder sounds even louder
        var newVolume = setting.Value;
        var volumeDecibels = newVolume == 0 ? -80 : Mathf.Log10(newVolume / 50) * 20;
        var settingName = setting.Name.Split(".")[1];
        audioMixer.SetFloat($"{settingName}Volume", volumeDecibels);
    }
}
