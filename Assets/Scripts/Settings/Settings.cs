using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject sensObject;
    [SerializeField] private GameObject fovObject;
    [SerializeField] private GameObject toggleCrouchObject;
    [SerializeField] private GameObject gunSwayObject;
    [SerializeField] private GameObject masterVolumeObject;
    [SerializeField] private GameObject musicVolumeObject;
    [SerializeField] private GameObject gameVolumeObject;
    [SerializeField] private GameObject playerVolumeObject;
    [SerializeField] private GameObject droneVolumeObject;
    [SerializeField] private GameObject otherVolumeObject;
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
        Game.FPS = new CycleButtonSetting("Game.FPS", FPSObject,
            new []{ "60", "120", "144", "180", "240", "Unlimited" }, defaultValueIndex:5);
        Game.VSync = new ButtonSetting("Game.VSync", VSyncObject);
        
        //Volume settings
        Volume.MasterVolume = new SliderSetting("Volume.Master", masterVolumeObject, 50);
        Volume.MusicVolume = new SliderSetting("Volume.Music", musicVolumeObject, 100);
        Volume.GameVolume = new SliderSetting("Volume.Game", gameVolumeObject, 100);
        Volume.PlayerVolume = new SliderSetting("Volume.Player", playerVolumeObject, 100);
        Volume.DroneVolume = new SliderSetting("Volume.Drone", droneVolumeObject, 100);
        Volume.OtherVolume = new SliderSetting("Volume.Other", otherVolumeObject, 100);
    }

    public static class Player
    {
        public static SliderSetting Sensitivity;
        public static SliderSetting FOV;
        public static ButtonSetting ToggleCrouch;
        public static CycleButtonSetting GunSway;
    }

    public static class Volume
    {
        public static SliderSetting MasterVolume;
        public static SliderSetting MusicVolume;
        public static SliderSetting GameVolume;
        public static SliderSetting PlayerVolume;
        public static SliderSetting DroneVolume;
        public static SliderSetting OtherVolume;
    }

    public static class Game
    {
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
