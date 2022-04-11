using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderPass : MonoBehaviour
{
    public Material material;

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
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
