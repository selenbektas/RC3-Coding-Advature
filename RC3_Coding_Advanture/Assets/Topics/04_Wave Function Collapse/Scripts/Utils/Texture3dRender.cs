using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class Texture3dRender : MonoBehaviour
{
    [SerializeField] Material tex3dRenderMaterial;

    [SerializeField]Texture3D tex3d;



    public Texture3dRender Instance
    {
        get;private set;
    }

    public void OnEnable()
    {
        Instance = this;
    }

    public void OnDisable()
    {
        Instance = null;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        tex3dRenderMaterial.SetTexture("_V3d", tex3d);
        Graphics.Blit(source, destination, tex3dRenderMaterial);
    }
}
