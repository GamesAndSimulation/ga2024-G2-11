using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimentions;
    public Tile.TileRotation[] tileObjects;
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
        for (int y = 0; y < dimentions; y++)
        {
            for (int x = 0; x < dimentions; x++)
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

        for (int i = 0; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
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
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        Debug.Log($"cellToCollapse tile options length: {cellToCollapse.tileOptions.Length}");
        Tile.TileRotation selectedTileRotation = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new Tile.TileRotation[] { selectedTileRotation };

        Instantiate(selectedTileRotation.tile, cellToCollapse.transform.position, Quaternion.Euler(-90, 0, selectedTileRotation.zRotation));
        UpdateGeneration();
    }

    private void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int y = 0; y < dimentions; y++)
        {
            for (int x = 0; x < dimentions; x++)
            {
                var index = x + y * dimentions;
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile.TileRotation> options = new List<Tile.TileRotation>(tileObjects);

                    // Update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimentions];
                        List<Tile.TileRotation> validOptions = GetValidOptions(up.tileOptions, t => t.upNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Update right
                    if (x < dimentions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimentions];
                        List<Tile.TileRotation> validOptions = GetValidOptions(right.tileOptions, t => t.leftNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Look down
                    if (y < dimentions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimentions];
                        List<Tile.TileRotation> validOptions = GetValidOptions(down.tileOptions, t => t.downNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimentions];
                        List<Tile.TileRotation> validOptions = GetValidOptions(left.tileOptions, t => t.rightNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    newGenerationCell[index].RecreateCell(options.ToArray());
                }
            }
        }
        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimentions * dimentions)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    private List<Tile.TileRotation> GetValidOptions(Tile.TileRotation[] tileOptions, Func<Tile, Tile.TileRotation[]> getNeighbours)
    {
        List<Tile.TileRotation> validOptions = new List<Tile.TileRotation>();
        foreach (var tileRotation in tileOptions)
        {
            var neighbours = getNeighbours(tileRotation.tile);
            validOptions.AddRange(neighbours);
        }
        return validOptions.Distinct().ToList();
    }

    void CheckValidity(List<Tile.TileRotation> optionList, List<Tile.TileRotation> validOptions)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOptions.Any(v => v.tile == element.tile))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
