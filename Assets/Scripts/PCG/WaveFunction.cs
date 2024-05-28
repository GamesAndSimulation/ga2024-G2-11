using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimentions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    private int iterations = 0;

    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for(int y = 0; y < dimentions; y++)
        {
            for(int x = 0; x < dimentions; x++)
            {
                Cell cell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity);
                cell.CreateCell(false, tileObjects);
                gridComponents.Add(cell);
            }
        }
        StartCoroutine(CheckEntropy());
    }
    
    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;
        
        for(int i = 0; i < tempGrid.Count; i++)
        {
            if(tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.1f);
        CollapseCell(tempGrid);
        //tempGrid.RemoveAll(c => c.collapsed);
        //tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
        //tempGrid.RemoveAll(x => x.tileOptions.Length != tempGrid[0].tileOptions.Length);
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(1, tempGrid.Count);
        
        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        Debug.Log($"cellToCollaps tile options length: {cellToCollapse.tileOptions.Length}");
        Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new Tile[] { selectedTile };

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.Euler(-90, 0, 0));
        UpdateGeneration();

    }

    private void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);
        
        for(int y = 0; y < dimentions; y++)
        {
            for(int x = 0; x < dimentions; x++)
            {
                var index = x + y * dimentions;
                if(gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

                    // Update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimentions];
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in up.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOption);
                            var valid = tileObjects[valOption].upNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }
                    
                    // Update right
                    if (x < dimentions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimentions].GetComponent<Cell>();
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in right.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOption);
                            var valid = tileObjects[valOption].leftNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }
                    
                    //Look down
                    if (y < dimentions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimentions].GetComponent<Cell>();
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in down.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOption);
                            var valid = tileObjects[valOption].downNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }
                    
                    //Look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimentions].GetComponent<Cell>();
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in left.tileOptions)
                        {
                            var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOption);
                            var valid = tileObjects[valOption].rightNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }

                    Tile[] newTileList = new Tile[options.Count];
                    
                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }
                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }
        gridComponents = newGenerationCell;
        iterations++;
        
        if(iterations < dimentions * dimentions)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOptions)
    {
        for(int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if(!validOptions.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}

