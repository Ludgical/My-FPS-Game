using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetting : Setting
{
    public bool Value { get; private set; }

    private readonly TMP_Text valueText;
    
    public ButtonSetting(string name, GameObject buttonObject, bool defaultValue = false)
    {
        //Find the button and the text field displaying the value of the setting
        var button = buttonObject.GetComponentInChildren<Button>();
        valueText = buttonObject.GetComponentInChildren<TMP_Text>();
        
        //When the button is pressed, call the OnSettingChanged method
        button.onClick.AddListener(OnSettingChanged);
        button.onClick.AddListener(() => References.Refs.startUI.OnButtonPressed());
        
        Initialize(name, defaultValue ? 1 : 0);
    }

    protected override void SetInitialValue(float defaultValue)
    {
        var savedValue = GetSavedValue();
        FloatValue = savedValue >= 0 ? savedValue : defaultValue;
        Value = FloatValue != 0;
    }

    protected override void SetValue()
    {
        Value = !Value;
        FloatValue = Value ? 1 : 0;
    }

    protected override void SetText()
    {
        var settingName = Name.Split('.').Last();
        valueText.text = settingName + ": " + (Value ? "ON" : "OFF");
    }
}
