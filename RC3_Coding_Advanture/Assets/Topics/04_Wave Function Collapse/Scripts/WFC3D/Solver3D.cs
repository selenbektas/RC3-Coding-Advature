using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

public class Solver3D : MonoBehaviour
{
    //the pre-defined tile holder object as a template prefab.
    [SerializeField] TileHolder3D holderPrefab;
    //the tile set holds all tiles in a list.
    [SerializeField] TileSet3D tileSet;

    [SerializeField] VolumetricMask mask;


    //controls how fast a solving process goes, smaller number for faster, larger for slower
    public int updateRate = 100;
    //maximal conflicts allows for solving
    public int maxConflicts = 10;

    public int countX;
    public int countY;
    public int countZ;

    //the unit scale of the grid
    public float gridScale = 1f;
    //the keycode for pressing to reset
    public KeyCode resetKey;

    //whether to set the boundray nodes with certain label, true to set all boundray nodes with the label below, false to leave them unconstrained.
    public bool constrainBounds;
    //the label used to set to the boundray nodes
    public string boundsLabel;

    //toggle play/pause;
    public bool play = false;
    //use default evaluate or use advanced evaluate.
    public bool advancedEvaluation;


    //dictionary of all tile holder objects, indexed by vector2 IDs, instead of integer numbers.
    Dictionary<Vector3Int, TileHolder3D> allHolders;

    //dictionary of the number of options for each tile holder object on the vector2 ID.
    Dictionary<Vector3Int, int> checkedNodesOptionCount;

    //a hashset of vector2 ids that were not solved yet.
    HashSet<Vector3Int> unsolved;

    //is the whole thing solved or not.
    bool solved = false;

    //count how many steps has passed.
    int currentStep = 0;
    //count how many conflicts total.
    int conflicts = 0;

    float mapGrayValueOffset = 0;
    public UnityEvent OnSolvedEvent;


    void Start()
    {
        //setup the grid on start.
        Setup();
    }


    void Update()
    {
        //reset the whole thing and start over when given key is pressed.
        if (Input.GetKeyDown(resetKey))
        {
            if (!solved) Restore();
        }
        //stop updating by returning if the grid is all solved or play is set to false.
        if (solved || !play)
        {
            return;
        }

        //step the solving process once every certain number of frames, controlled by the varible of "updateRate".
        if (Time.frameCount % updateRate == 0)
        {
            Step();
        }

        //reset the grid and start over if there are too many conflicts.
        if (conflicts > maxConflicts)
        {
            Restore();
        }
    }

    /// <summary>
    /// reset the whole grid to initial state, clear the buffered data, and restore all holder objects to initial state.
    /// </summary>
    public void Restore()
    {
        //clear bufferred option numbers on each vector2 node
        checkedNodesOptionCount.Clear();
        unsolved.Clear();
        currentStep = 0;
        conflicts = 0;
        solved = false;
        //reset all holder objects to initial state
        foreach (var holder in allHolders.Values)
        {
            holder.Restore();

            if (constrainBounds && boundsLabel != string.Empty)
            {
                int x = holder.id.x;
                int y = holder.id.y;
                int z = holder.id.z;

                if (x == 0)
                {
                    holder.constraint_left = boundsLabel;
                }
                if (y == 0)
                {
                    holder.constraint_down = boundsLabel;
                }
                if (z == 0)
                {
                    //set holder's back edge constraint to the given label
                    //if the holder is located to the back/down end of the grid
                    holder.constraint_back = boundsLabel;
                }
                if (x == countX - 1)
                {
                    holder.constraint_right = boundsLabel;
                }

                if (y == countY - 1)
                {
                    holder.constraint_up = boundsLabel;
                }
                if (z == countZ - 1)
                {
                    //set holder's front edge constraint to the given label
                    //if the holder is located to the front/top end of the grid
                    holder.constraint_forward = boundsLabel;
                }
            }

            //put every holder's id to the unsolved list
            unsolved.Add(holder.id);
        }
    }

