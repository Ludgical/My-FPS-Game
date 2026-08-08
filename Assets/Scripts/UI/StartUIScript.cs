using System.Collections;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class StartUIScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private Button timedButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonPressSound;

    private void Start()
    {
        refs = References.Refs;

        SetUpPlayButtons();
        SetUpOnButtonPressed();
        
        //Show UI on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);
    }
    
    private void SetUpPlayButtons()
    {
        timedButton.onClick.AddListener(() =>
        {
            refs.gameLogic.onPlay?.Invoke();
        });
        
        tutorialButton.onClick.AddListener(() =>
        {
            refs.gameLogic.onPlay?.Invoke();
        });
        
        quitButton.onClick.AddListener(() =>
        {
            StartCoroutine(QuitRoutine());
            return;
            
            IEnumerator QuitRoutine()
            {
                yield return new WaitForSeconds(0.1f);
                Application.Quit();
            }
        });
    }

    private void SetUpOnButtonPressed()
    {
        foreach (var button in GetComponentsInChildren<Button>())
            button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        audioSource.PlayOneShot(buttonPressSound);
    }
}
