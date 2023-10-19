using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPointInfo : MonoBehaviour
{
    public int m_ix = -1;
    public int m_iy = -1;
    public int m_iyShown = -1;
    public int m_ixShown = -1;

    public Vector3 position;
    public HexPoint.HexPointType hexPointType;
    public GameObject go;

    public Hex.HexVisibility pointVisibility;

    void Update()
    {
        if ((m_ix != m_ixShown) || (m_iy != m_iyShown))
        {
            m_ixShown = m_ix;
            m_iyShown = m_iy;
            if ((m_iy >= 0)
                && (m_iy < HexMapBuilder.Instance.HexPoints.Length)
                && (m_ix >= 0)
                && (m_ix < HexMapBuilder.Instance.HexPoints[m_iy].Length)
                )
            {
                HexPoint hp = HexMapBuilder.Instance.HexPoints[m_iyShown][m_ixShown];
                position = hp.position;
                hexPointType = hp.hexPointType;
                go = hp.go;
                pointVisibility = hp.pointVisibility;
            }
            else
            {
                position = Vector3.one * -1.0f;
                hexPointType = HexPoint.HexPointType.Undef;
                go = null;
                pointVisibility = Hex.HexVisibility.Undefined;
            }
        }
    }
}
