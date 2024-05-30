using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public List<TilePrototype> tileOptions = new List<TilePrototype>(); // Initialize the list
    public GameObject instantiatedTile;
    public Vector2Int gridCoordinates;

    public void CreateCell(bool collapsedState, List<TilePrototype> tiles, int x, int y)
    {
        collapsed = collapsedState;
        tileOptions.AddRange(tiles);
        gridCoordinates = new Vector2Int(x, y);
    }

    public void RecreateCell(List<TilePrototype> tiles)
    {
        tileOptions = tiles;
    }

    public int GetEntropy()
    {
        return collapsed ? 0 : tileOptions.Count;
    }
    
    public void ReplacePrefabWithDoor()
    {
        if(tileOptions[0].TilePrefabWithDoor == null)
        {
            return;
        }
        
        if (instantiatedTile != null)
        {
            Destroy(instantiatedTile);
        }
        instantiatedTile = Instantiate(tileOptions[0].TilePrefabWithDoor, transform.position, Quaternion.Euler(-90, 0, tileOptions[0].Rotation));
        instantiatedTile.transform.parent = transform;
        tileOptions[0].TilePrefabShape = TileData.TileShape.Door;
    }
}