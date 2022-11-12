using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RC3;

namespace CameraDataUtilities
{
    /// <summary>
    /// AttatchThisToCameraParent;
    /// </summary>
    public class CameraBehaviors : MonoBehaviour
    {
        [SerializeField] Camera _targetCamera;
        [SerializeField] SharedCameraDatas _cameraDatas;
        [SerializeField] Color takenColor;
        [SerializeField] RectTransform camTogglePanel;
        [SerializeField] Toggle camTogglePrefab;
        [SerializeField] TextMeshProUGUI fovText;
        [SerializeField] Slider fovSlider;
        [SerializeField] TextMeshProUGUI currentIndexText;

        [SerializeField] List<Toggle> toggles;
        

        bool fix;
        bool top;

        int currentDataIndex=-1;
        CameraDatas tempDatas;

        private void Start()
        {

            if (_cameraDatas == null)
            {
                return;
            }
            toggles = new List<Toggle>();
            for (int i = 0; i < _cameraDatas.savedCameras.Count; i++)
            {
                AddToggle(i);
            }

            currentIndexText.SetText("None");

            SetFOV(fovSlider.value);
        }

        public void SetFOV(float fov)
        {
            _targetCamera.fieldOfView = fov;
            fovText.SetText("FOV:" + Mathf.Round(fov*10f)/10f);
        }

     

        public void AddToggle(int index)
        {
            var toggle = Instantiate(camTogglePrefab, camTogglePanel);

            toggle.onValueChanged.AddListener(isOn => CheckCurrentState(isOn,index));
            
            toggle.isOn = false;
            toggles.Add(toggle);
        }
        public void ChangeToggleIndex(int index)
        {
            var toggle = toggles[index];

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(isOn => CheckCurrentState(isOn, index));
        }

        public void Remove()
        {
            if (currentDataIndex != -1)
            {
             
                var toDestroy = toggles[currentDataIndex].gameObject;
                toggles.RemoveAt(currentDataIndex);
                Destroy(toDestroy);
                _cameraDatas.Remove(currentDataIndex);

                currentDataIndex = -1;

                currentIndexText.SetText("None");

                for (int i = 0; i < _cameraDatas.savedCameras.Count; i++)
                {
                    ChangeToggleIndex(i);
                    print(i);
                }
            }
        }

        public void CheckCurrentState(bool shouldSet,int index)
        {
            if (shouldSet)
            {
                print("Should Set" + index);
                currentDataIndex = index;
                currentIndexText.SetText("" + currentDataIndex);
                ToggleOther();
            }
            else
            {
                if (currentDataIndex == index)
                {
                    currentDataIndex = -1;
                    currentIndexText.SetText("None");
                }
            }
        }
        public void SaveCamera()
        {
            var cd = new CameraDatas(_targetCamera);

            if (currentDataIndex != -1)
            {
                _cameraDatas.Override(currentDataIndex, cd);
            }
            else
            {
                int index = _cameraDatas.savedCameras.Count;
                _cameraDatas.Add(cd);
                AddToggle(index);
            }
        }

        public void SaveTemp()
        {
            tempDatas = new CameraDatas(_targetCamera);
        }
        public void LoadCamera(int index)
        {
            SaveTemp();
            if (_cameraDatas.savedCameras.Count > index)
            {
                LoadCamera(_cameraDatas.savedCameras[index]);
            }
  
        }
        public void LoadCurrentCamera()
        {

            if (currentDataIndex != -1)
            {
                LoadCamera(currentDataIndex);
            }

        }

        public void LoadCamera(CameraDatas camdatas)
        {
            _targetCamera.GetComponent<Pan>().SetPosition(camdatas._position);
            _targetCamera.GetComponent<Orbit>().SetRotation(camdatas._eulerAngles);
            _targetCamera.GetComponent<Zoom>().SetDistance(camdatas._distance);
            SetFOV( camdatas._fov);
            fovSlider.value = camdatas._fov;
        }

        public void ToggleOther()
        {
            for (int i = 0; i < _cameraDatas.savedCameras.Count; i++)
            {
                if (i != currentDataIndex)
                {
                    toggles[i].isOn = false;
                }
            }
        }
        public void FixPosition()
        {
            fix = !fix;
            _targetCamera.GetComponent<Pan>().enabled = !fix;
            _targetCamera.GetComponent<Orbit>().enabled = !fix;
            _targetCamera.GetComponent<Zoom>().enabled = !fix;
            fovSlider.interactable = !fix;
        }

        public void SetInteractable(bool enabled)
        {
            _targetCamera.GetComponent<Pan>().interactable = enabled;
            _targetCamera.GetComponent<Orbit>().interactable = enabled;
            _targetCamera.GetComponent<Zoom>().interactable = enabled;

        }


        public void ToggleTopView(bool toggle)
        {
            if (toggle)
                transform.localEulerAngles = new Vector3(90f, 0, 0);

            if (_targetCamera == null)
            {
                print("Camera Null");
            }
            if (!_targetCamera.GetComponent<Orbit>())
            {
                print("Orbit null");
            }

            _targetCamera.GetComponent<Orbit>().enabled = !toggle;
        }
    }
}

