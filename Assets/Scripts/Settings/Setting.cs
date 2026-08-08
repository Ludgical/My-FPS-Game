using System;
using UnityEngine;

public abstract class Setting
{
    /// The name of this setting
    public string Name { get; private set; }
    /// Float representation of the value of this setting
    public float FloatValue { get; protected set; }
    /// Gets invoked when the value of the setting has changed
    public Action onUpdated;
    
    protected void Initialize(string name, float defaultValue)
    {
        Name = name;
        
        SetInitialValue(defaultValue);
        SetText();
        
        Settings.SettingsList.Add(this);
    }

    protected void OnSettingChanged()
    {
        SetValue();
        SetText();
        // Other scripts can add methods that will get called once the setting has changed to this action
        onUpdated?.Invoke();
    }
    
    /// Returns the last saved value of this setting. Returns -1 if there is no saved value
    protected float GetSavedValue() => PlayerPrefs.GetFloat(Name, -1);

    /// Sets the value and float value of the setting to be the last saved value,
    /// or the default if there is no saved value
    protected abstract void SetInitialValue(float defaultValue);
    /// Called when the slider has been moved / button has been pressed.
    /// Changes the value of the setting so that other scripts can use it
    protected abstract void SetValue();
    /// Displays the current value of the setting on the text field
    protected abstract void SetText();
}