    /// <summary>
    /// initialize the grid
    /// </summary>
    public void Setup()
    {
        //initialize every dataset
        allHolders = new Dictionary<Vector3Int, TileHolder3D>();
        checkedNodesOptionCount = new Dictionary<Vector3Int, int>();
        unsolved = new HashSet<Vector3Int>();

        //configurate event handler, can use for adding additional functionalities, unused for now.
        OnSolvedEvent.AddListener(NotifySolved);

        //set the transform position, to relocate the whole grid to the center of screen
        //transform.position = new Vector3(countX * gridScale, countY * gridScale) * -0.5f;

        //2 dimensional for loop to set the grid in order.

        for (int z = 0; z < countZ; z++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    //get the vector2 id from x and y.
                    Vector3Int id = new Vector3Int(x, y, z);

                    var p = new Vector3(x * gridScale, y * gridScale, z * gridScale);

                    if (mask)
                    {
                        if (!mask.IsMaskedTrue(transform.TransformPoint(p)))
                        {
                            continue;
                        }
                    }

                    //make a clone of the holder prefab object, then put it under this solver object as a child.
                    var holder = Instantiate(holderPrefab, transform);
                    //pass the vector2 id to the holder object.
                    holder.id = id;
                    //set the local position of holder object to it's supposed position in the grid regarding it's id and grid scale.
                    holder.transform.localPosition = p;

                    holder.Setup();
                    holder.tileSize = gridScale;

                    //set the bounds conditions if needed.(constrainBounds toggle is set to true, and the given label for bounds edges is not empty)
                    if (constrainBounds && boundsLabel != string.Empty)
                    {
                        if (x == 0)
                        {
                            //set holder's left edge constraint to the given label
                            //if the holder is located to the left end of the grid
                            holder.constraint_left = boundsLabel;
                        }
                        if (y == 0)
                        {
                            //set holder's back edge constraint to the given label
                            //if the holder is located to the back/down end of the grid
                            holder.constraint_down = boundsLabel;
                        }

                        if (z == 0)
                        {
                            //set holder's back edge constraint to the given label
                            //if the holder is located to the back/down end of the grid
                            holder.constraint_back = boundsLabel;
                        }


                        if (x == countX - 1)
                        {
                            //set holder's right edge constraint to the given label
                            //if the holder is located to the right end of the grid
                            holder.constraint_right = boundsLabel;
                        }

                        if (y == countY - 1)
                        {
                            //set holder's front edge constraint to the given label
                            //if the holder is located to the front/top end of the grid
                            holder.constraint_up = boundsLabel;
                        }
                        if (z == countZ - 1)
                        {
                            //set holder's front edge constraint to the given label
                            //if the holder is located to the front/top end of the grid
                            holder.constraint_forward = boundsLabel;
                        }
                    }

                    //store the holder in a vector2 indexed dictionary.
                    allHolders.Add(id, holder);

                    unsolved.Add(id);
                }
            }
        }
    }



    /// <summary>
    ///  things to do when the grid is solved
    /// </summary>
    public void NotifySolved()
    {
        //can add your custom code here, i.e. print something.
    }

    /// <summary>
    /// the step function solving the grid every iteration.
    /// </summary>
    public void Step()
    {
        //the id currently inspecting
        Vector3Int currentID;

        //set current id to a random id if it's the very first step
        if (currentStep == 0 || checkedNodesOptionCount.Count == 0)
        {
            currentID = GetRandomID;
            //calculate the valid options for current id
            ProcessTileOptions(currentID);
        }
        else
        {
            //always select the id with the least number of options from the bufferred option data.
            //giving higher priority to the id has less options, ensures a better solving quality, increases the probability of solving the grid completely.
            currentID = checkedNodesOptionCount.OrderBy(k => k.Value).First().Key;

            //if the number of options = 0, it has a conflit, means by the holder's current combination of constraints on all sides, there is no any given tile can answer to, 
            //can avoid this by designing more tiles that can cover as more combinations as possible. 
            if (checkedNodesOptionCount[currentID] == 0)
            {
                //prints the specific conflict details for fixing it later.
                print("Found a conflit with label combination of :" + allHolders[currentID].GetCombinedConstraints);
                conflicts++;
                allHolders[currentID].NotifyConfilict();
                //in current setup, if got a conflict, then skip it, leave it empty. we also have ways of reversing previous steps that causes this conflict, 
                //to elimnate conflicts as much as possible. can implement that in later classes.
                checkedNodesOptionCount.Remove(currentID);
                unsolved.Remove(currentID);

                //start over with another id by returning.
                return;
            }
        }


        //get the holder object by the given id.
        TileHolder3D holder = allHolders[currentID];

        //declare a index number to hold the selected tile index, which indicates the order in the TileSet list.
        int tileIndex = -1;

        //which evaluation to use.
        if (advancedEvaluation)
        {
            //select the minimal value returned by the evaluate function, by ordering the list by the evaluate result, then get the first.
            tileIndex = holder.tempOptions.OrderBy(t => AdvancedEvaluate(tileSet[t], currentID)).First();

            //selecting minimal value is only a rule we get used to, also because the "OrderBy" starts with minimal value.
            //it also works if selecting the max, the evaluate process simply converts the complex criterias into a one dimentional min-max value. 
        }
        else
        {
            //select the minimal value returned by the evaluate function.
            tileIndex = holder.tempOptions.OrderBy(t => Evaluate(tileSet[t])).First();
        }


        Tile3D selectedTile = tileSet[tileIndex];
        //set the selected tile to the holder, this node of the grid is solved.
        holder.SetTile(selectedTile);

        //remove it from the unsolved list and options list after solveing.
        checkedNodesOptionCount.Remove(currentID);
        unsolved.Remove(currentID);

        //deal with the adjacent neighbors on four sides.
        Vector3Int left = holder.Left;
        Vector3Int right = holder.Right;
        Vector3Int back = holder.Behind;
        Vector3Int forward = holder.Forward;
        Vector3Int down = holder.Down;
        Vector3Int up = holder.Up;

        //check whether the neighbor node is contained in the unsolved list, 
        //if it's not, means either it's already solved and we don't need to worry about it,
        //or it's outside of the grid, where we didn't place any holder.
        if (unsolved.Contains(left))
        {
            //get the holder object at the "left" id location, and pass the constraint on "left" edge of current holder 
            //to the "right" edge of the "left" neighbor.
            //each node and it's neighbor share one edge, in this case with the "left" neighbor, it shares it's "right" edge with 
            //current node's "left" edge, it's the same priciple for other neighbors.
            allHolders[left].constraint_right = holder.constraint_left;

            //process valid options after updating the constraints.
            int optionCount = ProcessTileOptions(left);


            //update the options count to the buffer list
            if (checkedNodesOptionCount.ContainsKey(left))
            {
                //update the number only if the id of "left" already inside the list
                checkedNodesOptionCount[left] = optionCount;
            }
            else
            {
                //otherwise add it as new to the list.
                checkedNodesOptionCount.Add(left, optionCount);
            }

            //NOTE: the "checkedNodesOptionCount" is a composite dictionary of vector2 ids, and number of possible options (int), where we can quary which one has the 
            //least or most number of options, then access the "key" as the vector2 id for looking up the holder object. 

            //The dictionary is a "KeyValuePair" structure, see: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-6.0

            //The "checkedNodesOptionCount" dictionary is accumulated as the solving process goes, every node gets solved will pass the constraints to it's neighbors,
            //then neighbors will process valid options under current constraints, and gets bufferred into the "checkNodes..." dictionary, waiting to be selected and
            //solved in the next step.

            //Typically this part would be looking at the whole grid then find the one with least options then solve it first, but it's in-effecient and makes no sense
            //of calculating the nodes when it has zero constraints. So with our method, we only consider the nodes that already has some constraints, then increase the 
            //range of nodes gradually, to guarantee the performance of the program.
        }
        // for other neighbors they are the same with the first one.
        if (unsolved.Contains(right))
        {
            allHolders[right].constraint_left = holder.constraint_right;
            int optionCount = ProcessTileOptions(right);
            if (checkedNodesOptionCount.ContainsKey(right))
            {
                checkedNodesOptionCount[right] = optionCount;
            }
            else
            {
                checkedNodesOptionCount.Add(right, optionCount);
            }
        }

        if (unsolved.Contains(back))
        {
            allHolders[back].constraint_forward = holder.constraint_back;
            int optionCount = ProcessTileOptions(back);
            if (checkedNodesOptionCount.ContainsKey(back))
            {
                checkedNodesOptionCount[back] = optionCount;
            }
            else
            {
                checkedNodesOptionCount.Add(back, optionCount);
            }

        }

        if (unsolved.Contains(forward))
        {
            allHolders[forward].constraint_back = holder.constraint_forward;
            int optionCount = ProcessTileOptions(forward);
            if (checkedNodesOptionCount.ContainsKey(forward))
            {
                checkedNodesOptionCount[forward] = optionCount;
            }
            else
            {
                checkedNodesOptionCount.Add(forward, optionCount);
            }
        }

        if (unsolved.Contains(up))
        {
            allHolders[up].constraint_down = holder.constraint_up;
            int optionCount = ProcessTileOptions(up);
            if (checkedNodesOptionCount.ContainsKey(up))
            {
                checkedNodesOptionCount[up] = optionCount;
            }
            else
            {
                checkedNodesOptionCount.Add(up, optionCount);
            }
        }

        if (unsolved.Contains(down))
        {
            allHolders[down].constraint_up = holder.constraint_down;
            int optionCount = ProcessTileOptions(down);
            if (checkedNodesOptionCount.ContainsKey(down))
            {
                checkedNodesOptionCount[down] = optionCount;
            }
            else
            {
                checkedNodesOptionCount.Add(down, optionCount);
            }
        }



        currentStep++;

        //check whether the grid is solved completely.
        if (unsolved.Count == 0)
        {
            solved = true;
            OnSolvedEvent.Invoke();
        }
    }


    /// <summary>
    /// this function goes through all tiles in the set to get the valid options, and returns the total number of options, which can be used to
    /// order the priority of which tile holder to be solved earlier.
    /// </summary>
    /// <param name="id">the id location</param>
    /// <returns>returns the total number of valid options</returns>
    int ProcessTileOptions(Vector3Int id)
    {
        var holder = allHolders[id];

        //clear the bufferred options before starting a new process
        holder.tempOptions.Clear();

        int optionCount = 0;

        for (int i = 0; i < tileSet.Count; i++)
        {
            // check whether a tile is consistant with this holder's constraints.
            if (holder.IsConsistantWith(tileSet[i]))
            {
                //buffer the tile index into holder's temp options buffer.
                holder.tempOptions.Add(i);
                optionCount++;
            }
        }

        return optionCount;
    }


    /// <summary>
    /// normal evaluate function that can be customized for selecting tiles under specific requirements. 
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    float Evaluate(Tile3D tile)
    {
        // here you can implement your custom function code to return particular values


        // here the random is only an example
        return UnityEngine.Random.Range(0.0f, 1.0f);
    }

    /// <summary>
    /// advanced evaluate function that takes in the vector2 id as a coordinate and measure certain value regarding this id.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="id"></param>
    /// <returns></returns>

    float AdvancedEvaluate(Tile3D tile, Vector3Int id)
    {
        // this evaluate function here is measuring the overall grayscale of the tile image, compare it's difference with the map image, 
        // in order to select the tile that matches the map most on certain coordinate.

        // this is an example as well, you can input something else.


        return UnityEngine.Random.Range(0.0f, 1.0f);
    }

    /// <summary>
    /// Get a random vector2int from the grid
    /// </summary>
    Vector3Int GetRandomID
    {
        get
        {
            int count = unsolved.Count;

            return unsolved.ElementAt(UnityEngine.Random.Range(0, count));
        }
    }

    public void SetMapValueOffset(float value)
    {
        mapGrayValueOffset = value;
    }

    private void OnDrawGizmos()
    {
        var oldMtx = Gizmos.matrix;
        var oldColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;

        var min = Vector3.zero;
        var max = new Vector3(countX-1, countY-1, countZ-1) * gridScale;

        Gizmos.DrawWireCube(0.5f*max,max);

        Gizmos.matrix = oldMtx;
        Gizmos.color = oldColor;

    }


    public bool IsSolved
    {
        get
        {
            return solved;
        }
    }
}
