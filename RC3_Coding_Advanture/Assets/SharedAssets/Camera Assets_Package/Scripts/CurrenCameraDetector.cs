using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CurrenCameraDetector : MonoBehaviour
{

    Camera _current;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   

    // Update is called once per frame
    void Update()
    {
        if (Camera.current != null)
        {
            _current = Camera.current;
        }
    }

    public Camera GetCurrentCamera
    {
        get => _current;
    }
}
