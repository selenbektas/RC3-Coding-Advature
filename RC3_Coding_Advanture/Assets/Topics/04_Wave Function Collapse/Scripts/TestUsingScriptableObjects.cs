using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="MyScriptableObjects/Shared Stuff", fileName = "new my scriptable object")]
public class TestUsingScriptableObjects : ScriptableObject
{
    [TextArea]
    public string text;
    public GameObject my_objects;
    public Texture myTextrue;

    public List<Material> myMaterials;

}

