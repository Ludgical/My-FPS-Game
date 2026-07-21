using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip backgroundMusic;
    
    private Coroutine changeVolumeRoutine;
    
    private void Start()
    {
        refs = References.Refs;
        
        StartMusic();
        
        refs.gameLogic.onPlay += StopMusic;
        refs.gameLogic.onCompleted += StartMusic;

        refs.gameLogic.onChangeVolume += () =>
        {
            //Change the volume of the audio mixer
            //-80 db is silent
            //Divide by 50 instead of 100 to make louder sounds even louder
            var volumeDecibels = Settings.Game.Volume == 0 ? -80 : Mathf.Log10(Settings.Game.Volume / 50) * 20;
            audioMixer.SetFloat("Volume", volumeDecibels);
        };
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
}
