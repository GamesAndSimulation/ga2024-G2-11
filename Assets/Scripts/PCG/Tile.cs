using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public enum SideType {
        RL,
        LL,
        HF,
        LF
    }
    
    public enum SideOrientation {
        Up,
        Right,
        Down,
        Left
    }

    public static readonly Vector3[] Orientations = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

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

    public SideType upType;
    public SideType rightType;
    public SideType downType;
    public SideType leftType;

    public SideType[] Sides { get; private set; }
        
    public TileRotation[] upNeighbours;
    public TileRotation[] rightNeighbours;
    public TileRotation[] downNeighbours;
    public TileRotation[] leftNeighbours;

    public Tile[] AvailableTiles;

    private void Awake()
    {
        InitializeSides();
        ComputeAllNeighbors();
    }

    public void InitializeSides()
    {
        Sides = new SideType[4];
        Sides[0] = upType;
        Sides[1] = rightType;
        Sides[2] = downType;
        Sides[3] = leftType;
        Debug.Log("Sides initialized.");
    }

    public void ComputeAllNeighbors()
    {
        AvailableTiles = GameObject.FindWithTag("WaveFunction").GetComponent<WaveFunction>().AvailableTiles;
        if (AvailableTiles == null || AvailableTiles.Length == 0)
        {
            Debug.LogError("AvailableTiles array is not set or empty.");
            return;
        }

        upNeighbours = ComputeNeighbours(SideOrientation.Up, upType);
        rightNeighbours = ComputeNeighbours(SideOrientation.Right, rightType);
        downNeighbours = ComputeNeighbours(SideOrientation.Down, downType);
        leftNeighbours = ComputeNeighbours(SideOrientation.Left, leftType);

        Debug.Log("Neighbors computed.");
    }

    private TileRotation[] ComputeNeighbours(SideOrientation orientation, SideType type)
    {
        List<TileRotation> neighbours = new List<TileRotation>();
        foreach (Tile t in AvailableTiles)
        {
            if (t.Sides == null || t.Sides.Length == 0)
            {
                Debug.LogError($"Tile {t.name} has not been initialized properly.");
                continue;
            }

            for (int direction = 0; direction < t.Sides.Length; direction++)
            {
                if (t.Sides[direction] == getCompatibleSide(type))
                {
                    neighbours.Add(new TileRotation(t, getAngle(orientation, direction)));
                }
            }
        }

        if (neighbours.Count == 0)
        {
            Debug.LogWarning($"No neighbors found for {orientation} with side type {type}.");
        }

        return neighbours.ToArray();
    }

    private float getAngle(SideOrientation orientation, int direction)
    {
        Vector3 orientationNormal;
        switch (orientation)
        {
            case SideOrientation.Up:
                orientationNormal = Vector3.forward;
                break;
            case SideOrientation.Right:
                orientationNormal = Vector3.right;
                break;
            case SideOrientation.Down:
                orientationNormal = Vector3.back;
                break;
            case SideOrientation.Left:
                orientationNormal = Vector3.left;
                break;
            default:
                orientationNormal = Vector3.up; // Should never happen
                break;
        }
        Quaternion rotation = Quaternion.FromToRotation(orientationNormal, Orientations[direction]);
        return rotation.eulerAngles.z;
    }

    private SideType getCompatibleSide(SideType type)
    {
        switch (type)
        {
            case SideType.HF:
                return SideType.HF;
            case SideType.LF:
                return SideType.LF;
            case SideType.LL:
                return SideType.RL;
            case SideType.RL:
                return SideType.LL;
            default:
                Debug.LogError("Unknown SideType.");
                return SideType.HF; // Should never happen
        }
    }

    public TileRotation[] GetUpNeighbours()
    {
        return upNeighbours;
    }
    
    public TileRotation[] GetRightNeighbours()
    {
        return rightNeighbours;
    }
    
    public TileRotation[] GetDownNeighbours()
    {
        return downNeighbours;
    }
    
    public TileRotation[] GetLeftNeighbours()
    {
        return leftNeighbours;
    }
}
