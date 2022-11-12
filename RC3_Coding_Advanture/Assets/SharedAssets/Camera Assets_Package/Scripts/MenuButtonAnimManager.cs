using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonAnimManager : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SetHighlight(bool highlighted)
    {
        animator.SetBool("IsHighlighted", highlighted);
    }

    public void SetHighlightTrue()
    {
        animator.SetBool("IsHighlighted", true);
    }
    public void SetHighlightFalse()
    {
        animator.SetBool("IsHighlighted", false);
    }
}
