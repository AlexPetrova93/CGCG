using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ShaderFloatValue
{
    [HideInInspector] public float current;
    [HideInInspector] public float min;
    [HideInInspector] public float max;
    [HideInInspector] public UIFloatProperty uiProperty;

    public void Init(float current, float min, float max)
    {
        this.current = current;
        this.min = min;
        this.max = max;
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

    public void Start()
    {
        components = GetComponentsInChildren<UIFloatProperty>();
        foreach (var component in components)
        {
            component.Init(min, max);
        }
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
