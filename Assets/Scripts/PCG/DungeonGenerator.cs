using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public GameObject hallwayPrefab;
    public int numberOfRooms = 10;
    public int gridSize = 10; // Size of each grid cell in world units
    public int gridWidth = 20;
    public int gridHeight = 20;
    public LayerMask roomLayerMask;

    private bool[,] grid;
    private List<GameObject> spawnedRooms = new List<GameObject>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        ClearDungeon();
        InitializeGrid();

        for (int i = 0; i < numberOfRooms; i++)
        {
            bool roomPlaced = false;
            int attempts = 0;

            while (!roomPlaced && attempts < 100)
            {
                attempts++;
                Vector2Int randomPosition = GetRandomGridPosition();
                GameObject randomRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
                Vector2Int roomSize = GetRoomSize(randomRoomPrefab);

                if (CanPlaceRoom(randomPosition, roomSize))
                {
                    PlaceRoom(randomPosition, roomSize, randomRoomPrefab);
                    roomPlaced = true;
                }
            }
        }

        //ConnectRooms();
    }

    void InitializeGrid()
    {
        grid = new bool[gridWidth, gridHeight];
    }

    Vector2Int GetRandomGridPosition()
    {
        int x = Random.Range(0, gridWidth);
        int y = Random.Range(0, gridHeight);
        return new Vector2Int(x, y);
    }

    Vector2Int GetRoomSize(GameObject roomPrefab)
    {
        // Assume the room prefab has a BoxCollider that defines its size
        BoxCollider collider = roomPrefab.GetComponent<BoxCollider>();
        int width = Mathf.CeilToInt(collider.size.x / gridSize);
        int height = Mathf.CeilToInt(collider.size.z / gridSize);
        return new Vector2Int(width, height);
    }

    bool CanPlaceRoom(Vector2Int position, Vector2Int size)
    {
        int startX = position.x - size.x / 2;
        int startY = position.y - size.y / 2;
        int endX = startX + size.x;
        int endY = startY + size.y;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight || grid[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    void PlaceRoom(Vector2Int position, Vector2Int size, GameObject roomPrefab)
    {
        int startX = position.x - size.x / 2;
        int startY = position.y - size.y / 2;
        int endX = startX + size.x;
        int endY = startY + size.y;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                grid[x, y] = true;
            }
        }

        Vector3 worldPosition = new Vector3(position.x * gridSize, 0, position.y * gridSize);
        GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        spawnedRooms.Add(newRoom);
    }

    void ConnectRooms()
    {
        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            GameObject roomA = spawnedRooms[i];
            Vector3 centerA = roomA.GetComponent<BoxCollider>().bounds.center;

            GameObject closestRoom = null;
            float closestDistance = float.MaxValue;

            for (int j = 0; j < spawnedRooms.Count; j++)
            {
                if (i == j) continue;

                GameObject roomB = spawnedRooms[j];
                Vector3 centerB = roomB.GetComponent<BoxCollider>().bounds.center;
                float distance = Vector3.Distance(centerA, centerB);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRoom = roomB;
                }
            }

            if (closestRoom != null)
            {
                Vector3 centerB = closestRoom.GetComponent<BoxCollider>().bounds.center;
                CreateHallwayPath(centerA, centerB);
            }
        }
    }

    void CreateHallwayPath(Vector3 positionA, Vector3 positionB)
    {
        Vector3 direction = (positionB - positionA).normalized;
        float distance = Vector3.Distance(positionA, positionB);
        int segmentCount = Mathf.CeilToInt(distance / gridSize);

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 segmentPosition = positionA + direction * gridSize * i;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Instantiate(hallwayPrefab, segmentPosition, rotation);
        }
    }

    public void ClearDungeon()
    {
        foreach (GameObject room in spawnedRooms)
        {
            Destroy(room);
        }
        spawnedRooms.Clear();
    }

    void OnDrawGizmos()
    {
        if (grid == null)
        {
            InitializeGrid();
        }

        // Draw grid squares
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellCenter = new Vector3(x * gridSize + gridSize / 2f, 0, y * gridSize + gridSize / 2f);
                Gizmos.color = grid[x, y] ? Color.red : Color.green;
                Gizmos.DrawWireCube(cellCenter, new Vector3(gridSize, 0.1f, gridSize));
            }
        }

        Gizmos.color = Color.blue;
        // Draw occupied cells
        if (spawnedRooms != null)
        {
            foreach (var room in spawnedRooms)
            {
                if (room != null)
                {
                    BoxCollider collider = room.GetComponent<BoxCollider>();
                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
                }
            }
        }
    }
}
