using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CycleButtonSetting : Setting
{
    public string Value { get; private set; }
    public int ValueIndex => (int)FloatValue;

    /// The values to cycle through when you press the button
    private readonly string[] Values;

    private readonly TMP_Text valueText;
    
    public CycleButtonSetting(string name, GameObject buttonObject, string[] values, int defaultValueIndex = 0)
    {
        //Find the button and the text field displaying the value of the setting
        var button = buttonObject.GetComponentInChildren<Button>();
        valueText = buttonObject.GetComponentInChildren<TMP_Text>();
        
        Values = values;
        
        //When the button is pressed, call OnSettingChanged
        button.onClick.AddListener(OnSettingChanged);
        
        Initialize(name, defaultValueIndex);
    }
    
    protected override void SetInitialValue(float defaultValueIndex)
    {
        var savedValue = GetSavedValue();
        FloatValue = savedValue >= 0 ? savedValue : defaultValueIndex;
        Value = Values[(int)FloatValue];
    }

    protected override void SetValue()
    {
        var newValueIndex = (FloatValue + 1) % Values.Length;
        FloatValue = newValueIndex;
        Value = Values[(int)FloatValue];
    }

    protected override void SetText()
    {
        var settingName = Name.Split('.').Last();
        valueText.text = $"{settingName}: {Value}";
    }
}
