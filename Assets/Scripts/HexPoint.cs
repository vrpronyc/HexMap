using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexPoint
{
    [System.Serializable]
    public enum HexPointType { Land, Sea, Undef };

    public Hex.HexSubType hexSubType;
    public Vector3 position;
    public HexPointType hexPointType;
    public GameObject go;
    public Hex.HexIndexPair indeces;

    public Hex.HexVisibility pointVisibility;

    public Island m_Island = null;

    // HEX_POINT_NEIGHBOR_DELTA[0] is the 6 index offsets for HexPoints in even columns.
    // HEX_POINT_NEIGHBOR_DELTA[1] is the 6 index offsets for HexPoints in odd columns.
    public static Hex.HexIndexPair[][] HEX_POINT_NEIGHBOR_DELTA = new Hex.HexIndexPair[2][]
        {
                new Hex.HexIndexPair[6]
                {
                    new Hex.HexIndexPair(-2, 0),
                    new Hex.HexIndexPair(-1, 0),
                    new Hex.HexIndexPair(1, 0),
                    new Hex.HexIndexPair(2, 0),
                    new Hex.HexIndexPair(-1, 1),
                    new Hex.HexIndexPair(1, 1),
                },
                new Hex.HexIndexPair[6]
                {
                    new Hex.HexIndexPair(-2, 0),
                    new Hex.HexIndexPair(-1, -1),
                    new Hex.HexIndexPair(1, -1),
                    new Hex.HexIndexPair(2, 0),
                    new Hex.HexIndexPair(-1, 0),
                    new Hex.HexIndexPair(1, 0),
                },
        };

    public HexPoint()
    {
        hexSubType = Hex.HexSubType.Undefined;
        position = Vector3.zero;
        hexPointType = HexPointType.Undef;
        go = null;
        pointVisibility = Hex.HexVisibility.Undefined;
        indeces = new Hex.HexIndexPair(-1, -1);
    }
}
