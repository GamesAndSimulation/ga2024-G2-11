using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveFunction : MonoBehaviour
{
    [Header("Wave Function Settings")] public int GridDimentions;

    public float OuterWallsHeight = 3f;
    [Range(0.1f, 1.0f)] public float TileAnimationDuration = 0.15f;
    public GameObject LoadingScreen;

    [Header("Possible Tiles")] public TileData[] TileDatas;

    public Cell cellObj;

    [Header("Colors")] public Color WallColor = Color.black;

    public Color TileColor = Color.white;

    public float LevelScaleMultiplier = 20;

    private readonly List<TilePrototype> AvailablePrototypes = new();

    private readonly Vector2Int[] directions =
    {
        new(1, 0),
        new(0, -1),
        new(-1, 0),
        new(0, 1)
    };

    private List<Cell> gridComponents; // A grid of cells


    private int iterations;

    private GameObject[] outerWalls;

    private void Awake()
    {
        outerWalls = new GameObject[4];
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
            foreach (var prototype in tile.Prototypes) AvailablePrototypes.Add(prototype);
        }

        // Compute neighbors for each prototype
        foreach (var prototype in AvailablePrototypes)
        {
            prototype.Neighbors = new List<TilePrototype>[4]; // posX, negZ, negX, posZ
            for (var i = 0; i < 4; i++)
            {
                prototype.Neighbors[i] = new List<TilePrototype>();
                foreach (var neighbor in AvailablePrototypes)
                    if (prototype.Sockets[i].ToString().EndsWith("S")) // Asymmetric
                    {
                        if ((prototype.Sockets[i] == TileData.SideType.LLS &&
                             neighbor.Sockets[(i + 2) % 4] == TileData.SideType.RLS)
                            || (prototype.Sockets[i] == TileData.SideType.RLS &&
                                neighbor.Sockets[(i + 2) % 4] == TileData.SideType.LLS))
                            prototype.Neighbors[i].Add(neighbor);
                    }
                    else if (prototype.Sockets[i] == neighbor.Sockets[(i + 2) % 4])
                    {
                        prototype.Neighbors[i].Add(neighbor);
                    }
            }
            //tile.ComputePrototypesNeighbors();
        }
    }

    private void InitializeGrid()
    {
        LoadingScreen.SetActive(true);
        for (var y = 0; y < GridDimentions; y++)
        for (var x = 0; x < GridDimentions; x++)
        {
            var cell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity, transform);
            if (AvailablePrototypes.Count == 0)
            {
                Debug.LogError("No available prototypes");
                return;
            }

            cell.CreateCell(false, AvailablePrototypes, x, y);
            gridComponents.Add(cell);
        }

        CreateSealingWalls();

        StartCoroutine(CollapseWaveFunctionWithAnim());
    }

    private void CreateSealingWalls()
    {
        float gridSize = GridDimentions;
        var wallThickness = 0.1f;

        // Create a material for the walls
        var wallMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        wallMaterial.color = WallColor;

        foreach (var wall in outerWalls)
            if (wall != null)
                Destroy(wall);

        // Create walls
        // Left wall
        outerWalls[0] = CreateWall(new Vector3(-0.5f, 0.5f, gridSize / 2 - 0.5f),
            new Vector3(wallThickness, OuterWallsHeight, gridSize),
            wallMaterial);
        // Right wall
        outerWalls[1] = CreateWall(new Vector3(gridSize - 0.5f + wallThickness, 0.5f, gridSize / 2 - 0.5f),
            new Vector3(wallThickness, OuterWallsHeight, gridSize), wallMaterial);
        // Top wall
        outerWalls[2] = CreateWall(new Vector3(gridSize / 2 - 0.5f, 0.5f, -0.5f),
            new Vector3(gridSize, OuterWallsHeight, wallThickness),
            wallMaterial);
        // Bottom wall
        outerWalls[3] = CreateWall(new Vector3(gridSize / 2 - 0.5f, 0.5f, gridSize - 0.5f + wallThickness),
            new Vector3(gridSize, OuterWallsHeight, wallThickness), wallMaterial);
    }

    private GameObject CreateWall(Vector3 position, Vector3 scale, Material material)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = material;
        wall.transform.parent = transform;
        return wall;
    }

    private void CollapseWaveFunction()
    {
        // Pick a random starting cell to collapse
        var startCell = gridComponents[Random.Range(0, gridComponents.Count)];
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
        var startCell = gridComponents[Random.Range(0, gridComponents.Count)];
        CollapseCell(startCell);
        PropagateChanges(startCell);

        // Continue with the usual process
        while (!IsFunctionCollapsed())
        {
            IterateWFC();
            yield return null;
            iterations++;
        }

        var spawnPoint = GetBestSpawnPoint();

        transform.localScale *= LevelScaleMultiplier;

        UpdateColliders();

        PlacePlayerAtSpawnPoint(spawnPoint);

        yield return new WaitForSeconds(0.3f);
        GameObject.FindWithTag("Player").GetComponent<Rigidbody>().useGravity = true;

        LoadingScreen.SetActive(false);
    }

    private void UpdateColliders()
    {
        foreach (var cell in gridComponents)
            if (cell.instantiatedTile != null)
            {
                var existingCollider = cell.instantiatedTile.GetComponent<MeshCollider>();
                if (existingCollider != null) Destroy(existingCollider); // Remove the existing collider

                var meshFilter = cell.instantiatedTile.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    var meshCollider = cell.instantiatedTile.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                    meshCollider.convex = false; // Adjust based on your requirements
                }
                else
                {
                    Debug.LogError("MeshFilter not found on the instantiated tile.");
                }
            }
            else
            {
                Debug.LogError("Instantiated tile is null.");
            }
    }


    private void PlacePlayerAtSpawnPoint(Vector2Int spawnPoint)
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var worldSpawnPoint = new Vector3(
                spawnPoint.x * gridComponents[0].transform.localScale.x * LevelScaleMultiplier,
                1.5f * gridComponents[0].transform.localScale.y * LevelScaleMultiplier,
                spawnPoint.y * gridComponents[0].transform.localScale.z * LevelScaleMultiplier);

            player.transform.parent = transform;
            player.transform.position = worldSpawnPoint;
            player.transform.parent = null;
            Instantiate(Resources.Load("Prefabs/Board/CornerSphere"), worldSpawnPoint, Quaternion.identity, transform);
        }
    }

    private void IterateWFC()
    {
        var cell = GetCellWithLowestEntropy();
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
        var queue = new Queue<Cell>();
        queue.Enqueue(cell);

        while (queue.Count > 0)
        {
            var currentCell = queue.Dequeue();
            for (var directionIndex = 0; directionIndex < directions.Length; directionIndex++)
            {
                var direction = directions[directionIndex];
                var neighborCoordinates = currentCell.gridCoordinates + direction;
                if (neighborCoordinates.x < 0 || neighborCoordinates.x >= GridDimentions || neighborCoordinates.y < 0 ||
                    neighborCoordinates.y >= GridDimentions) continue;

                var neighbor = gridComponents.Find(c => c.gridCoordinates == neighborCoordinates);
                if (neighbor.collapsed) continue;

                var neighborChanged = false;
                var validNeighbors = GetPossibleNeighbors(currentCell, directionIndex);
                for (var i = neighbor.tileOptions.Count - 1; i >= 0; i--)
                    if (!validNeighbors.Contains(neighbor.tileOptions[i]))
                    {
                        neighbor.tileOptions.RemoveAt(i);
                        neighborChanged = true;
                    }

                if (neighbor.tileOptions.Count == 0)
                {
                    Debug.LogError("Propagation led to an empty tileOptions at " + neighbor.gridCoordinates);
                    return; // or handle the contradiction appropriately
                }

                if (neighborChanged) queue.Enqueue(neighbor); // Only enqueue if changes were made
            }
        }
    }

    private List<TilePrototype> GetPossibleNeighbors(Cell currentCell, int directionIndex)
    {
        var possibleNeighbors = new List<TilePrototype>();
        foreach (var prototype in currentCell.tileOptions)
            possibleNeighbors.AddRange(prototype.Neighbors[directionIndex]);
        if (possibleNeighbors.Count == 0) Debug.LogWarning("No possible neighbors");
        return possibleNeighbors;
    }

    private void CollapseCell(Cell cell)
    {
        cell.collapsed = true;
        var randomIndex = Random.Range(0, cell.tileOptions.Count);
        if (randomIndex < 0 || randomIndex >= cell.tileOptions.Count)
        {
            Debug.LogError(
                $"{iterations} : Random index {randomIndex} out of range for tile options count {cell.tileOptions.Count}");
            return;
        }

        var chosenTile = cell.tileOptions[randomIndex];
        cell.RecreateCell(new List<TilePrototype> { chosenTile });
        cell.instantiatedTile = Instantiate(chosenTile.TilePrefab, cell.transform.position,
            Quaternion.Euler(-90, 0, chosenTile.Rotation), transform);
        var tempScale = cell.instantiatedTile.transform.localScale;
        //cell.instantiatedTile.GetComponent<MeshRenderer>().material.color = TileColor;
        cell.instantiatedTile.transform.localScale = Vector3.zero; // Start from zero scale
        cell.instantiatedTile.transform.DOScale(tempScale, TileAnimationDuration).SetEase(Ease.OutBounce);
    }

    private Cell GetCellWithLowestEntropy()
    {
        var lowestEntropyCells = new List<Cell>();
        var lowestEntropy = float.MaxValue;

        foreach (var cell in gridComponents)
        {
            if (cell.collapsed) continue;
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

        if (lowestEntropyCells.Count == 0) return null;

        var randomIndex = Random.Range(0, lowestEntropyCells.Count);
        return lowestEntropyCells[randomIndex];
    }

    private bool IsFunctionCollapsed()
    {
        foreach (var cell in gridComponents)
            if (!cell.collapsed)
                return false;
        return true;
    }

    private Vector2Int GetBestSpawnPoint()
    {
        var visited = new HashSet<Cell>();
        var queue = new Queue<Cell>();
        visited.Clear();
        queue.Clear();

        var maxTiles = 0;
        var bestSpawnPoint = Vector2Int.zero;

        foreach (var cell in gridComponents)
            if (!visited.Contains(cell))
            {
                var currentTiles = 0;
                queue.Enqueue(cell);
                visited.Add(cell);

                while (queue.Count > 0)
                {
                    var currentCell = queue.Dequeue();
                    currentTiles++;
                    var directionIndex = 0;
                    foreach (var direction in directions)
                    {
                        var neighborCoordinates = currentCell.gridCoordinates + direction;
                        var neighbor = gridComponents.Find(c => c.gridCoordinates == neighborCoordinates);
                        if (neighbor != null && !visited.Contains(neighbor))
                        {
                            var oppositeSide = neighbor.tileOptions[0].Sockets[TileData.Mod(directionIndex + 2, 4)];
                            if (oppositeSide != TileData.SideType.LF ||
                                neighbor.tileOptions[0].TilePrefabShape == TileData.TileShape.Door)
                            {
                                visited.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }

                        directionIndex++;
                    }
                }

                if (currentTiles > maxTiles)
                {
                    maxTiles = currentTiles;
                    bestSpawnPoint = cell.gridCoordinates;
                }
            }

        //Instantiate(Resources.Load("Prefabs/Board/CornerSphere"), bestSpawnPoint + Vector3.up * 2, Quaternion.identity, transform);
        Debug.LogWarning($"spawn point: {bestSpawnPoint} with {maxTiles} tiles.");
        return bestSpawnPoint;
    }

    public void FloodFillWrapper()
    {
        foreach (var cell in gridComponents)
            cell.instantiatedTile.GetComponent<MeshRenderer>().material.color = TileColor;
        StartCoroutine(EnsureConnectivity());
    }

    private IEnumerator EnsureConnectivity()
    {
        var visited = new HashSet<Cell>();
        var queue = new Queue<Cell>();
        visited.Clear();
        queue.Clear();

        if (gridComponents.Count > 0)
        {
            var randomIndex = Random.Range(0, gridComponents.Count);
            queue.Enqueue(gridComponents[randomIndex]);
            visited.Add(gridComponents[randomIndex]);

            while (queue.Count > 0)
            {
                var currentCell = queue.Dequeue();
                var directionIndex = 0;
                foreach (var direction in directions)
                {
                    var neighborCoordinates = currentCell.gridCoordinates + direction;
                    var neighbor = gridComponents.Find(c => c.gridCoordinates == neighborCoordinates);
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        var oppositeSide = neighbor.tileOptions[0].Sockets[TileData.Mod(directionIndex + 2, 4)];
                        if (oppositeSide != TileData.SideType.LF ||
                            neighbor.tileOptions[0].TilePrefabShape == TileData.TileShape.Door)
                        {
                            // Turn blue
                            neighbor.instantiatedTile.GetComponent<MeshRenderer>().material
                                .DOColor(Color.blue, TileAnimationDuration);
                            yield return new WaitForSeconds(0.001f);
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
            if (!visited.Contains(cell))
            {
                Debug.LogWarning($"Isolated cell at {cell.gridCoordinates}. Connecting...");
                break;
            }

        yield return null; // Yield control to avoid freezing
    }
}