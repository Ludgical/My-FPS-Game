using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private References refs;
    
    public Action onPlay;
    public Action onCompleted;

    public bool gameIsOn;
    public static bool FirstTime = true;

    private void Start()
    {
        refs = References.Refs;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        onPlay += () =>
        {
            gameIsOn = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        };
    }

    public void OnGameCompleted()
    {
        onCompleted?.Invoke();
        StartCoroutine(ResetScene());
        return;

        IEnumerator ResetScene()
        {
            yield return new WaitForSeconds(refs.gameData.completedToResetDelay);
            Settings.Save();
            FirstTime = false;
            SceneManager.LoadScene("GameScene");
        }
    }
}
