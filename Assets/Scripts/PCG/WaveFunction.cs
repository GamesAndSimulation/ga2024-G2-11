using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public int dimentions; // Grid dimensions
    public TileData[] TileDatas;
    public List<TilePrototype> AvailablePrototypes = new List<TilePrototype>(); // Initialize the list
    private List<Cell> gridComponents; // A grid of cells
    public Cell cellObj;
    
    [Range(0.1f, 1.0f)]
    public float TileAnimationDuration = 0.15f;
    

    private int iterations;

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
    
    public void RegenerateWaveFunction()
    {
        foreach (var cell in gridComponents)
        {
            Destroy(cell.instantiatedTile);
            Destroy(cell.gameObject);
        }
        gridComponents.Clear();
        iterations = 0;
        InitializeGrid();
    }

    private void GetAllPrototypes()
    {
        foreach (var tile in TileDatas)
        {
            tile.ComputePrototypes();
            foreach (var prototype in tile.Prototypes)
            {
                AvailablePrototypes.Add(prototype);
            }
        }

        // Compute neighbors for each prototype
        foreach (var prototype in AvailablePrototypes)
        {
            prototype.Neighbors = new List<TilePrototype>[4]; // posX, negZ, negX, posZ
            for(int i = 0; i < 4; i++)
            {
                prototype.Neighbors[i] = new List<TilePrototype>();
                foreach(TilePrototype neighbor in AvailablePrototypes)
                {
                    if(prototype.Sockets[i].ToString().EndsWith("S")) // Asymmetric
                    {
                        if(prototype.Sockets[i] == TileData.SideType.LLS && neighbor.Sockets[(i + 2) % 4] == TileData.SideType.RLS
                           || prototype.Sockets[i] == TileData.SideType.RLS && neighbor.Sockets[(i + 2) % 4] == TileData.SideType.LLS)
                        {
                            prototype.Neighbors[i].Add(neighbor);
                        }
                        
                    }
                    else if(prototype.Sockets[i] == neighbor.Sockets[(i + 2) % 4])
                    {
                        prototype.Neighbors[i].Add(neighbor);
                    }
                    
                }
            }
            //tile.ComputePrototypesNeighbors();
        }
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimentions; y++)
        {
            for (int x = 0; x < dimentions; x++)
            {
                Cell cell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity, transform);
                if (AvailablePrototypes.Count == 0)
                {   
                    Debug.LogError("No available prototypes");
                    return;
                }
                cell.CreateCell(false, AvailablePrototypes, x, y);
                gridComponents.Add(cell);
            }
        }
        StartCoroutine(CollapseWaveFunctionWithAnim());
    }

    private void CollapseWaveFunction()
    {
        // Pick a random starting cell to collapse
        Cell startCell = gridComponents[UnityEngine.Random.Range(0, gridComponents.Count)];
        CollapseCell(startCell);
        PropagateChanges(startCell);

        // Continue with the usual process
        while (!IsFunctionCollapsed())
        {
            IterateWFC();
            iterations++;
        }
        StartCoroutine(EnsureConnectivity());
    }
    
    private IEnumerator CollapseWaveFunctionWithAnim()
    {
        // Pick a random starting cell to collapse
        Cell startCell = gridComponents[UnityEngine.Random.Range(0, gridComponents.Count)];
        CollapseCell(startCell);
        PropagateChanges(startCell);

        // Continue with the usual process
        while (!IsFunctionCollapsed())
        {
            IterateWFC();
            yield return null;
            iterations++;
        }
        //StartCoroutine(EnsureConnectivity());
    }

    private void IterateWFC()
    {
        Cell cell = GetCellWithLowestEntropy();
        if (cell == null)
        {
            Debug.LogError("Failed to find cell with lowest entropy.");
            return;
        }
        CollapseCell(cell);
        PropagateChanges(cell);
    }

    private void PropagateChanges(Cell cell)
    {
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(cell);

        while (queue.Count > 0)
        {
            Cell currentCell = queue.Dequeue();
            for (int directionIndex = 0; directionIndex < directions.Length; directionIndex++)
            {
                Vector2Int direction = directions[directionIndex];
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

                bool neighborChanged = false;
                List<TilePrototype> validNeighbors = GetPossibleNeighbors(currentCell, directionIndex);
                for (int i = neighbor.tileOptions.Count - 1; i >= 0; i--)
                {
                    if (!validNeighbors.Contains(neighbor.tileOptions[i]))
                    {
                        neighbor.tileOptions.RemoveAt(i);
                        neighborChanged = true;
                    }
                }

                if (neighbor.tileOptions.Count == 0)
                {
                    Debug.LogError("Propagation led to an empty tileOptions at " + neighbor.gridCoordinates);
                    return; // or handle the contradiction appropriately
                }

                if (neighborChanged)
                {
                    queue.Enqueue(neighbor); // Only enqueue if changes were made
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
        if (possibleNeighbors.Count == 0)
        {
            Debug.LogWarning("No possible neighbors");
        }
        return possibleNeighbors;
    }

    private void CollapseCell(Cell cell)
    {
        cell.collapsed = true;
        int randomIndex = UnityEngine.Random.Range(0, cell.tileOptions.Count);
        if (randomIndex < 0 || randomIndex >= cell.tileOptions.Count)
        {
            Debug.LogError($"{iterations} : Random index {randomIndex} out of range for tile options count {cell.tileOptions.Count}");
            return;
        }
        TilePrototype chosenTile = cell.tileOptions[randomIndex];
        cell.RecreateCell(new List<TilePrototype> { chosenTile });
        cell.instantiatedTile = Instantiate(chosenTile.TilePrefab, cell.transform.position, Quaternion.Euler(-90, 0, chosenTile.Rotation), transform);
        Vector3 tempScale = cell.instantiatedTile.transform.localScale;
        cell.instantiatedTile.transform.localScale = Vector3.zero; // Start from zero scale
        cell.instantiatedTile.transform.DOScale(tempScale, TileAnimationDuration).SetEase(Ease.OutBounce);
    }

    private Cell GetCellWithLowestEntropy()
    {
        List<Cell> lowestEntropyCells = new List<Cell>();
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
                lowestEntropyCells.Clear();
                lowestEntropyCells.Add(cell);
            }
            else if (entropy == lowestEntropy)
            {
                lowestEntropyCells.Add(cell);
            }
        }

        if (lowestEntropyCells.Count == 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, lowestEntropyCells.Count);
        return lowestEntropyCells[randomIndex];
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
    
    private IEnumerator EnsureConnectivity()
    {
        bool allConnected;
        HashSet<Cell> visited = new HashSet<Cell>();
        Queue<Cell> queue = new Queue<Cell>();

        do
        {
            allConnected = true;
            visited.Clear();
            queue.Clear();

            if (gridComponents.Count > 0)
            {
                queue.Enqueue(gridComponents[0]);
                visited.Add(gridComponents[0]);

                while (queue.Count > 0)
                {
                    Cell currentCell = queue.Dequeue();
                    int directionIndex = 0;
                    foreach (var direction in directions)
                    {
                        Vector2Int neighborCoordinates = currentCell.gridCoordinates + direction;
                        Cell neighbor = gridComponents.Find(c => c.gridCoordinates == neighborCoordinates);
                        if (neighbor != null && !visited.Contains(neighbor))
                        {

                            var oppositeSide = neighbor.tileOptions[0].Sockets[TileData.Mod(directionIndex + 2, 4)];
                            if (oppositeSide != TileData.SideType.LF || neighbor.tileOptions[0].TilePrefabShape == TileData.TileShape.Door)
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                        directionIndex++;
                    }
                }

                yield return null; // Yield control to avoid freezing
            }

            // If there are unvisited cells, connect them
            foreach (var cell in gridComponents)
            {
                if (!visited.Contains(cell))
                {
                    Debug.LogWarning($"Isolated cell at {cell.gridCoordinates}. Connecting...");
                    cell.ReplacePrefabWithDoor(); // Implement this method to replace with a door prefab
                    allConnected = false;
                    break;
                }
            }
        
            yield return null; // Yield control to avoid freezing
        } while (!allConnected);

        Debug.Log("All cells are connected.");
    }
}

