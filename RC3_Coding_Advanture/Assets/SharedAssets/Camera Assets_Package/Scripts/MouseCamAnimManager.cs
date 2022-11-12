using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamAnimManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string x_axis_key;
    [SerializeField] string y_axis_key;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //float x = Input.GetAxis("Mouse X");
        //float y = Input.GetAxis("Mouse Y");


        //print($"Mouse Pose({Input.mousePosition})");

    }

    private void LateUpdate()
    {
        var mp = GetMousePositionNorm();

        print($"Mouse Pose({mp})");

        animator.SetFloat(x_axis_key, Mathf.Clamp(mp.x, 0, 1));
        animator.SetFloat(y_axis_key, Mathf.Clamp(mp.y, 0, 1));
    }

    Vector3 GetMousePositionNorm()
    {
        var mp = Input.mousePosition;

        float x = mp.x / Screen.width;
        float y = (Screen.height - mp.y) / Screen.height;

        x = Mathf.Clamp(x, 0.0f, 1.0f);

        y = Mathf.Clamp(y, 0.0f, 1.0f);


        return new Vector3(x, y, 0);

    }

    Vector3 ScreenCenter => new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);

}
