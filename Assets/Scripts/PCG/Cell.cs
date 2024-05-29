using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public List<TilePrototype> tileOptions = new List<TilePrototype>(); // Initialize the list
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
}