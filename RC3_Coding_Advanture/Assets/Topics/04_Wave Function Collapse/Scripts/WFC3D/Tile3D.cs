using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class Tile3D : MonoBehaviour
{
    [SerializeField] ProfileSet profiles;
    public float tileSize = 1.0f;

    [Space(20)]
    public string label_left;
    public string label_right;
    [Space(10)]
    public string label_back;
    public string label_forward;
    [Space(10)]
    public string label_down;
    public string label_up;

    public float param;
    
    // Start is called before the first frame update

    private void OnDrawGizmosSelected()
    {
        var oldMtx = Gizmos.matrix;
        var oldCol = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one*tileSize);

       
#if UNITY_EDITOR

        var handleMtx = Handles.matrix;
        var handleCol = Handles.color;
        Handles.BeginGUI();
       
        Handles.matrix = transform.localToWorldMatrix;

        float off = 0.6f;
        float positive = 1.5f;
        float negative = 1.1f;

        Color left=Color.magenta* positive;
        Color right = Color.red * negative;
        Color forward=Color.cyan* positive;
        Color back = Color.blue * negative;
        Color up = Color.yellow * positive;
        Color down=Color.green* negative;



        GUI.color = left;
        Handles.Label(Vector3.right * tileSize * off, "[right]: " + label_right);
        GUI.color = right;
        Handles.Label(Vector3.left * tileSize * off, "[left]: " + label_left);

      

        GUI.color = forward;
        Handles.Label(Vector3.forward*tileSize*off, "[forward]: "+label_forward);
        GUI.color = back;
        Handles.Label(Vector3.back*tileSize*off, "[back]: "+label_back);

      

        GUI.color = up;
        Handles.Label(Vector3.up*tileSize*off, "[up]: "+label_up);
        GUI.color = down;
        Handles.Label(Vector3.down*tileSize*off, "[down]: "+label_down);

        if (profiles != null)
        {
            var pr = profiles.Find(label_right);
            var pl = profiles.Find(label_left);

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

            var pf = profiles.Find(label_forward);
            var pb = profiles.Find(label_back);

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


            var pu = profiles.Find(label_up);
            var pd = profiles.Find(label_down);

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
        }
      

     


        Handles.matrix = handleMtx;

        Handles.color = handleCol;

        Handles.EndGUI();
#endif


        Gizmos.matrix = oldMtx;
        Gizmos.color = oldCol;
    }

}


