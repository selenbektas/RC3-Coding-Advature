using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife3d : MonoBehaviour
{
    public Texture2D seedImage;
    public Cell cellPrefab;
    public int width = 10;
    public int height = 10;
    public int layerCount = 10;
    [Range(0,1)]
    public float seedThreshold = 0.5f;
    public int[] rule = new int[] { 2, 3, 3, 3 };

    public int updateLatency = 50;


    public Gradient gradient;


    int[,,] states;
    int[,,] nextStates;

    Cell[,,] cells;

    int currentLayer = 0;

    void Start()
    {
        PopulateCells();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % updateLatency == 0)
        {
            if (currentLayer < layerCount)
            {
                CalculateNextStates();
                UpdateCells();
                currentLayer++;
            }
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStates();
            currentLayer = 1;
        }

    }

    void ResetStates()
    {
        for (int layer = 0; layer < layerCount; layer++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int state = 0;
                    if (layer == 0)
                    {
                        float u = x / (float)width;
                        float v = y / (float)height;

                        float g = seedImage.GetPixelBilinear(u, v).grayscale;

                         state = g > 0.5f ? 1 : 0;
                    }

                    states[x, y,layer] = state;
                    var cell = cells[x, y,layer];
                    cell.SetState(state);
                    cell.SetAge(0);
                }
            }
        }
     
    }

    void PopulateCells()
    {
        states = new int[width, height,layerCount];
        nextStates = new int[width, height,layerCount];
        cells = new Cell[width, height,layerCount];

        currentLayer = 1;

        for (int layer = 0; layer < layerCount; layer++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int state = 0;
                    if (layer == 0)
                    {
                        float u = x / (float)width;
                        float v = y / (float)height;

                        float g = seedImage.GetPixelBilinear(u, v).grayscale;

                         state = g > seedThreshold ? 1 : 0;
                    }

                    states[x, y,layer] = state;

                    var cell = Instantiate(cellPrefab, transform);
                    cell.transform.localPosition = new Vector3(x, layer, y);
                    cell.SetState(state);
                    cell.SetID(x, y,layer);
                    cell.Init();
                    cell.SetColor(GetColorByAge(0));
                    cells[x, y,layer] = cell;
                }
            }
        }
    }

    public void CalculateNextStates()
    {
        int prev_layer = currentLayer - 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int state = states[x, y,prev_layer];

                // neighbor total states 
                int statesNB = GetNeighborStatesSum(x, y,prev_layer);

                int nextState = state;

                if (state == 1)
                {
                    if (statesNB < rule[0])
                    {
                        nextState = 0;
                    }

                    if (statesNB >= rule[0] && statesNB <= rule[1])
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

                nextStates[x, y,currentLayer] = nextState;
            }
        }


    }

    void UpdateCells()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = cells[x, y,currentLayer];

                int newState = nextStates[x, y,currentLayer];

                cell.SetState(newState);

                if (newState == 1)
                {
                    int prev_age = currentLayer > 0 ? cells[x, y, currentLayer - 1].age : 0;
                    int age = prev_age + 1;
                    cell.SetAge(age);
                    cell.SetColor(GetColorByAge(age));
                }

                states[x, y,currentLayer] = newState;
            }
        }
    }



    Color GetColorByAge(int age)
    {
        float t = age / (float)layerCount;

        return gradient.Evaluate(t);
    }

    int GetNeighborStatesSum(int x, int y,int layer)
    {
        int sum = 0;

        for (int iy = -1; iy < 2; iy++)
        {
            for (int ix = -1; ix < 2; ix++)
            {
                if (ix == 0 && iy == 0)
                {
                    continue;
                }

                int nx = x + ix;
                int ny = y + iy;

                if (nx >= 0 && nx < width && ny > 0 && ny < height)
                {
                    int stateN = states[nx, ny,layer];
                    sum += stateN;
                }
            }
        }

        return sum;
    }
}
