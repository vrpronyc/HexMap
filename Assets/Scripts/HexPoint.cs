using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPoint 
{
    public enum HexPointType { Land, Sea, Undef };
    public Vector3 position;
    public HexPointType hexPointType;
    public GameObject go;
    public HexPoint()
    {
        position = Vector3.zero;
        hexPointType = HexPointType.Undef;
        go = null;
    }
}
