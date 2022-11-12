using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCameraManager : MonoBehaviour
{
    [SerializeField] GameObject[] cameraObjects;
    // Start is called before the first frame update

    int currentActivedCameraIndex = 0;

    public void CycleCameras()
    {
        cameraObjects[currentActivedCameraIndex].SetActive(false);

        currentActivedCameraIndex = (currentActivedCameraIndex + 1) % cameraObjects.Length;

        cameraObjects[currentActivedCameraIndex].SetActive(true);

    }

    public void SetActiveCamera(int index)
    {
        currentActivedCameraIndex = index;
        cameraObjects[currentActivedCameraIndex].SetActive(true);
        for (int i = 0; i < cameraObjects.Length; i++)
        {
            if (i != currentActivedCameraIndex)
            {
                cameraObjects[i].SetActive(false);
            }
        }
    }

    public Camera GetCurrentCamera
    {
        get
        {
            return cameraObjects[currentActivedCameraIndex].GetComponentInChildren<Camera>();
        }
    }


}
