using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBoolAnimManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string paramKey;

    public void SetHighlight(bool highlighted)
    {
        animator.SetBool(paramKey, highlighted);
    }

    public void SetParamTrue()
    {
        animator.SetBool(paramKey, true);
    }
    public void SetParamFalse()
    {
        animator.SetBool(paramKey, false);
    }

    public void SwapParam()
    {
        animator.SetBool(paramKey, !animator.GetBool(paramKey));
    }
}
