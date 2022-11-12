using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// empty placeholder object that is distributed in the beginning, waiting to be solved by giving it a specific tile object.
/// </summary>
public class TileHolder2D : MonoBehaviour
{

    // constant identifier for the condition that is totally free to all kinds of labels.
    const string NoConstraint = "Nah";

    //a vector2 location identifier with x and y, indicates it's coordinate location inside a grid.
    public Vector2Int id;


    // string labeled constrints, similar concept with the labels on the tile object, but these are used mainly for filtering out the incompatible tiles.
    public string constraint_left = NoConstraint;
    public string constraint_right = NoConstraint;
    public string constraint_behind = NoConstraint;
    public string constraint_forward = NoConstraint;

    //the tile object to be placed, is null by default, till the holder object is assigned with a particular tile.
    public Tile2D m_tile = null;


    /// <summary>
    /// a temporary list holding all possible tile options of the holder
    /// </summary>
    public List<int> tempOptions;


    /// <summary>
    /// a event handler that can invoke certain customized functions if inserted in the inspector, it does nothing by default.
    /// </summary>
    public UnityEvent OnNotifyConfilict;

    
    /// <summary>
    /// notify the event to invoke.
    /// </summary>
    public void NotifyConfilict()
    {
        OnNotifyConfilict.Invoke();
    }

    /// <summary>
    /// reset everything to initial unsolved state. 
    /// </summary>
    public void Restore()
    {
        // reset all labels to unconstrained 
        constraint_left = NoConstraint;
        constraint_right = NoConstraint;
        constraint_behind = NoConstraint;
        constraint_forward = NoConstraint;


        // destroy the assgined tile object, if there is any.
        if (m_tile != null)
        {
            DestroyImmediate(m_tile.gameObject);
            m_tile = null;
        }

        //clear the temp options that was buffered in previous steps.
        tempOptions.Clear();
    }

    //Initialize the holder
    public void Setup()
    {
        tempOptions = new List<int>();
    }


    // gets the ID to the left of current ID
    public Vector2Int Left
    {
        get
        {
            return new Vector2Int(id.x - 1, id.y);
        }
    }

    // gets the ID to the right of current ID
    public Vector2Int Right
    {
        get
        {
            return new Vector2Int(id.x + 1, id.y);
        }
    }

    // gets the ID in front of of current ID
    public Vector2Int Forward
    {
        get
        {
            return new Vector2Int(id.x, id.y + 1);
        }
    }

    // gets the ID in the back of current ID
    public Vector2Int Behind
    {
        get
        {
            return new Vector2Int(id.x, id.y - 1);
        }
    }


    //gets the 4 neighboring IDs in group
    public IEnumerable<Vector2Int> GetNeightbors()
    {
        yield return Left;
        yield return Right;
        yield return Behind;
        yield return Forward;
    }


    /// <summary>
    /// check  label on each face of a tile with constraint on each face of a holder
    /// </summary>
    /// <param name="tile"> the tile object to be tested against </param>
    /// <returns>true if given tile is compatible with this holder object, otherwise false</returns>
    public bool IsConsistantWith(Tile2D tile)
    {
        if (constraint_left != NoConstraint)
        {
            if (constraint_left != tile.label_left)
            {
                return false;
            }
        }

        if (constraint_right != NoConstraint)
        {
            if (constraint_right != tile.label_right)
            {
                return false;
            }
        }

        if (constraint_behind != NoConstraint)
        {
            if (constraint_behind != tile.label_behind)
            {
                return false;
            }
        }

        if (constraint_forward != NoConstraint)
        {
            if (constraint_forward != tile.label_forward)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// assign a tile object to this holder object, which is essentially making a clone/instantiate/duplicate of the given tile object, and put the cloned instance under this holder object.
    /// </summary>
    /// <param name="tile">the source tile object</param>
    public void SetTile(Tile2D tile)
    {
        //make the "m_tile" to be the clone of source tile object.
        m_tile = Instantiate(tile,transform);

        //reset the transform position,rotation and scale.
        m_tile.transform.localPosition = Vector3.zero;
        m_tile.transform.localRotation = Quaternion.identity;
        m_tile.transform.localScale = Vector3.one;

        //inherit the constraints from labels of the tile.
        constraint_left = tile.label_left;
        constraint_right = tile.label_right;
        constraint_behind = tile.label_behind;
        constraint_forward = tile.label_forward;
    }


    public string GetCombinedConstraints
    {
        get
        {
            return $"left:{constraint_left},right:{constraint_right},behind:{constraint_behind},forward:{constraint_forward}";
        }
    }


    private void OnDrawGizmosSelected()
    {
        var oldMtx = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldMtx;
    }




}
