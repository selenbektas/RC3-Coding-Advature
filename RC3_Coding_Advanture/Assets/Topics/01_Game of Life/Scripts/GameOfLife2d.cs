using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife2d : MonoBehaviour
{
    public Texture2D seedImage;
    public Cell cellPrefab;
    public int width = 10;
    public int height = 10;
    public int[] rule = new int[] { 2, 3, 3, 3 };

    public int updateLatency = 50;
    public int maxIterations = 10000;

    int currentIter = 0;

    int[,] states;
    int[,] nextStates;

    Cell[,] cells;

    void Start()
    {
        PopulateCells();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % updateLatency == 0)
        {
            if (currentIter < maxIterations)
            {
                CalculateNextStates();
                UpdateCells();

                currentIter++;
            }
        }
       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStates();
            currentIter = 0;
        }
    
    }

    void ResetStates()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float u = x / (float)width;
                float v = y / (float)height;

                float g = seedImage.GetPixelBilinear(u, v).grayscale;

                int state = g > 0.5f ? 1 : 0;

                states[x, y] = state;


                var cell = cells[x, y];
                cell.SetState(state);
                cell.SetID(x, y);
            }
        }
    }

    void PopulateCells()
    {
        states = new int[width, height];
        nextStates = new int[width, height];
        cells = new Cell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float u = x / (float)width;
                float v = y / (float)height;

                float g = seedImage.GetPixelBilinear(u, v).grayscale;

                int state = g > 0.5f ? 1 : 0;

                states[x, y] = state;

                var cell = Instantiate(cellPrefab, transform);
                cell.transform.localPosition = new Vector3(x, 0, y);
                cell.SetState(state);
                cell.SetID(x, y);
                cells[x, y] = cell;

            }
        }
    }

    public void CalculateNextStates()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int state = states[x,y];

                // neighbor total states 
                int statesNB = GetNeighborStatesSum(x, y);

                int nextState = state;

                if (state == 1)
                {
                    if (statesNB < rule[0])
                    {
                        nextState = 0;
                    }

                    if (statesNB>=rule[0] && statesNB <= rule[1])
                    {
                        nextState = 1;
                    }

                    if (statesNB > rule[2])
                    {
                        nextState = 0;
                    }
                }
                else
                {
                    if (statesNB == rule[3])
                    {
                        nextState = 1;
                    }
                }

                nextStates[x,y] = nextState;
            }
        }

        
    }

    void UpdateCells()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = cells[x, y];

                int newState = nextStates[x, y];

                cell.SetState(newState);

                states[x, y] = newState;
            }
        }
    }


    // Start is called before the first frame update
   


    int GetNeighborStatesSum(int x, int y)
    {
        int sum = 0;

        for (int iy = -1; iy < 2; iy++)
        {
            for (int ix = -1; ix < 2; ix++)
            {
                if ( ix == 0 && iy == 0)
                {
                    continue;
                }

                int nx = x + ix;
                int ny = y + iy;

                if (nx >= 0 && nx < width && ny > 0 && ny < height)
                {
                    int stateN = states[nx, ny];
                    sum += stateN;
                }
            }
        }

        return sum;
    }
}
