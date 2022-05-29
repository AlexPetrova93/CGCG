using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderPass : MonoBehaviour
{
    public Material depthMat;
    public Material normalsMat;

    public Material edgesMat;

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
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

    }

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
