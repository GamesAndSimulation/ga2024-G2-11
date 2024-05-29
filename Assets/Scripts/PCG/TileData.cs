using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData", order = 1)]
public class TileData : ScriptableObject
{
    
    public enum SideType {
        RLS,
        LLS,
        HF,
        LF
    }
    
    public enum SideOrientation {
        posX,
        negZ,
        negX,
        posZ
    }

    //public static readonly Vector3[] Orientations = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    

    public SideType posXType;
    public SideType negZType;
    public SideType negXType;
    public SideType posZType;

    public GameObject tilePrefab;

    public int numRotations;
    
    public SideType[] Sockets;

    public TilePrototype[] Prototypes;

    public void ComputePrototypes()
    {
        Prototypes = new TilePrototype[numRotations + 1];
        Sockets = new SideType[] {posXType, negZType, negXType, posZType};
        
        for(int i = 0; i <= numRotations; i++)
        {
            TilePrototype prototype = new TilePrototype
            {
                Sockets = new[] {
                    Sockets[Mod(((int)SideOrientation.posX - i) ,Sockets.Length)],
                    Sockets[Mod(((int)SideOrientation.negZ - i) ,Sockets.Length)],
                    Sockets[Mod(((int)SideOrientation.negX - i) ,Sockets.Length)],
                    Sockets[Mod(((int)SideOrientation.posZ - i) ,Sockets.Length)]},
                Rotation = i * 90,
                TilePrefab = tilePrefab
            };
            Prototypes[i] = prototype;
            Debug.LogWarning(Prototypes[i].TileToString());
        }
    }

    public void ComputePrototypesNeighbors()
    {
        foreach(TilePrototype prototype in Prototypes)
        {
            prototype.Neighbors = new List<TilePrototype>[4]; // posX, negZ, negX, posZ
            for(int i = 0; i < 4; i++)
            {
                prototype.Neighbors[i] = new List<TilePrototype>();
                foreach(TilePrototype neighbor in Prototypes)
                {
                    if(prototype.Sockets[i].ToString().EndsWith("S")) // Asymmetric
                    {
                        if(prototype.Sockets[i] == SideType.LLS && neighbor.Sockets[(i + 2) % 4] == SideType.RLS
                           || prototype.Sockets[i] == SideType.RLS && neighbor.Sockets[(i + 2) % 4] == SideType.LLS)
                        {
                            prototype.Neighbors[i].Add(neighbor);
                        }
                        
                    }
                    else if(prototype.Sockets[i] == neighbor.Sockets[(i + 2) % 4])
                    {
                        prototype.Neighbors[i].Add(neighbor);
                    }
                    
                }
            }
        }
    }
    
    public void PrintPrototypesNeighbors()
    {
        foreach (TilePrototype prototype in Prototypes)
        {
            Debug.Log("Neighbors for " + prototype.TileToString());
            for (int i = 0; i < 4; i++)
            {
                Debug.Log("Socket " + i + ": ");
                foreach (TilePrototype neighbor in prototype.Neighbors[i])
                {
                    Debug.Log(neighbor.TileToString());
                }
            }
        }
    }
    
    public void PrintPrototypes()
    {
        foreach (TilePrototype prototype in Prototypes)
        {
            Debug.Log(prototype.TileToString());
        }
    }
    
    private TileData[] FindAllTileDataAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:TileData");
        TileData[] tiles = new TileData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            tiles[i] = AssetDatabase.LoadAssetAtPath<TileData>(path);
        }
        return tiles;
    }
    
    int Mod(int a, int b)
    {
        int result = a % b;
        if (result < 0)
        {
            result += b;
        }
        return result;
    }
}
