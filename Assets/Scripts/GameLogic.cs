using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    private References refs;
    
    public Action onPlay;
    public Action onResetScene;
    public Action onCompleted;
    
    [SerializeField] private Button timedButton;
    [SerializeField] private Button tutorialButton;

    public bool gameIsOn;

    private void Start()
    {
        refs = References.Refs;
        
        timedButton.onClick.AddListener(() =>
        {
            onPlay?.Invoke();
        });
        tutorialButton.onClick.AddListener(() =>
        {
            onPlay?.Invoke();
        });
        
        onPlay += () =>
        {
            gameIsOn = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        };
        onResetScene += () =>
        {
            gameIsOn = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        };
    }

    public void OnGameCompleted()
    {
        onCompleted?.Invoke();
        StartCoroutine(InvokeOnCompleted());
        return;

        IEnumerator InvokeOnCompleted()
        {
            yield return new WaitForSeconds(refs.gameData.completedToResetDelay);
            onResetScene?.Invoke();
        }
    }
}
