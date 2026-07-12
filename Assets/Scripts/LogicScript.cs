using UnityEngine;

public class LogicScript : MonoBehaviour
{
    [SerializeField] private PlayerScript player;
    [SerializeField] private DoorScript door;
    [SerializeField] private GameObject startUI;
    [SerializeField] private MusicScript music;

    public bool gameIsOn;
    
    [Header("Constants")]
    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float jumpStrength;
    public float gravityScale;
    public float colliderHeight;
    public float movementLerpStepGround;
    public float movementLerpStepAir;
    public float speedForRunAnimation;
    
    [Header("Settings")]
    public float sensitivity;
    public float fov;
    public bool toggleCrouch;

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
        door.OpenDoor();
        startUI.SetActive(false);
        music.StopMusic();
        
        //Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    [ContextMenu("completed")]
    private void OnGameCompleted()
    {
        gameIsOn = false;
        door.CloseDoor();
        startUI.SetActive(true);
        music.StartMusic();
        
        //Teleport player
        player.ResetPlayer();
        
        //Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
