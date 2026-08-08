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
        
        SetFPS();
        Settings.Game.FPS.onUpdated += SetFPS;
        
        SetVSync();
        Settings.Game.VSync.onUpdated += SetVSync;
        
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

    private void SetFPS()
    {
        var newFPSStr = Settings.Game.FPS.Value;
        var newFPS = newFPSStr == "Unlimited" ? -1 : int.Parse(newFPSStr);
        Application.targetFrameRate = newFPS;
    }
    
    private void SetVSync()
    {
        var vSyncCount = Settings.Game.VSync.Value ? 1 : 0;
        QualitySettings.vSyncCount = vSyncCount;
    }
}
