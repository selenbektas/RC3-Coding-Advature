using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePanel : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] RectTransform button;
    [SerializeField] int intervals = 50;



    bool isOpened;

    public void MovePosition()
    {

        //if (defualtPosition == Vector3.zero)
        //{
        //    defualtPosition = rect.localPosition;
        //}

        if (isOpened)
        {
            StartCoroutine(OpenPanel(Vector3.zero));
            isOpened = false;

        }
        else
        {
            StartCoroutine(OpenPanel(new Vector3(-rect.rect.width, 0, 0)));
            isOpened = true;

        }

    }

    IEnumerator OpenPanel(Vector3 position)
    {
        var scale = isOpened ? new Vector3(1, 1,1) : new Vector3(1, -1, 1);
        for (int i = 0; i <= intervals; i++)
        {
            yield return new WaitForFixedUpdate();

            float t = i / (float)intervals;
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, position, t);
            button.localScale = Vector3.Lerp(button.localScale, scale, t);
        }
        //if (!isOpened)
        //{
        //    button.localEulerAngles = new Vector3(0, 0, 90);
        //}
        //else
        //{
        //    button.localEulerAngles = new Vector3(0, 0, -90);
        //}

    }



}
