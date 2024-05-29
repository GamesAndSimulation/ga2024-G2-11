using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimentions; // Grid dimensions
    public List<TilePrototype> AvailablePrototypes; // All possible prototypes with already computed neighbors
    public List<Cell> gridComponents; // A grid of cells
    public Cell cellObj;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1)
    };

    private void Awake()
    {
        gridComponents = new List<Cell>();
        GetAllPrototypes();
        InitializeGrid();
    }

    private void GetAllPrototypes()
    {
        var tileData = FindAllTileDataAssets();
        foreach (var tile in tileData)
        {
            foreach (var prototype in tile.Prototypes)
            {
                AvailablePrototypes.Add(prototype);
            }
        }
        
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimentions; y++)
        {
            for (int x = 0; x < dimentions; x++)
            {
                Cell cell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity);
                cell.CreateCell(false, AvailablePrototypes, x, y);
                gridComponents.Add(cell);
            }
        }
        StartCoroutine(CollapseFunction());
    }

    private IEnumerator CollapseFunction()
    {
        while(!IsFunctionCollapsed())
        {
            IterateWFC();
        }
    }
    
    private void IterateWFC()
    {
        Cell cell = GetCellWithLowestEntropy();
        CollapseCell(cell);
        PropagateChanges(cell);
    }

    private void PropagateChanges(Cell cell)
    {
        Stack<Cell> stack = new Stack<Cell>();
        while(stack.Count > 0)
        {
            Cell currentCell = stack.Pop();
            int directionIndex = 0;
            foreach (var direction in directions)
            {
                Vector2Int neighborCoordinates = currentCell.gridCoordinates + direction;
                if (neighborCoordinates.x < 0 || neighborCoordinates.x >= dimentions || neighborCoordinates.y < 0 || neighborCoordinates.y >= dimentions)
                {
                    continue;
                }
                Cell neighbor = gridComponents.Find(c => c.gridCoordinates == neighborCoordinates);
                if (neighbor.collapsed)
                {
                    continue;
                }
                
                var neighborPossiblePrototypes = neighbor.tileOptions.ToList();
                var cellPrototypeValidNeighbors = GetPossibleNeighbors(currentCell, directionIndex);

                if (neighborPossiblePrototypes.Count == 0)
                {
                    continue;
                }

                foreach (TilePrototype neighborPrototype in neighborPossiblePrototypes)
                {
                    if (!cellPrototypeValidNeighbors.Contains(neighborPrototype))
                    {
                        neighbor.tileOptions.Remove(neighborPrototype);
                    }
                }
                
            }
        }
        
    }
    
    private List<TilePrototype> GetPossibleNeighbors(Cell currentCell, int directionIndex)
    {
        List<TilePrototype> possibleNeighbors = new List<TilePrototype>();
        foreach (var prototype in currentCell.tileOptions)
        {
            possibleNeighbors.AddRange(prototype.Neighbors[directionIndex]);
        }
        return possibleNeighbors;
    }

    private void CollapseCell(Cell cell)
    {
        cell.collapsed = true;
        int randomIndex = UnityEngine.Random.Range(0, cell.tileOptions.Count);
        TilePrototype chosenTile = cell.tileOptions[randomIndex];
        cell.RecreateCell(new List<TilePrototype> {chosenTile});
        Instantiate(chosenTile.TilePrefab, cell.transform.position, Quaternion.Euler(-90, 0, chosenTile.Rotation));
    }

    private Cell GetCellWithLowestEntropy()
    {
        Cell lowestEntropyCell = null;
        float lowestEntropy = float.MaxValue;
        foreach (var cell in gridComponents)
        {
            if (cell.collapsed)
            {
                continue;
            }
            float entropy = cell.GetEntropy();
            if (entropy < lowestEntropy)
            {
                lowestEntropy = entropy;
                lowestEntropyCell = cell;
            }
        }
        return lowestEntropyCell;
    }

    private bool IsFunctionCollapsed()
    {
        foreach (var cell in gridComponents)
        {
            if (!cell.collapsed)
            {
                return false;
            }
        }
        return true;
    }


    // Auxiliary functions

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
