using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject sensObject;
    [SerializeField] private GameObject fovObject;
    [SerializeField] private GameObject toggleCrouchObject;
    [SerializeField] private GameObject volumeObject;
    
    public static readonly List<Setting> SettingsList = new();

    private void Awake()
    {
        Player.Sensitivity = new SliderSetting("Player.Sensitivity", sensObject, 50);
        Player.FOV = new SliderSetting("Player.FOV", fovObject, 70);
        Player.ToggleCrouch = new ButtonSetting("Player.Toggle Crouch", toggleCrouchObject);
        Game.MasterVolume = new SliderSetting("Game.Master Volume", volumeObject, 50);
    }

    public static class Player
    {
        public static SliderSetting Sensitivity;
        public static SliderSetting FOV;
        public static ButtonSetting ToggleCrouch;
    }

    public static class Game
    {
        public static SliderSetting MasterVolume;
    }
    
    /// Save the float representation of the value of every setting using PlayerPrefs
    public static void Save()
    {
        foreach (var setting in SettingsList)
            PlayerPrefs.SetFloat(setting.Name, setting.FloatValue);
        
        PlayerPrefs.Save();
    }
    
    private void OnApplicationQuit()
    {
        Save();
    }
}
