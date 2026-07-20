using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private DoorScript startRoomDoor;
    [SerializeField] private MusicScript musicScript;

    public bool gameIsOn;

    private void Start()
    {
        refs = References.Refs;
    }

    public void OnPlayTimed()
    {
        OnPlay();
    }

    public void OnPlayTutorial()
    {
        OnPlay();
    }

    private void OnPlay()
    {
        gameIsOn = true;
        Settings.Save();
        startRoomDoor.OpenDoor(1.2f);
        refs.startUI.SetActive(false);
        refs.gunObject.SetActive(true);
        musicScript.StopMusic();
        
        //Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    [ContextMenu("completed")]
    private void OnGameCompleted()
    {
        gameIsOn = false;
        startRoomDoor.CloseDoor();
        refs.startUI.SetActive(true);
        refs.gunObject.SetActive(false);
        musicScript.StartMusic();
        
        //Teleport player to the start
        refs.player.ResetPlayer();
        
        //Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
