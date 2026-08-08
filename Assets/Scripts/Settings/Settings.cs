using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject sensObject;
    [SerializeField] private GameObject fovObject;
    [SerializeField] private GameObject toggleCrouchObject;
    [SerializeField] private GameObject gunSwayObject;
    [SerializeField] private GameObject volumeObject;
    [SerializeField] private GameObject FPSObject;
    [SerializeField] private GameObject VSyncObject;
    
    public static readonly List<Setting> SettingsList = new();

    private void Awake()
    {
        //Player settings
        Player.Sensitivity = new SliderSetting("Player.Sensitivity", sensObject, defaultValue:50);
        Player.FOV = new SliderSetting("Player.FOV", fovObject, defaultValue:70);
        Player.ToggleCrouch = new ButtonSetting("Player.Toggle Crouch", toggleCrouchObject);
        Player.GunSway = new CycleButtonSetting("Player.Gun Sway", gunSwayObject, 
            new []{ "None", "Low", "High" }, defaultValueIndex:2);
        
        //Game settings
        Game.MasterVolume = new SliderSetting("Game.Master Volume", volumeObject, 50);
        Game.FPS = new CycleButtonSetting("Game.FPS", FPSObject,
            new []{ "60", "120", "144", "180", "240", "Unlimited" }, defaultValueIndex:5);
        Game.VSync = new ButtonSetting("Game.VSync", VSyncObject);
    }

    public static class Player
    {
        public static SliderSetting Sensitivity;
        public static SliderSetting FOV;
        public static ButtonSetting ToggleCrouch;
        public static CycleButtonSetting GunSway;
    }

    public static class Game
    {
        public static SliderSetting MasterVolume;
        public static CycleButtonSetting FPS;
        public static ButtonSetting VSync;
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
