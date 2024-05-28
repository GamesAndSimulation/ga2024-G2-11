using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public TileData.TileRotation[] tileOptions;

    public void CreateCell(bool collapsedState, TileData.TileRotation[] tiles)
    {
        collapsed = collapsedState;
        tileOptions = tiles;
    }

    public void RecreateCell(TileData.TileRotation[] tiles)
    {
        tileOptions = tiles;
    }
}