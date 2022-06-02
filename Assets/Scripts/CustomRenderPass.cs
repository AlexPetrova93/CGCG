using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderPass : MonoBehaviour
{
    [SerializeField] private ControlPanel shaderProperties;

    //[Header("Edge detection")]
    //[Range(0, 1)] public float depthEdgeThreshold = 0.01f;
    [Range(0, 10)] public float normalEdgeThreshold = 0.01f;

    //[Header("Noise")]
    //public Vector2 noiseScale = new Vector2(100, 100);
    //public Vector4 uncertaintyMatrix = Vector4.one;

    [Header("Materials")]
    public Material edgeDetectionMat;
    public Material sketchyMat;

    //public Material depthNormalsMat;

    //public Material depthMat;
    //public Material normalsMat;

    //public Material edgesMat;

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        GeneratePerlinNoise();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // get temp texture for the edge map
        RenderTexture edgeMap = RenderTexture.GetTemporary(src.width, src.height);

        // get edge map
        edgeDetectionMat.SetFloat("_ThresholdDepth", shaderProperties.depthEdgeThreshold.ToFloat());
        edgeDetectionMat.SetFloat("_ThresholdNormal", shaderProperties.normalEdgeThreshold.ToFloat());
        Graphics.Blit(src, edgeMap, edgeDetectionMat);

        // apply sketchy effect and render to screen
        sketchyMat.SetTexture("_EdgeMap", edgeMap);
        sketchyMat.SetVector("_UncertaintyMatrix", shaderProperties.uncertaintyMatrix.ToVector4());
        Graphics.Blit(src, dest, sketchyMat);

        // release temp textures
        RenderTexture.ReleaseTemporary(edgeMap);
    }

    public void GeneratePerlinNoise() => GeneratePerlinNoise(Screen.width, Screen.height);

    private void GeneratePerlinNoise(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RFloat, false);

        float xScale = (1f / width) * shaderProperties.noiseScale.ToVector2().x;
        float yScale = (1f / width) * shaderProperties.noiseScale.ToVector2().y;

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
