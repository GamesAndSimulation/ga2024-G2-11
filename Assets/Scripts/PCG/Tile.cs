using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    [System.Serializable]
    public struct TileRotation
    {
        public Tile tile;
        public float zRotation;
        
        public TileRotation(Tile tile, float zRotation)
        {
            this.tile = tile;
            this.zRotation = zRotation;
        }
    }

    public TileRotation[] upNeighbours;
    public TileRotation[] rightNeighbours;
    public TileRotation[] downNeighbours;
    public TileRotation[] leftNeighbours;
}
