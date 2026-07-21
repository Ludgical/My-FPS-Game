using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private References refs;
    
    private void Start()
    {
        refs = References.Refs;

        SetFOV();
        
        refs.gameLogic.onChangeFOV += SetFOV;
    }

    private void SetFOV()
    {
        refs.camera.fieldOfView = Settings.Player.FOV;
    }
}
