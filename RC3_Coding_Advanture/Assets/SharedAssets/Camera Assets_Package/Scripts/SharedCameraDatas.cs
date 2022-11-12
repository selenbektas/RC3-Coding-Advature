using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraDataUtilities
{
    [CreateAssetMenu(fileName ="CameraList",menuName ="CameraSaving/CameraDataList")]
    [System.Serializable]
    public class SharedCameraDatas : ScriptableObject
    {
        [SerializeField]public List<CameraDatas> savedCameras = new List<CameraDatas>();
        public void Initialize()
        {
            savedCameras = new List<CameraDatas>();
        }

        public void Add(CameraDatas camdatas)
        {

            savedCameras.Add(camdatas);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

        }

        public void Override(int index,CameraDatas camdatas)
        {
            savedCameras[index] = camdatas;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void Remove(int index)
        {
            savedCameras.RemoveAt(index);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}

