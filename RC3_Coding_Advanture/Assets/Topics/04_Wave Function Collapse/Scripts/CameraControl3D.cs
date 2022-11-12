using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl3D : MonoBehaviour
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

    [SerializeField] float _minHeight = 0;
    [SerializeField] float _maxHeight = 100;

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
        if (Input.GetMouseButton(2))
        {
            holding = true;
            var t = _sensitivity * transform.localPosition.z * 0.1f; // sensitivity is proportional to distance from pivot
            var dx = Input.GetAxis("Mouse X") * t;
            var dy = Input.GetAxis("Mouse Y") * t;
            _position += transform.TransformVector(dx, dy, 0.0f);
            _position.y=Mathf.Clamp(_position.y,_minHeight,_maxHeight);
        }
        else
        {
            holding = false;
        }


        _pivot.position = Vector3.Lerp(_pivot.position, _position, Time.deltaTime * _stiffness);


        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(delta)>0)
        {
            _distance -= delta * _sensitivity * _distance;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            var p = transform.localPosition;
            p.z = Mathf.Lerp(p.z, -_distance, Time.deltaTime * _stiffness);

            transform.localPosition = p;
        }

        if (orbit)
        {
            if (Input.GetMouseButton(1))
            {
                _rotation.x -= Input.GetAxis("Mouse Y") * _sensitivity*2;
                _rotation.y += Input.GetAxis("Mouse X") * _sensitivity*2;
                _rotation.x = Mathf.Clamp(_rotation.x, _minRotationX, _maxRotationX);
            }


            var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
            _pivot.rotation = Quaternion.Lerp(_pivot.rotation, q, Time.deltaTime * _stiffness);
        }

      


        prevMouse = Input.mousePosition;

    }

   

   
}
