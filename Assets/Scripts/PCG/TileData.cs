using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData", order = 1)]
public class TileData : ScriptableObject
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
    
    public GameObject tilePrefab;

    [System.Serializable]
    public struct TileRotation
    {
        public TileData tile;
        public float zRotation;
        
        public TileRotation(TileData tile, float zRotation)
        {
            this.tile = tile;
            this.zRotation = zRotation;
        }
    }

    public SideType upType;
    public SideType rightType;
    public SideType downType;
    public SideType leftType;

    public SideType[] Sides;

    public TileRotation[] upNeighbours;
    public TileRotation[] rightNeighbours;
    public TileRotation[] downNeighbours;
    public TileRotation[] leftNeighbours;

    public void InitializeSides()
    {
        Sides = new SideType[4];
        Sides[0] = upType;
        Sides[1] = rightType;
        Sides[2] = downType;
        Sides[3] = leftType;
    }

    public void ComputeAllNeighbors(TileData[] availableTiles)
    {
        if (availableTiles == null || availableTiles.Length == 0)
        {
            Debug.LogError("AvailableTiles array is not set or empty.");
            return;
        }

        upNeighbours = ComputeNeighbours(SideOrientation.Up, upType, availableTiles);
        rightNeighbours = ComputeNeighbours(SideOrientation.Right, rightType, availableTiles);
        downNeighbours = ComputeNeighbours(SideOrientation.Down, downType, availableTiles);
        leftNeighbours = ComputeNeighbours(SideOrientation.Left, leftType, availableTiles);
    }

    private TileRotation[] ComputeNeighbours(SideOrientation orientation, SideType type, TileData[] availableTiles)
    {
        List<TileRotation> neighbours = new List<TileRotation>();
        foreach (TileData t in availableTiles)
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
        Vector3 orientationNormal = GetOrientationNormal(orientation);
        Vector3 targetNormal = Orientations[direction];

        // Calculate the signed angle between the orientationNormal and targetNormal
        float angle = Vector3.SignedAngle(orientationNormal, targetNormal, Vector3.up);
    
        // Ensure the angle is positive
        if (angle < 0)
        {
            angle += 360;
        }

        Debug.Log($"Orientation: {orientation}, Direction: {direction}, Angle: {angle}");
        return angle;
    }

    private Vector3 GetOrientationNormal(SideOrientation orientation)
    {
        switch (orientation)
        {
            case SideOrientation.Up:
                return Vector3.back;
            case SideOrientation.Right:
                return Vector3.right;
            case SideOrientation.Down:
                return Vector3.forward;
            case SideOrientation.Left:
                return Vector3.left;
            default:
                return Vector3.forward; // Should never happen
        }
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
}
