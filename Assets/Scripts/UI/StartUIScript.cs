using System.Collections;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class StartUIScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private Button timedButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text bestTimeText;

    private void Start()
    {
        refs = References.Refs;

        SetUpPlayButtons();
        
        SetBestTime(refs.gameLogic.GetBestTime());
        
        //Show UI on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);
    }
    
    private void SetUpPlayButtons()
    {
        timedButton.onClick.AddListener(() =>
        {
            refs.gameLogic.onPlayTimed?.Invoke();
        });
        
        tutorialButton.onClick.AddListener(() =>
        {
            refs.gameLogic.onPlayTutorial?.Invoke();
        });
        
        quitButton.onClick.AddListener(() =>
        {
            StartCoroutine(QuitRoutine());
            return;
            
            IEnumerator QuitRoutine()
            {
                yield return new WaitForSeconds(0.2f);
                Application.Quit();
            }
        });
    }

    private void SetBestTime(int time)
    {
        if (time >= 0)
            bestTimeText.text = $"Best Time: {time}";
    }
}
