using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Island 
{
    [SerializeField]
    public string m_IslandName = string.Empty;
    [SerializeField]
    public List<HexPoint> m_HexPoints = new List<HexPoint>();
}
