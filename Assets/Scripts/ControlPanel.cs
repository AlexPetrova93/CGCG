using UnityEngine;


public class ControlPanel : MonoBehaviour
{
    [SerializeField] private CustomRenderPass customRenderPass;

    // edge detection
    public bool edgesOn = true;
    public ShaderProperty depthEdgeThreshold;
    public ShaderProperty normalEdgeThreshold;

    // noise
    public bool uncertaintyOn = true;
    public ShaderProperty noiseScale;
    public ShaderProperty uncertaintyMatrix;


    // Edge detection
    //[Range(0, 1)] public static float depthEdgeThreshold = 0.01f;
    //[Range(0, 10)] public static float normalEdgeThreshold = 0.01f;

    // Noise
    //public static float[] noiseScale = new float[2] { 100, 100 };
    //public static float[] uncertaintyMatrix = new float[4] { 2, 0, 0, 2 };

    private void Start()
    {
        foreach (var component in noiseScale.components)
        {
            component.onValueChanged.AddListener(OnNoiseScaleChanged);
        }
    }

    public void OnNoiseScaleChanged(float _)
    {
        customRenderPass.GeneratePerlinNoise();
    }

    public void SwitchEdgesOn() => edgesOn = !edgesOn;
    public void SwitchUncertaintyOn() => uncertaintyOn = !uncertaintyOn;


}

public static class ShaderPropertiesExtensions
{
    public static float ToFloat(this ShaderProperty property)
    {
        float[] value = property.GetValue();
        if (value.Length < 1) return 0;
        else return value[0];
    }

    public static Vector2 ToVector2(this ShaderProperty property)
    {
        float[] value = property.GetValue();
        if (value.Length < 2) return Vector2.zero;
        else return new Vector2(value[0], value[1]);
    }

    public static Vector4 ToVector4(this ShaderProperty property)
    {
        float[] value = property.GetValue();
        if (value.Length < 4) return Vector4.zero;
        else return new Vector4(value[0], value[1], value[2], value[3]);
    }

    public static float[] ToFloatArray(this float value)
    {
        return new float[] { value };
    }

    public static float[] ToFloatArray(this Vector2 value)
    {
        return new float[] { value.x, value.y };
    }

    public static float[] ToFloatArray(this Vector4 value)
    {
        return new float[] { value.x, value.y, value.z, value.w };
    }
}
