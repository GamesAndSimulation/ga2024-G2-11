using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilePrototype 
{
    public TileData.SideType[] Sockets;
    public int Rotation;
    public TileData.TileShape TilePrefabShape;
    public GameObject TilePrefab;
    public GameObject TilePrefabWithDoor; // Only used for corner in and wall
    public List<TilePrototype>[] Neighbors; // Neighbors for each socket
    
    public string TileToString()
    {
        return "TilePrototype: " + Sockets[0] + ", " + Sockets[1] + ", " + Sockets[2] + ", " + Sockets[3] + ", " + Rotation;
    }
}