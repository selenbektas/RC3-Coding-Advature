using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileHolder3D : MonoBehaviour
{
    // constant identifier for the condition that is totally free to all kinds of labels.
    const string NoConstraint = "Nah";

    //a vector2 location identifier with x and y, indicates it's coordinate location inside a grid.
    public Vector3Int id;

    public float tileSize = 1;

    public ProfileSet profiles;


    // string labeled constrints, similar concept with the labels on the tile object, but these are used mainly for filtering out the incompatible tiles.
    public string constraint_left = NoConstraint;
    public string constraint_right = NoConstraint;
    public string constraint_back = NoConstraint;
    public string constraint_forward = NoConstraint;
    public string constraint_up = NoConstraint;
    public string constraint_down = NoConstraint;

    //the tile object to be placed, is null by default, till the holder object is assigned with a particular tile.
    public Tile3D m_tile = null;


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
        constraint_back = NoConstraint;
        constraint_forward = NoConstraint;
        constraint_up = NoConstraint;
        constraint_down = NoConstraint;


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
    public Vector3Int Left
    {
        get
        {
            return new Vector3Int(id.x - 1, id.y,id.z);
        }
    }

    // gets the ID to the right of current ID
    public Vector3Int Right
    {
        get
        {
            return new Vector3Int(id.x + 1, id.y,id.z);
        }
    }

    // gets the ID in front of of current ID
    public Vector3Int Forward
    {
        get
        {
            return new Vector3Int(id.x, id.y ,id.z+1);
        }
    }

    // gets the ID in the back of current ID
    public Vector3Int Behind
    {
        get
        {
            return new Vector3Int(id.x, id.y,id.z-1);
        }
    }
    // gets the ID in the back of current ID
    public Vector3Int Up
    {
        get
        {
            return new Vector3Int(id.x, id.y + 1,id.z);
        }
    }

    // gets the ID in the back of current ID
    public Vector3Int Down
    {
        get
        {
            return new Vector3Int(id.x, id.y - 1,id.z);
        }
    }


    //gets the 4 neighboring IDs in group
    public IEnumerable<Vector3Int> GetNeightbors()
    {
        yield return Left;
        yield return Right;
        yield return Behind;
        yield return Forward;
        yield return Up;
        yield return Down;
    }


    /// <summary>
    /// check  label on each face of a tile with constraint on each face of a holder
    /// </summary>
    /// <param name="tile"> the tile object to be tested against </param>
    /// <returns>true if given tile is compatible with this holder object, otherwise false</returns>
    public bool IsConsistantWith(Tile3D tile)
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

        if (constraint_back != NoConstraint)
        {
            if (constraint_back != tile.label_back)
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

        if (constraint_up != NoConstraint)
        {
            if (constraint_up != tile.label_up)
            {
                return false;
            }
        }

        if (constraint_down != NoConstraint)
        {
            if (constraint_down != tile.label_down)
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
    public void SetTile(Tile3D tile)
    {
        //make the "m_tile" to be the clone of source tile object.
        m_tile = Instantiate(tile, transform);

        //reset the transform position,rotation and scale.
        m_tile.transform.localPosition = Vector3.zero;
        m_tile.transform.localRotation = Quaternion.identity;
        m_tile.transform.localScale = Vector3.one;

        //inherit the constraints from labels of the tile.
        constraint_left = tile.label_left;
        constraint_right = tile.label_right;
        constraint_back = tile.label_back;
        constraint_forward = tile.label_forward;
        constraint_up = tile.label_up;
        constraint_down = tile.label_down;
    }

    public string GetCombinedConstraints
    {
        get
        {
            return $"left:{constraint_left},right:{constraint_right},behind:{constraint_back},forward:{constraint_forward}";
        }
    }

    private void OnDrawGizmosSelected()
    {
        var oldMtx = Gizmos.matrix;
        var oldCol = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * tileSize);


#if UNITY_EDITOR

        var handleMtx = Handles.matrix;
        var handleCol = Handles.color;
        Handles.BeginGUI();

        Handles.matrix = transform.localToWorldMatrix;

        float off = 0.6f;
        float positive = 1.5f;
        float negative = 1.1f;

        Color left = Color.magenta * positive;
        Color right = Color.red * negative;
        Color forward = Color.cyan * positive;
        Color back = Color.blue * negative;
        Color up = Color.yellow * positive;
        Color down = Color.green * negative;



        GUI.color = left;
        Handles.Label(Vector3.right * tileSize * off, "[right]: " + constraint_right);
        GUI.color = right;
        Handles.Label(Vector3.left * tileSize * off, "[left]: " + constraint_left);

        var pr = profiles.Find(constraint_right);
        var pl = profiles.Find(constraint_left);

        if (pr != null)
        {
            Gizmos.color = left;
            pr.DrawShape(Vector3.right * tileSize * 0.5f, Vector3.right, Vector3.up, tileSize);
        }
        if (pl != null)
        {
            Gizmos.color = right;
            pl.DrawShape(Vector3.left * tileSize * 0.5f, Vector3.right, Vector3.up, tileSize);
        }

        GUI.color = forward;
        Handles.Label(Vector3.forward * tileSize * off, "[forward]: " + constraint_forward);
        GUI.color = back;
        Handles.Label(Vector3.back * tileSize * off, "[back]: " + constraint_back);

        var pf = profiles.Find(constraint_forward);
        var pb = profiles.Find(constraint_back);

        if (pf != null)
        {
            Gizmos.color = forward;
            pf.DrawShape(Vector3.forward * tileSize * 0.5f, Vector3.forward, Vector3.up, tileSize);
        }
        if (pb != null)
        {
            Gizmos.color = back;
            pb.DrawShape(Vector3.back * tileSize * 0.5f, Vector3.forward, Vector3.up, tileSize);
        }

        GUI.color = up;
        Handles.Label(Vector3.up * tileSize * off, "[up]: " + constraint_up);
        GUI.color = down;
        Handles.Label(Vector3.down * tileSize * off, "[down]: " + constraint_down);


        var pu = profiles.Find(constraint_up);
        var pd = profiles.Find(constraint_down);

        if (pu != null)
        {
            Gizmos.color = up;
            pu.DrawShape(Vector3.up * tileSize * 0.5f, Vector3.up, Vector3.forward, tileSize);
        }
        if (pd != null)
        {
            Gizmos.color = down;
            pd.DrawShape(Vector3.down * tileSize * 0.5f, Vector3.up, Vector3.forward, tileSize);
        }


        Handles.matrix = handleMtx;

        Handles.color = handleCol;

        Handles.EndGUI();
#endif


        Gizmos.matrix = oldMtx;
        Gizmos.color = oldCol;
    }

}
