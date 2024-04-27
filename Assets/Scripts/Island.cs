using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Hex;

[System.Serializable]
public class Island
{
    [SerializeField]
    public string m_IslandName = string.Empty;
    //[SerializeField]
    //public List<HexPoint> m_HexPoints = new List<HexPoint>();
    [SerializeField]
    public List<Hex.HexIndex> m_HexPointIndeces = new List<Hex.HexIndex>();

    [SerializeField]
    public GameObject m_HexPointTitle = null;
    [SerializeField]
    public static GameObject m_HexPointTitlePrefab = null;
    const string HEX_POINT_TITLE_PREFAB = "HexPointTitle";

    HexVisibility m_IslandVisibility = HexVisibility.Unknown;
    public HexVisibility IslandVisibility
    {
        get
        {
            return m_IslandVisibility;
        }
    }


    [SerializeField]
    public int m_IslandIndex = -1;

    public void SetIslandName(string islandName)
    {
        m_IslandName = islandName;
    }
    public void SetTitle(string title, Hex hex, HexPoint hexPoint)
    {
        if (m_HexPointTitle != null)
        {
            GameObject.Destroy(m_HexPointTitle);
        }
        if (m_HexPointTitlePrefab == null)
        {
            m_HexPointTitlePrefab = Resources.Load(HEX_POINT_TITLE_PREFAB) as GameObject;
        }
        m_HexPointTitle = GameObject.Instantiate(m_HexPointTitlePrefab, hex.transform);
        TextMeshPro tmp = m_HexPointTitle.GetComponent<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogError("Could not get title");
        }
        else
        {
            tmp.text = title;
            m_HexPointTitle.transform.position = new Vector3(hexPoint.position.x, hexPoint.position.y, m_HexPointTitle.transform.position.z);
        }
        SetIslandVisibility(hex.m_HexVisibility);
    }

    public void SetIslandVisibility(HexVisibility vis)
    {
        //if (m_IslandVisibility != vis)
        {
            if (m_HexPointTitle != null)
            {
                m_IslandVisibility = vis;
                switch (vis)
                {
                    case HexVisibility.Unknown:
                        m_HexPointTitle.gameObject.layer = Hex.UnknownLayer;
                        break;
                    case HexVisibility.Known:
                        m_HexPointTitle.gameObject.layer = Hex.KnownLayer;
                        break;
                    case HexVisibility.Discovered:
                        m_HexPointTitle.gameObject.layer = Hex.DiscoveredLayer;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
