using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    // Start is called before the first frame update

    public bool bothAxis = false;
    Camera _currentCam;

    MultiCameraManager _manager;

    MultiCameraManager CamManager
    {
        get
        {
            if (_manager == null)
            {
                _manager = GameObject.FindObjectOfType<MultiCameraManager>();
            }

            return _manager;
        }
    }


    Vector3 FacingDirection
    {
        get
        {
            var camPos = CamManager.GetCurrentCamera.transform.position;
            var objPos = transform.position;

            if (!bothAxis)
            {
                camPos.y = 0;
                objPos.y = 0;
            }
          

            return  objPos-camPos;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(FacingDirection);

    }
    private void OnEnable()
    {
        var ps = transform.parent.localScale;
        var ts = transform.localScale;
        transform.localScale = new Vector3(ps.x*ts.x,ps.y*ts.y,ps.z*ts.z);
    }

}
