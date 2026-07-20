using System.Collections;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;
    
    private Coroutine changeVolumeRoutine;
    
    private void Start()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        //Stop the previous routine and fade in the music
        if (changeVolumeRoutine != null)
            StopCoroutine(changeVolumeRoutine);
        changeVolumeRoutine = StartCoroutine(ChangeVolume(1, 7));
    }
    
    public void StopMusic()
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
            audioSource.Play();
        
        //The amount of steps to take to get to the new volume
        var stepCount = (int)(fadeDurationSeconds * 50);
        //The amount the volume changes by on a step
        var volumeChangePerStep = (newVolume - audioSource.volume) / stepCount;
        
        for (var i = 0; i < stepCount; i++)
        {
            //Move the volume closer to the goal and wait for the next step
            audioSource.volume += volumeChangePerStep;
            yield return new WaitForSeconds(fadeDurationSeconds / stepCount);
        }
        
        //Set the volume to the correct volume
        audioSource.volume = newVolume;

        //Stop the music if it's getting turned off
        if (newVolume == 0)
            audioSource.Stop();
    }
}
