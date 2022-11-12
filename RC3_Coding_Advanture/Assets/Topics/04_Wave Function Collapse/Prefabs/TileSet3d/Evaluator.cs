using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluator : ScriptableObject
{
    public float multiplier = 1.0f;

   public float Evaluate(GameObject obj)
    {
        return Mathf.Sin(obj.transform.position.y * obj.transform.position.z * multiplier);
    }
}
