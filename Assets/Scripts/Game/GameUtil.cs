using UnityEngine;

public class GameUtil : MonoBehaviour
{
    public static bool FirstTime = true;
    public static GameMode LastGameMode;
    public static int MissionTime;
    public static bool IsNewBestTime;

    private void Start()
    {
        ShowCursor();
        
        SetUpFPS();
        SetUpVSync();
    }
    
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void SetUpFPS()
    {
        SetFPS();
        Settings.Game.FPS.onUpdated += SetFPS;
    }

    private void SetUpVSync()
    {
        SetVSync();
        Settings.Game.VSync.onUpdated += SetVSync;
    }
    
    private void SetFPS()
    {
        var newFPSStr = Settings.Game.FPS.Value;
        var newFPS = newFPSStr == "Unlimited" ? -1 : int.Parse(newFPSStr);
        Application.targetFrameRate = newFPS;
    }
    
    private void SetVSync()
    {
        QualitySettings.vSyncCount = Settings.Game.VSync.Value ? 1 : 0;
    }
}
