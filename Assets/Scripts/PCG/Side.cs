using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #######################
/// # SideType definition #
/// #     RL : /‾         #
/// #     LL : ‾\         #
/// #     HF : ‾          #
/// #     LF : _          #
/// #######################
///
/// We define a side's type by it's shape when looking at that side "eye to eye".
/// For two sides to be compatible, they need be one of the following pairs:
/// - RL <-> LL
/// - HF <-> HF
/// - LF <-> LF
/// </summary>


public class Side : MonoBehaviour
{
    [System.Serializable]
    public struct SideTypeOrientation 
    {
        public SideType type;
        public SideOrientation orientation;
        public Tile.TileRotation[] tileOptions;
    }
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
    
}
