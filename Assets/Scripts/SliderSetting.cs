using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : Setting
{
    public float Value { get; private set; }
    
    private readonly Slider slider;
    private readonly TMP_Text valueText;
    
    public SliderSetting(string name, GameObject sliderObject, float defaultValue = 0)
    {
        //Find the slider and the text field displaying the value of the setting
        slider = sliderObject.GetComponentInChildren<Slider>();
        valueText = slider.GetComponentInChildren<TMP_Text>();
        
        //When the slider has moved, call the OnSettingChanged method
        slider.onValueChanged.AddListener(_ => OnSettingChanged());
        
        Initialize(name, defaultValue);
    }

    protected override void SetInitialValue(float defaultValue)
    {
        var savedValue = GetSavedValue();
        FloatValue = savedValue >= 0 ? savedValue : defaultValue;
        Value = FloatValue;
        slider.value = Value;
    }

    protected override void SetValue()
    {
        Value = slider.value;
        FloatValue = Value;
    }

    protected override void SetText()
    {
        valueText.text = Value.ToString(CultureInfo.InvariantCulture);
    }
}
