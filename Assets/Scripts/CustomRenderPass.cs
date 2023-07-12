using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderPass : MonoBehaviour
{
    //[SerializeField] private ControlPanel shaderProperties;

    [Header("Edge detection")]
    public float depthEdgeThreshold = 0.01f;
    public float normalEdgeThreshold = 0.01f;

    [Header("Noise")]
    public Vector2 noiseScale = new Vector2(100, 100);
    public Vector4 uncertaintyMatrix = new Vector4(10, 0, 0, 10);
    public Texture2D NoiseMap;
    public float NoiseScale = 1;
    public float NoiseIntensity = 1;

    public float normalEdgesModifier = 1;

    public bool EdgesOn;
    public bool NoiseOn;

    [Header("Materials")]
    public Material edgeDetectionMat;
    public Material sketchyMat;


    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
        GeneratePerlinNoise();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // get temp texture for the edge map
        RenderTexture edgeMap = RenderTexture.GetTemporary(src.width, src.height);

        // get edge map
        edgeDetectionMat.SetFloat("_ThresholdDepth", depthEdgeThreshold);
        edgeDetectionMat.SetFloat("_ThresholdNormal", normalEdgeThreshold);
        sketchyMat.SetFloat("_NormalModifier", normalEdgesModifier);
        Graphics.Blit(src, edgeMap, edgeDetectionMat);

        // apply sketchy effect and render to screen
        sketchyMat.SetTexture("_EdgeMap", edgeMap);
        sketchyMat.SetVector("_UncertaintyMatrix", uncertaintyMatrix);
        sketchyMat.SetInt("_EdgesOn", EdgesOn ? 1 : 0);
        sketchyMat.SetInt("_UncertaintyOn", NoiseOn ? 1 : 0);
        sketchyMat.SetTexture("_NoiseMap", NoiseMap);
        sketchyMat.SetFloat("_NoiseScale", NoiseScale);
        sketchyMat.SetFloat("_NoiseIntensity", NoiseIntensity);
        Graphics.Blit(src, dest, sketchyMat);

        // release temp textures
        RenderTexture.ReleaseTemporary(edgeMap);
    }

    public void GeneratePerlinNoise() => GeneratePerlinNoise(256, 256);

    private void GeneratePerlinNoise(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RFloat, false);

        float xScale = (1f / width) * noiseScale.x;
        float yScale = (1f / width) * noiseScale.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * xScale, y * yScale);

                tex.SetPixel(x, y, new Color(noise, 0, 0));
            }
        }

        tex.Apply();
        sketchyMat.SetTexture("_NoiseMap", tex);
    }


    /*void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // get temp texture for depth and normals
        RenderTexture depthNormalsTex = RenderTexture.GetTemporary(src.width, src.height);

        // get depth and normals
        Graphics.Blit(src, depthNormalsTex, depthNormalsMat);

        // pass depth and normals to edge shader
        Graphics.Blit(depthNormalsTex, dest, edgesMat);


        // release temp textures
        RenderTexture.ReleaseTemporary(depthNormalsTex);

    }*/

    /*void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // get temp textures for depth, normals
        RenderTexture depthTex = RenderTexture.GetTemporary(src.width, src.height);
        RenderTexture normalsTex = RenderTexture.GetTemporary(src.width, src.height);

        // get depth
        Graphics.Blit(src, depthTex, depthMat);

        // get normals
        Graphics.Blit(src, normalsTex, normalsMat);

        // pass depth and normals to edge shader
        edgesMat.SetTexture("_DepthTex", depthTex);
        edgesMat.SetTexture("_NormalsTex", normalsTex);
        Graphics.Blit(src, dest, edgesMat);


        // release temp textures
        RenderTexture.ReleaseTemporary(depthTex);
        RenderTexture.ReleaseTemporary(normalsTex);

    }*/

    //void OnRenderImage(RenderTexture src, RenderTexture dest)
    //{
    //    // allocate a temp render texture the same dimensions as src
    //    RenderTexture tmp = RenderTexture.GetTemporary(src.width, src.height);

    //    // 1st pass
    //    Graphics.Blit(src, tmp, material);

    //    // 2nd pass
    //    Graphics.Blit(tmp, dest, material); 

    //    RenderTexture.ReleaseTemporary(tmp);
    //}
}
