using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyUIHandler : MonoBehaviour
{
    private float currentValue;

    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeValueSlider()
    {


    }

    public void ChangeValueTextField()
    {
        string text = inputField.text;
        bool isFloat = float.TryParse(text, out float value);

        if (isFloat)
        {
            // change the slider 
            if (slider)
            {

            }

            currentValue = value;
        }
        else
        {
            inputField.text = "";
        }

    }
}
