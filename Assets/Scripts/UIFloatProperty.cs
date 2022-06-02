using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFloatProperty : MonoBehaviour
{
    [SerializeField] private float defaultValue;

    [HideInInspector] public ShaderFloatValue value;

    [HideInInspector] public UnityEvent<float> onValueChanged;

    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TMP_InputField inputField;

    [SerializeField] private string numberFormat = "0.00";

    public void Init(float min, float max)
    {
        value = new ShaderFloatValue(defaultValue, min, max);
        onValueChanged.AddListener(value.ChangeValue);

        if (slider)
        {
            slider.minValue = value.min;
            slider.maxValue = value.max;
            slider.value = value.current;
        }

        inputField.text = value.current.ToString(numberFormat);

        SetUI();
    }

    public void SetUI()
    {
        if (slider)
        {
            slider.value = value.current;
        }
        inputField.text = value.current.ToString(numberFormat);
    }

    public void ChangeValueSlider()
    {
        value.current = slider.value;
        inputField.text = value.current.ToString(numberFormat);
        onValueChanged.Invoke(value.current);
    }

    public void ChangeValueTextField()
    {
        string text = inputField.text;
        bool isFloat = float.TryParse(text, out float current);

        if (isFloat)
        {
            value.current = Mathf.Clamp(current, value.min, value.max);
            inputField.text = value.current.ToString(numberFormat);

            // change the slider 
            if (slider)
            {
                slider.value = value.current;
            }

            onValueChanged.Invoke(value.current);
        }
        else
        {
            inputField.text = "";
        }

    }

    private void OnDestroy()
    {
        onValueChanged.RemoveAllListeners();
    }
}
