using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSet3D : MonoBehaviour
{
    public List<Tile3D> tileSet;


    public Tile3D this[int index]
    {
        get => tileSet[index];
    }

    public int Count
    {
        get => tileSet.Count;
    }
}
