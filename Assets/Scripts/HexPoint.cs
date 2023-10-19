using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexPoint 
{
    [System.Serializable]
    public enum HexPointType { Land, Sea, Undef };
    public Vector3 position;
    public HexPointType hexPointType;
    public GameObject go;

    public Hex.HexVisibility pointVisibility;
    public HexPoint()
    {
        position = Vector3.zero;
        hexPointType = HexPointType.Undef;
        go = null;
        pointVisibility = Hex.HexVisibility.Undefined;
    }
}
