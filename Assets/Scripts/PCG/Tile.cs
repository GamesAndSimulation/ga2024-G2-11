using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public TileData tileData;

    private void Awake()
    {
        InitializeTile();
    }

    public void InitializeTile()
    {
        tileData.InitializeSides();
        tileData.ComputeAllNeighbors(FindObjectOfType<WaveFunction>().AvailableTiles);
    }

    public TileData.TileRotation[] GetUpNeighbours()
    {
        return tileData.upNeighbours;
    }
    
    public TileData.TileRotation[] GetRightNeighbours()
    {
        return tileData.rightNeighbours;
    }
    
    public TileData.TileRotation[] GetDownNeighbours()
    {
        return tileData.downNeighbours;
    }
    
    public TileData.TileRotation[] GetLeftNeighbours()
    {
        return tileData.leftNeighbours;
    }
}