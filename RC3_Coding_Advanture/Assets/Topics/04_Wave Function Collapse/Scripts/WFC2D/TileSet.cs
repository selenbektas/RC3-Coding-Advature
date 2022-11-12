using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSet : MonoBehaviour
{
    public List<Tile2D> tileSet;


    public Tile2D this[int index]
    {
        get => tileSet[index];
    }

    public int Count
    {
        get => tileSet.Count;
    }
}
