using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CameraDataUtilities
{
    [Serializable]
    public class CameraDatas
    {
        public Vector3 _position;
        public Vector3 _eulerAngles;

        public float _distance;
        public float _fov;

        public CameraDatas(Camera targetCamera)
        {
            _position = targetCamera.transform.parent.position;
            _eulerAngles = targetCamera.transform.parent.eulerAngles;
            _distance = targetCamera.transform.localPosition.z;
            _fov = targetCamera.fieldOfView;
        }
    }
}


