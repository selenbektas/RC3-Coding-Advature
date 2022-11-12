using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSolverMap : MonoBehaviour
{
    [SerializeField] Texture2D[] textures;
    [SerializeField] Solver2D solver;
    [SerializeField] RawImage preview;

    public void SetMap(int index)
    {
        solver.map = textures[index];
        preview.texture = textures[index];
    }

  
}
