using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StartUIScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private Slider sensSlider;
    [SerializeField] private TMP_Text sensText;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_Text fovText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private TMP_Text toggleCrouchText;

    private void Start()
    {
        refs = References.Refs;

        //Set up sliders and button
        sensSlider.value = Settings.Player.Sensitivity;
        OnChangeSens();
        fovSlider.value = Settings.Player.FOV;
        OnChangeFov();
        SetToggleCrouchText();
        volumeSlider.value = Settings.Game.Volume;
        OnChangeVolume();
        
        //Show on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);;
        refs.gameLogic.onCompleted += () => gameObject.SetActive(true);;
    }
    
    public void OnChangeSens()
    {
        //Change the sensitivity in the settings and on the slider
        var newSens = sensSlider.value;
        Settings.Player.Sensitivity = newSens;
        sensText.text = newSens.ToString(CultureInfo.InvariantCulture);
    }

    public void OnChangeFov()
    {
        //Change the FOV in the settings, on the slider and on the camera
        var newFov = fovSlider.value;
        Settings.Player.FOV = newFov;
        fovText.text = newFov.ToString(CultureInfo.InvariantCulture);
        refs.camera.fieldOfView = newFov;

        //Set the gun's z-value 
        var gunPos = refs.gunPivot.localPosition;
        gunPos.z = -0.00667f * newFov + 1.24f;
        refs.gunPivot.localPosition = gunPos;
    }

    public void OnChangeVolume()
    {
        //Change the volume in the settings, on the slider and on the camera
        var newVolume = volumeSlider.value;
        Settings.Game.Volume = newVolume;
        volumeText.text = newVolume.ToString(CultureInfo.InvariantCulture);
        
        //Change the volume of the audio mixer
        //-80 db is silent
        //Divide by 50 instead of 100 to make louder sounds even louder
        var volumeDecibels = newVolume == 0 ? -80 : Mathf.Log10(newVolume / 50) * 20;
        audioMixer.SetFloat("Volume", volumeDecibels);
    }

    public void OnPressToggleCrouch()
    {
        //Invert toggle crouch in the settings and change the text on the button
        Settings.Player.ToggleCrouch = !Settings.Player.ToggleCrouch;
        SetToggleCrouchText();
    }

    private void SetToggleCrouchText()
    {
        toggleCrouchText.text = Settings.Player.ToggleCrouch ? "Toggle Crouch: ON" : "Toggle Crouch: OFF";
    }
}
