using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    public int age = 0;

    public MeshRenderer meshRenderer;

    Material material;

    public void Init()
    {
        material = new Material(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = material;
    }

    public void SetID(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetID(int x,int y,int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void SetAge(int age)
    {
        this.age = age;
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

    public void SetState(int state)
    {
        meshRenderer.enabled = state == 0 ? false : true;
    }
}
