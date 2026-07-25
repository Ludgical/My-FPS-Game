using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        refs = References.Refs;
        
        fadeImage.gameObject.SetActive(false);
        
        var data = refs.gameData;
        refs.gameLogic.onCompleted += () =>
        {
            //Fade the screen to black so that the scene resets once it's completely black
            FadeToBlack(data.completedToResetDelay - data.fadeToBlackDuration - 0.1f, data.fadeToBlackDuration);
            //Fade the screen back once the scene has reset
            FadeFromBlack(data.completedToResetDelay + 0.1f, data.fadeFromBlackDuration);
        };
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

        //Show the image once the fading starts
        if (toBlack)
            fadeImage.gameObject.SetActive(true);

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
        
        //Hide the image once the fading stops
        if (!toBlack)
            fadeImage.gameObject.SetActive(false);
    }
}
