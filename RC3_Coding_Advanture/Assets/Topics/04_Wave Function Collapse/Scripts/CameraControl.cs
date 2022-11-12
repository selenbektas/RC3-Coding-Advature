using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{

    [SerializeField]
    private float _sensitivity = 1.0f;

    [SerializeField]
    private float _stiffness = 5.0f;

    public float _minRotationX = -90;
    public float _maxRotationX = 90;

    public bool orbit = false;


    [SerializeField]
    float _minDistance = 0;
    [SerializeField]
    float _maxDistance = 100f;

    [SerializeField]Transform _pivot;

    Camera m_camera;

  
    private Vector3 _position;
    Vector3 _rotation;

    float _distance;

    bool holding = false;
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        _position = _pivot.position;
        _distance = -transform.localPosition.z;
        _rotation = _pivot.rotation.eulerAngles;
        m_camera = GetComponent<Camera>();
    }

    Vector3 prevMouse;
    /// <summary>
    /// 
    /// </summary>
    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            holding = true;

            var pos = m_camera.WorldToScreenPoint(_pivot.position);

            var mouseDelta = Input.mousePosition - prevMouse;

            pos -= mouseDelta;
            _pivot.position = m_camera.ScreenToWorldPoint(pos);

            
        }
        else
        {
            holding = false;
        }


        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(delta)>0)
        {
            _distance -= delta * _sensitivity * _distance;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            var p = transform.localPosition;
            p.z = -_distance;

            transform.localPosition = p;
        }

        if (orbit)
        {
            if (Input.GetMouseButton(0))
            {
                _rotation.x -= Input.GetAxis("Mouse Y") * _sensitivity;
                _rotation.y += Input.GetAxis("Mouse X") * _sensitivity;
                _rotation.x = Mathf.Clamp(_rotation.x, _minRotationX, _maxRotationX);
            }


            var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, q, Time.deltaTime * _stiffness);
        }

      


        prevMouse = Input.mousePosition;

    }

   

   
}
