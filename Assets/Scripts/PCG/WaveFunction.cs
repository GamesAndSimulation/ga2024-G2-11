using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimentions;
    public TileData[] AvailableTiles;
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
                cell.CreateCell(false, GetAllTileRotations());
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

        yield return new WaitForSeconds(3f);
        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<Cell> tempGrid)
    {
        Debug.Log($"tempgrid count {tempGrid.Count}");
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        Debug.Log($"cellToCollapse tile options length: {cellToCollapse.tileOptions.Length}");
        TileData.TileRotation selectedTileRotation = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
        cellToCollapse.tileOptions = new TileData.TileRotation[] { selectedTileRotation };

        Instantiate(selectedTileRotation.tile.tilePrefab, cellToCollapse.transform.position, Quaternion.Euler(-90,0 , selectedTileRotation.zRotation));
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
                    List<TileData.TileRotation> options = new List<TileData.TileRotation>(GetAllTileRotations());

                    // Update above
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimentions];
                        List<TileData.TileRotation> validOptions = GetValidOptions(up.tileOptions, t => t.downNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Update right
                    if (x < dimentions - 1)
                    {
                        Cell right = gridComponents[x + 1 + y * dimentions];
                        List<TileData.TileRotation> validOptions = GetValidOptions(right.tileOptions, t => t.leftNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Look down
                    if (y < dimentions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimentions];
                        List<TileData.TileRotation> validOptions = GetValidOptions(down.tileOptions, t => t.upNeighbours);
                        CheckValidity(options, validOptions);
                    }

                    // Look left
                    if (x > 0)
                    {
                        Cell left = gridComponents[x - 1 + y * dimentions];
                        List<TileData.TileRotation> validOptions = GetValidOptions(left.tileOptions, t => t.rightNeighbours);
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

    private List<TileData.TileRotation> GetValidOptions(TileData.TileRotation[] tileOptions, Func<TileData, TileData.TileRotation[]> getNeighbours)
    {
        List<TileData.TileRotation> validOptions = new List<TileData.TileRotation>();
        foreach (var tileRotation in tileOptions)
        {
            var neighbours = getNeighbours(tileRotation.tile);
            validOptions.AddRange(neighbours);
        }
        return validOptions.Distinct().ToList();
    }

    void CheckValidity(List<TileData.TileRotation> optionList, List<TileData.TileRotation> validOptions)
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

    private TileData.TileRotation[] GetAllTileRotations()
    {
        TileData[] tileDataArray = FindAllTileDataAssets();
        List<TileData.TileRotation> allTileRotations = new List<TileData.TileRotation>();

        foreach (var tileData in tileDataArray)
        {
            foreach (var rotation in tileData.upNeighbours)
            {
                allTileRotations.Add(rotation);
            }
            foreach (var rotation in tileData.rightNeighbours)
            {
                allTileRotations.Add(rotation);
            }
            foreach (var rotation in tileData.downNeighbours)
            {
                allTileRotations.Add(rotation);
            }
            foreach (var rotation in tileData.leftNeighbours)
            {
                allTileRotations.Add(rotation);
            }
        }

        return allTileRotations.ToArray();
    }

    private TileData[] FindAllTileDataAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:TileData");
        TileData[] tiles = new TileData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            tiles[i] = AssetDatabase.LoadAssetAtPath<TileData>(path);
        }
        return tiles;
    }
}
