using UnityEngine;

public static class Settings
{
    public static class Player
    {
        public static float Sensitivity
        {
            get => PlayerPrefs.GetFloat("Sensitivity", 100f);
            set => PlayerPrefs.SetFloat("Sensitivity", value);
        }

        public static float FOV
        {
            get => PlayerPrefs.GetFloat("FOV", 70f);
            set => PlayerPrefs.SetFloat("FOV", value);
        }

        public static bool ToggleCrouch
        {
            get => PlayerPrefs.GetInt("ToggleCrouch", 0) == 1;
            set => PlayerPrefs.SetInt("ToggleCrouch", value ? 1 : 0);
        }
    }

    public static class Game
    {
        public static float Volume
        {
            get => PlayerPrefs.GetFloat("Volume", 50f);
            set => PlayerPrefs.SetFloat("Volume", value);
        }
    }
    
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
