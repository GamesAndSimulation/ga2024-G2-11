using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public Tile.TileRotation[] tileOptions;

    public void CreateCell(bool collapsedState, Tile.TileRotation[] tiles)
    {
        collapsed = collapsedState;
        tileOptions = tiles;
    }

    public void RecreateCell(Tile.TileRotation[] tiles)
    {
        tileOptions = tiles;
    }
}