using System;
using System.Collections;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private References refs;
    
    public Action onPlay;
    public Action onResetScene;
    public Action onCompleted;

    public bool gameIsOn;

    private void Start()
    {
        refs = References.Refs;
        
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
