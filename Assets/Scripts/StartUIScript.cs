using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartUIScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private Slider sensSlider;
    [SerializeField] private TMP_Text sensText;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_Text fovText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Button toggleCrouchButton;
    [SerializeField] private TMP_Text toggleCrouchText;

    private void Start()
    {
        refs = References.Refs;
        
        SetUpUI();
        
        //Show UI on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);
        refs.gameLogic.onCompleted += () => gameObject.SetActive(true);
        
        SetUpOnChangeSens();
        SetUpOnChangeFov();
        SetUpOnChangeVolume();
        SetUpOnPressToggleCrouch();
    }

    private void SetUpUI()
    {
        //Set the values of the sliders
        sensSlider.value = Settings.Player.Sensitivity;
        fovSlider.value = Settings.Player.FOV;
        volumeSlider.value = Settings.Game.Volume;
        
        //Set the text on the sliders and buttons
        SetSensText();
        SetFOVText();
        SetToggleCrouchText();
        SetVolumeText();
    }
    
    private void SetUpOnChangeSens()
    {
        sensSlider.onValueChanged.AddListener(_ =>
        {
            refs.gameLogic.onChangeSensitivity?.Invoke();
        });
        
        refs.gameLogic.onChangeSensitivity += () =>
        {
            Settings.Player.Sensitivity = sensSlider.value;
            SetSensText();
        };
    }

    private void SetUpOnChangeFov()
    {
        fovSlider.onValueChanged.AddListener(_ =>
        {
            refs.gameLogic.onChangeFOV?.Invoke();
        });
        
        refs.gameLogic.onChangeFOV += () =>
        {
            Settings.Player.FOV = fovSlider.value;
            SetFOVText();
        };
    }

    private void SetUpOnChangeVolume()
    {
        volumeSlider.onValueChanged.AddListener(_ =>
        {
            refs.gameLogic.onChangeVolume?.Invoke();
        });
        
        refs.gameLogic.onChangeVolume += () =>
        {
            Settings.Game.Volume = volumeSlider.value;
            SetVolumeText();
        };
    }

    private void SetUpOnPressToggleCrouch()
    {
        toggleCrouchButton.onClick.AddListener(() =>
        {
            refs.gameLogic.onPressToggleCrouch?.Invoke();
        });
        
        refs.gameLogic.onPressToggleCrouch += () =>
        {
            Settings.Player.ToggleCrouch = !Settings.Player.ToggleCrouch;
            SetToggleCrouchText();
        };
    }

    private void SetSensText()
    {
        sensText.text = Settings.Player.Sensitivity.ToString(CultureInfo.InvariantCulture);
    }
    private void SetFOVText()
    {
        fovText.text = Settings.Player.FOV.ToString(CultureInfo.InvariantCulture);
    }
    private void SetVolumeText()
    {
        volumeText.text = Settings.Game.Volume.ToString(CultureInfo.InvariantCulture);
    }
    private void SetToggleCrouchText()
    {
        toggleCrouchText.text = Settings.Player.ToggleCrouch ? "Toggle Crouch: ON" : "Toggle Crouch: OFF";
    }
}
