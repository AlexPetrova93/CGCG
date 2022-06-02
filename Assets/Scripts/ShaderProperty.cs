using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShaderFloatValue
{
    public float current;
    public float min;
    public float max;

    public ShaderFloatValue(float current, float min, float max)
    {
        this.current = current;
        this.min = min;
        this.max = max;
        Debug.Log("this.current is " + this.current);
    }

    public void ChangeValue(float value)
    {
        current = value;
    }
}

public class ShaderProperty : MonoBehaviour
{
    [HideInInspector] public UIFloatProperty[] components;

    public float min = 0;
    public float max = 100;

    [HideInInspector] public UnityEvent onValueChanged;

    public void Start()
    {
        components = GetComponentsInChildren<UIFloatProperty>();
        foreach (var component in components)
        {
            component.Init(min, max);
            onValueChanged.AddListener(component.SetUI);
        }
    }

    public void SetCurrent(float[] value)
    {
        for (int i = 0; i < components.Length; i++)
        {
            components[i].value.current = value[i];
        }
        onValueChanged.Invoke();
    }

    public float[] GetValue()
    {
        float[] ret = new float[components.Length];
        for (int i = 0; i < components.Length; i++)
        {
            ret[i] = components[i].value.current;
        }
        return ret;
    }
}
