using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RC3;

/*
 * Notes
 */

namespace RC3
{
    /// <summary>
    /// 
    /// </summary>
    public class Zoom : MonoBehaviour
    {
        [SerializeField]
        private float _sensitivity = 1.0f;

        [SerializeField]
        private float _stiffness = 5.0f;
        

        private float _distance = 50.0f;
        public float _minDistance = 1.0f;
        public float _maxDistance = 100.0f;
        public bool interactable = true;
        

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            _distance = -transform.localPosition.z;
        }


        /// <summary>
        /// 
        /// </summary>
        private void LateUpdate()
        {
            if (interactable)
            {
                _distance -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity * _distance;
            }
        
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);

            var p = transform.localPosition;
            p.z = Mathf.Lerp(p.z, -_distance, Time.deltaTime * _stiffness);

            transform.localPosition = p;
        }

        public void SetDistance(float dist)
        {
            _distance =-dist;
        }
    }
}
