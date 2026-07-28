using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private References refs;
    
    private void Start()
    {
        refs = References.Refs;

        SetFOV();
        Settings.Player.FOV.onUpdated += SetFOV;
    }

    private void SetFOV()
    {
        refs.camera.fieldOfView = Settings.Player.FOV.Value;
    }

    private void ResetCamera()
    {
        refs.camera.transform.localRotation = Quaternion.identity;
    }
}
