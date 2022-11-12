using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI2ObjectLocator : MonoBehaviour
{
    [SerializeField] RectTransform uiXform;
    [SerializeField] Transform targetXform;
    [SerializeField] RectTransform indicator;
    [SerializeField] float pedding = 0f;
    [SerializeField] bool clampRectPos;

    MultiCameraManager _camManager;

    MultiCameraManager CamManager
    {
        get
        {
            if (_camManager == null)
            {
                _camManager = GameObject.FindObjectOfType<MultiCameraManager>();
            }
            return _camManager;
        }
    }




    private void LateUpdate()
    {
        if (uiXform.gameObject.activeSelf)
        {
            var cam = CamManager.GetCurrentCamera;

            var screenPoint = cam.WorldToScreenPoint(targetXform.position);

            var rect = uiXform.rect;

            if (screenPoint.z < 0)
            {
                screenPoint.x = (Screen.width - screenPoint.x);
                screenPoint.y = 0;
            }

            Vector3 outBound=Vector3.zero;

            uiXform.position = ScreenClampPoint(rect, screenPoint, out outBound, pedding);


            var dir = new Vector3(outBound.x, outBound.y);

            dir.x = Mathf.Clamp(dir.x, rect.xMin, rect.xMax);
            dir.y = Mathf.Clamp(dir.y, rect.yMin, rect.yMax);

            if (indicator == null)
            {
                return;
            }

            if (dir.x == rect.xMin || dir.x == rect.xMax||dir.y==rect.yMin||dir.y==rect.yMax)
            {
                indicator.localPosition = dir;
                return;
            }
            else
            {
                float vx = dir.x - rect.xMin;
                float vy = dir.y - rect.yMin;

                float px = vx / rect.width;
                float py = vy / rect.height;

                if (px > py)
                {
                    dir.y = dir.y > rect.center.y ? rect.yMax : rect.yMin;
                }

                if (px<py)
                {
                    dir.x = dir.x > rect.center.x ? rect.xMax : rect.xMin;
                }

                indicator.localPosition = dir;
            }

        }
    }

    Vector3 ScreenClampPoint(Rect rect, Vector3 inputPoint,out Vector3 outBoundAmount,float pedding=0f)
    {
        var newpoint = inputPoint;


        newpoint.x = Mathf.Clamp(newpoint.x, 0-rect.xMin + pedding, Screen.width-rect.xMax - pedding);
        newpoint.y = Mathf.Clamp(newpoint.y, 0-rect.yMin+ pedding, Screen.height -rect.yMax - pedding);

        outBoundAmount = inputPoint - newpoint;


        return newpoint;
    }

    Vector3 ScreenCenter
    {
        get
        {
            return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        }
    }

   
}
