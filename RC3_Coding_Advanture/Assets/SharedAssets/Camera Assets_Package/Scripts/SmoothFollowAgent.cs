using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowAgent : MonoBehaviour
{
    public Transform target;
    public Camera camera;

    [Header("Follow")]
    public float followStiffness = 1.0f;
    [Header("Orbit")]
    public bool follorRotation = false;
    public bool invertYAxis = false;
    public float rotateSpeed = 10f;
    public float rotateStiffness = 1.0f;
    public float minRotationX = -90f;
    public float maxRotationX = 90f;


    [Header("Height")]
    public float height = 1.5f;
    public float changeHeightSpeed = 1.0f;
    public float changeHeightStiffness = 1.0f;
    public float minHeight = 1.2f;
    public float maxHeight = 2.0f;



    Vector3 _position;
    Vector3 _rotation;
    float _distance;

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.position;
        _rotation = transform.rotation.eulerAngles;
        _distance = -camera.transform.localPosition.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetPosition();
        SetRotation();
        SetHeight();
    }

    void SetPosition()
    {
        _position = Vector3.Lerp(_position, target.position + Vector3.up * height, Time.deltaTime * followStiffness);
        transform.position = _position;
    }

    void SetRotation()
    {
        if (!follorRotation)
        {
            if (Input.GetMouseButton(1))
            {
                float rotY = Input.GetAxis("Mouse Y") * rotateSpeed;
                _rotation.x -= invertYAxis ? -rotY : rotY;
                _rotation.y += Input.GetAxis("Mouse X") * rotateSpeed;

                _rotation.x = Mathf.Clamp(_rotation.x, minRotationX, maxRotationX);
            }
        }
        else
        {
            _rotation.y = Mathf.Lerp(_rotation.y, target.rotation.eulerAngles.y, Time.deltaTime * rotateStiffness);
        }

        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, _rotation, Time.deltaTime* rotateStiffness);

        var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * rotateStiffness);
    }

    void SetHeight()
    {
        if (Input.GetMouseButton(2))
        {
            float targetHeight = height;
            targetHeight -= Input.GetAxis("Mouse Y") * changeHeightSpeed;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
            height = targetHeight;
        }
    }

    void ProtectFromWallClip()
    {

    }
}
