using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The 2d tile class that has only 4 strings as labels of each edge of a squre.
/// </summary>
public class Tile2D : MonoBehaviour
{
    //labels used to identify the types of each edge.
    public string label_left;
    public string label_right;
    public string label_behind;
    public string label_forward;

    public float param;

}
