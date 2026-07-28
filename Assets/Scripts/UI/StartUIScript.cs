using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        
        //Show UI on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);
    }
    
    private void SetUpPlayButtons()
    {
        timedButton.onClick.AddListener(() =>
        {
            OnButtonPressed();
            refs.gameLogic.onPlay?.Invoke();
        });
        
        tutorialButton.onClick.AddListener(() =>
        {
            OnButtonPressed();
            refs.gameLogic.onPlay?.Invoke();
        });
        
        quitButton.onClick.AddListener(() =>
        {
            OnButtonPressed();
            StartCoroutine(QuitRoutine());
            return;
            
            IEnumerator QuitRoutine()
            {
                yield return new WaitForSeconds(0.1f);
                Application.Quit();
            }
        });
    }

    public void OnButtonPressed()
    {
        audioSource.PlayOneShot(buttonPressSound);
    }
}
