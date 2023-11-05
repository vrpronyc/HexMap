using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexArrayAccessor 
{
    public Hex this[int ix, int iy, int iz]
    {
        get
        {
            return HexMapBuilder.Instance.Hexes[ix][iy];
        }
    }
}
