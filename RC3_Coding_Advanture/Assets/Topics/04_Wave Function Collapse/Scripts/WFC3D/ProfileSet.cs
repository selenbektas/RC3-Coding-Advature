using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DataIO;
using DataIO.Profiles;

#if UNITY_EDITOR
using UnityEditor;


#region Editor

[CustomEditor(typeof(ProfileSet))]
public class ProfileSetEditor : Editor
{

    string filePath;
    ProfileSet obj;


    private void OnEnable()
    {
        obj = (ProfileSet)target;
        EditorPrefs.GetString($"{target.GetInstanceID()}_filePath", "");

       
    }

    private void OnDestroy()
    {
        EditorPrefs.SetString($"{target.GetInstanceID()}_filePath", filePath);
    }

    private void OnDisable()
    {
        EditorPrefs.SetString($"{target.GetInstanceID()}_filePath", filePath);
    }

    public override void OnInspectorGUI()
    {
      
        GUILayout.Label("Read Profiles From Json");
        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(filePath);
        if (GUILayout.Button("Load"))
        {
            filePath= EditorUtility.OpenFilePanelWithFilters("Select File", "", new string[] { "JSON File","json" });

            if (filePath.Length != 0)
            {
                try
                {
                    var set = IOUtil.DeserializeJson<ProfileDataSet>(filePath);

                    obj.profiles = new List<Profile>();

                    foreach (var profileData in set.profiles)
                    {
                        obj.profiles.Add(new Profile(profileData));
                    }

                    EditorUtility.SetDirty(target);
                }
                catch
                {

                }
             
            }

        }
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Copy From Other"))
        {
            if (obj.othersToCopy == null || obj.othersToCopy.Count == 0)
            {
                return;
            }

            obj.profiles = new List<Profile>();
            foreach(var other in obj.othersToCopy)
            {
                if (other.profiles != null)
                {
                    obj.profiles.AddRange(other.profiles);
                }
              
            }
            EditorUtility.SetDirty(target);
        }

        GUILayout.Space(15f);

       

  
        DrawDefaultInspector();

    }

}
#endregion

#endif

[CreateAssetMenu(menuName ="RC3/SimpleWFC/ProfileSet",fileName ="New ProfileSet")]
public class ProfileSet : ScriptableObject
{

    public List<Profile> profiles;

    
    public List<ProfileSet> othersToCopy;

    public Profile Find(string label)
    {

        return profiles.Find(p => p.label == label);
    }
}
[Serializable]
public struct KeyIndex
{
    public string key;
    public int index;
}

[Serializable]
public class Profile
{
    public string label;
    public List<Vector3> points;
    public List<Shape> subShapes;


    public void DrawShape(Vector3 origin,Vector3 upward,Vector3 forward, float scale)
    {
        //Matrix4x4 matrix= Matrix4x4.TRS(origin,Quaternion.FromToRotation(Vector3.up,axis),Vector3.one*scale);

        //Plane plane;

        if (points != null)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 xAxis = Vector3.Cross(upward, forward);
                var from = origin + points[i].x * xAxis * scale + points[i].y * upward * scale + points[i].z * forward * scale;
                var to = origin + points[i + 1].x * xAxis * scale + points[i + 1].y * upward * scale + points[i + 1].z * forward * scale;

                Gizmos.DrawLine(from, to);
            }
        }

        if (subShapes != null)
        {
            foreach(var shape in subShapes)
            {

                for (int i = 0; i < shape.points.Count - 1; i++)
                {
                    Vector3 xAxis = Vector3.Cross(upward, forward);
                    var from = origin + (float)shape.points[i].x * xAxis * scale + (float)shape.points[i].y * upward * scale + (float)shape.points[i].z * forward * scale;
                    var to = origin + (float)shape.points[i + 1].x * xAxis * scale + (float)shape.points[i + 1].y * upward * scale + (float)shape.points[i + 1].z * forward * scale;

                    Gizmos.DrawLine(from, to);
                }
            }
        }

     
    }

    public Profile()
    {
        points = new List<Vector3>();
        subShapes = new List<Shape>();
    }

    public Profile(ProfileShapeData profileData)
    {
        label = profileData.label;
        points = new List<Vector3>();
        subShapes = new List<Shape>();
        foreach (var shape in profileData.shapes)
        {
            subShapes.Add(shape);
        }
    }
}