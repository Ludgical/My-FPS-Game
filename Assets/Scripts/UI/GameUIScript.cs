using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIScript : MonoBehaviour
{
    private References refs;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        refs = References.Refs;
        
        SetUpFading();
        SetUpCrosshair();

        if (!GameLogic.FirstTime)
        {
            //Fade the screen from black once the scene has reset
            //as long as it's not the first round this session
            fadeImage.color = Color.black;
            FadeFromBlack(0.2f, refs.gameData.fadeFromBlackDuration);
        }
    }

    private void SetUpFading()
    {
        refs.gameLogic.onCompleted += () =>
        {
            //Fade the screen to black so that the scene resets once it's completely black
            FadeToBlack(
                refs.gameData.completedToResetDelay - refs.gameData.fadeToBlackDuration, 
                refs.gameData.fadeToBlackDuration);
        };
    }

    private void SetUpCrosshair()
    {
        crosshair.SetActive(false);
        refs.gameLogic.onPlay += () => crosshair.SetActive(true);
    }

    private void FadeToBlack(float waitTime, float duration)
    {
        StartCoroutine(FadeRoutine(toBlack:true, waitTime, duration));
    }

    private void FadeFromBlack(float waitTime, float duration)
    {
        StartCoroutine(FadeRoutine(toBlack:false, waitTime, duration));
    }
    
    private IEnumerator FadeRoutine(bool toBlack, float waitTime, float duration)
    {
        yield return new WaitForSeconds(waitTime);

        var color = fadeImage.color;

        //Fade the alpha value of the color in / out over the duration
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Clamp01(toBlack 
                ? t / duration
                : 1 - t / duration);
            fadeImage.color = color;
            yield return null;
        }

        //Set the alpha to the exact value it should be
        color.a = toBlack ? 1f : 0f;
        fadeImage.color = color;
    }
}
