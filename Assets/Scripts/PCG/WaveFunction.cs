using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveFunction : MonoBehaviour
{
    [Header("Wave Function Settings")] public int GridDimentions;

    public float OuterWallsHeight = 80f;
    [Range(0.1f, 10.0f)] public float TileAnimationDuration = 0.15f;
    public GameObject LoadingScreen;
    public Material WallMaterial;

    [Header("Possible Tiles")] public TileData[] TileDatas;

    public Cell cellObj;

    [Header("Colors")] public Color WallColor = Color.black;

    public Color TileColor = Color.white;

    public float LevelScaleMultiplier = 20;
    public float CellSize;

    private readonly List<TilePrototype> AvailablePrototypes = new();

    private Vector2Int playerCoords;
    private Vector2Int corridorCoords;

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
        playerCoords = new Vector2Int(-1, -1);
        GetAllPrototypes();
        InitializeGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log($"Player standing in cell {GetCellUnderPlayer()}");
            RegenerateWaveFunction();
        }
    }

    private void MakeCorridor()
    {
        int x = Random.Range(4, GridDimentions - 4);
        int y = Random.Range(4, GridDimentions - 4);

        corridorCoords = new Vector2Int(x, y);

        for (int i = 0; i < 4; i++)
        {
            var cell = gridComponents.Find(c => c.gridCoordinates == new Vector2Int(x, y + i));
            Instantiate(Resources.Load("Prefabs/Board/CornerSphere"), cell.transform.position, Quaternion.identity,
                transform);
            cell.collapsed = true;
            cell.tileOptions.Clear();
            cell.tileOptions.Add(AvailablePrototypes.Find(p =>
                p.TilePrefabShape == TileData.TileShape.Wall && p.Rotation == 90));
            SpawnTileInCell(cell, cell.tileOptions[0]);
            PropagateChanges(cell);
            cell = gridComponents.Find(c => c.gridCoordinates == new Vector2Int(x - 1, y + i));
            cell.tileOptions.Clear();
            cell.tileOptions.Add(AvailablePrototypes.Find(p =>
                p.TilePrefabShape == TileData.TileShape.Wall && p.Rotation == 270));
            SpawnTileInCell(cell, cell.tileOptions[0]);
            PropagateChanges(cell);
            if (i == 1)
            {
                Vector3 puzzlePos = cell.transform.position + Vector3.up * 2f + Vector3.right * 2.5f;
                Instantiate(Resources.Load("Prefabs/Puzzles/Puzzle (Medium)"), puzzlePos, Quaternion.identity, null);
            }
        }
    }

    public void RegenerateWaveFunction()
    {
        playerCoords = GetCellUnderPlayer();
        foreach (var cell in gridComponents)
        {
            if (cell != gridComponents.Find(c => c.gridCoordinates == playerCoords))
            {
                Destroy(cell.instantiatedTile);
                Destroy(cell.gameObject);
            }
        }

        //clear gridcomponents except the player cell
        gridComponents.RemoveAll(c => c.gridCoordinates != playerCoords);
        iterations = 0;
        InitializeGrid(true);
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

    private void InitializeGrid(bool firstCellInPlayer = false)
    {
        if (!firstCellInPlayer)
        {
            LoadingScreen.SetActive(true);
            LoadingScreen.GetComponentInChildren<UIFadeInOut>().enabled = true;
        }

        var cellCount = 0;
        for (var y = 0; y < GridDimentions; y++)
        for (var x = 0; x < GridDimentions; x++)
        {
            cellCount++;
            if (firstCellInPlayer && playerCoords.x == x && playerCoords.y == y)
            {
                continue;
            }

            var cell = Instantiate(cellObj, new Vector3(x * 5, 0, y * 5), Quaternion.identity, transform);
            cell.tag = "WFCcell";
            if (AvailablePrototypes.Count == 0)
            {
                Debug.LogError("No available prototypes");
                return;
            }

            cell.CreateCell(false, AvailablePrototypes, x, y);
            gridComponents.Add(cell);
            if (cellCount % 2 == 0)
                cell.GetComponentInChildren<Light>().gameObject.SetActive(false);
        }

        //if (!firstCellInPlayer)
        //{
        //    CreateSealingWalls();
        //}

        MakeCorridor();

        StartCoroutine(!firstCellInPlayer
            ? CollapseWaveFunctionWithAnim(true, Vector2Int.zero)
            : CollapseWaveFunctionWithAnim(false, playerCoords));
    }

    private void CreateSealingWalls()
    {
        var cellSize = 5f; // New size of each cell
        var gridSize = GridDimentions;
        var wallThickness = 0.1f * cellSize;
        var segmentHeight = OuterWallsHeight / 6; // Height of each wall segment


        foreach (var wall in outerWalls)
            if (wall != null)
                Destroy(wall);

        var numHorizontalSegments = gridSize;
        var numVerticalSegments = Mathf.CeilToInt(OuterWallsHeight / segmentHeight);

        // Create segmented walls
        for (var i = 0; i < GridDimentions; i++)
        for (var j = 0; j < numVerticalSegments; j++)
        {
            //Left
            var wall = CreateWall(new Vector3(-cellSize / 2, j * cellSize, i * cellSize),
                new Vector3(wallThickness, segmentHeight, cellSize), WallMaterial);
            outerWalls[0] = wall;

            //Right
            wall = CreateWall(new Vector3(GridDimentions * 5f - cellSize / 2, j * cellSize, i * cellSize),
                new Vector3(wallThickness, segmentHeight, cellSize), WallMaterial);
            outerWalls[1] = wall;

            //Top
            wall = CreateWall(new Vector3(i * cellSize, j * cellSize, GridDimentions * 5f - cellSize / 2),
                new Vector3(cellSize, segmentHeight, wallThickness), WallMaterial);
            outerWalls[2] = wall;

            //Bottom
            wall = CreateWall(new Vector3(i * cellSize, j * cellSize, -cellSize / 2),
                new Vector3(cellSize, segmentHeight, wallThickness), WallMaterial);
            outerWalls[3] = wall;
        }
    }

    private GameObject CreateWall(Vector3 position, Vector3 scale, Material material)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.parent = transform;
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = material;
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

    private IEnumerator CollapseWaveFunctionWithAnim(bool useRandomFirstCell, Vector2Int firstCell)
    {
        // get cell from coordinates
        var startCell = gridComponents.Find(c => c.gridCoordinates == firstCell);
        if (useRandomFirstCell)
            startCell = gridComponents[Random.Range(0, gridComponents.Count)];
        //CollapseCell(startCell);
        PropagateChanges(startCell);

        // Continue with the usual process
        while (!IsFunctionCollapsed())
        {
            IterateWFC();
            yield return null;
            iterations++;
        }


        //transform.localScale *= LevelScaleMaultiplier;

        //UpdateColliders();

        if (useRandomFirstCell)
        {
            PlacePlayerAtSpawnPoint(GetBestSpawnCorridor());
        }


        LoadingScreen.SetActive(false);
        LoadingScreen.GetComponentInChildren<UIFadeInOut>().enabled = false;
    }

    private void PlacePlayerAtSpawnPoint(Vector2Int spawnPoint)
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {

            player.transform.parent = transform;
            player.transform.position = gridComponents.Find(c => c.gridCoordinates == spawnPoint).transform.position + Vector3.up * 4;
            player.transform.parent = null;
            //Instantiate(Resources.Load("Prefabs/Board/CornerSphere"), worldSpawnPoint, Quaternion.identity, transform);
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
        SpawnTileInCell(cell, chosenTile);
    }

    private void SpawnTileInCell(Cell cell, TilePrototype chosenTile)
    {
        cell.RecreateCell(new List<TilePrototype> { chosenTile });
        cell.instantiatedTile = Instantiate(chosenTile.TilePrefab, cell.transform.position,
            Quaternion.Euler(-90, 0, chosenTile.Rotation), cell.transform);
        var tempScale = cell.instantiatedTile.transform.localScale;
        cell.instantiatedTile.transform.localScale = Vector3.zero; // Start from zero scale
        cell.instantiatedTile.transform.DOScale(tempScale, TileAnimationDuration).SetEase(Ease.OutBounce);
    }

    private Vector2Int GetCellUnderPlayer()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 10f))
            {
                if (hit.collider.transform.parent.gameObject.CompareTag("WFCcell"))
                    return hit.collider.transform.parent.GetComponent<Cell>().gridCoordinates;
            }
        }

        return Vector2Int.zero;
    }

    private Cell GetCellWithLowestEntropy()
    {
        var lowestEntropyCells = new List<Cell>();
        var lowestEntropy = float.MaxValue;

        foreach (var cell in gridComponents)
        {
            if (cell.collapsed ||
                (cell.gridCoordinates.x == playerCoords.x && cell.gridCoordinates.y == playerCoords.y)) continue;
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

    // Get farthes cell from the corridor
    private Vector2Int GetBestSpawnCorridor()
    {
        var visited = new HashSet<Cell>();
        var queue = new Queue<Cell>();
        var distanceMap = new Dictionary<Cell, int>();
        visited.Clear();
        queue.Clear();
        Vector2Int farthestCell = Vector2Int.zero;

        if (gridComponents.Count > 0)
        {
            var startCell = gridComponents.Find(c => c.gridCoordinates == corridorCoords);
            queue.Enqueue(startCell);
            visited.Add(startCell);
            distanceMap[startCell] = 0;
            farthestCell = corridorCoords;

            while (queue.Count > 0)
            {
                var currentCell = queue.Dequeue();
                int currentDistance = distanceMap[currentCell];
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
                            distanceMap[neighbor] = currentDistance + 1;

                            if (distanceMap[neighbor] > distanceMap[gridComponents.Find(c => c.gridCoordinates == farthestCell)])
                            {
                                farthestCell = neighbor.gridCoordinates;
                            }
                        }
                    }
                    directionIndex++;
                }
            }
        }

        Cell farthestCellObj = gridComponents.Find(c => c.gridCoordinates == farthestCell);
        Instantiate(Resources.Load("Prefabs/Board/CornerSphere"), farthestCellObj.transform.position, Quaternion.identity, transform);

        return farthestCell;

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
        return new Vector2Int(bestSpawnPoint.x * 5, bestSpawnPoint.y * 5);
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
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }

                    directionIndex++;
                }
            }

            yield return null;
        }

        foreach (var cell in gridComponents)
            if (!visited.Contains(cell))
            {
                break;
            }

        yield return null;
    }
}