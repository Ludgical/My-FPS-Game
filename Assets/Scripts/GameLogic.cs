using System;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public Action onPlay;
    public Action onCompleted;
    
    [SerializeField] private Button timedButton;
    [SerializeField] private Button tutorialButton;

    public bool gameIsOn;

    private void Start()
    {
        timedButton.onClick.AddListener(() =>
        {
            onPlay.Invoke();
        });
        tutorialButton.onClick.AddListener(() =>
        {
            onPlay.Invoke();
        });
        
        onPlay += () =>
        {
            gameIsOn = true;
            Settings.Save();
            
            //Hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        };
        onCompleted += () =>
        {
            gameIsOn = false;
            
            //Show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        };
    }
    
    [ContextMenu("completed")]
    private void OnGameCompleted()
    {
        onCompleted.Invoke();
    }
}
