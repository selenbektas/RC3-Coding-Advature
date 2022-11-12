using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public enum SolverDimension
    {
        TWO,
        THREE
    }

    [SerializeField] Solver2D solver;
    [SerializeField] Solver3D solver3D;
    public SolverDimension dimension;
    public Toggle toggle;
    public Image mark;
    public Sprite spriteTrue;
    public Sprite spriteFalse;




    public void Start()
    {
        if (dimension == SolverDimension.TWO)
        {
            solver.OnSolvedEvent.AddListener(NotifySolved);
        }
        else
        {
            solver3D.OnSolvedEvent.AddListener(NotifySolved);
        }
       
        toggle.onValueChanged.AddListener(ToggleSolverPlay);
    }

    public void ToggleSolverPlay(bool toggle)
    {
        if (dimension == SolverDimension.TWO)
        {
            solver.play = toggle;

            if (toggle == true && solver.IsSolved)
            {
                solver.Restore();
            }
        }
        else
        {
            solver3D.play = toggle;

            if (toggle == true && solver3D.IsSolved)
            {
                solver3D.Restore();

            }
        }
            

        mark.sprite = toggle ? spriteTrue : spriteFalse;
    }

    

    public void NotifySolved()
    {
        toggle.isOn = false;
    }
    
}
