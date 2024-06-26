using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 
///    Hex points
///      1-------3
///     /         \
///    0     2     4
///     \         /
///      5-------6
///         
/// 
///    Hex Edges
///    
///          1
///      /-------\
///   0 /         \ 2
///    <           >
///   5 \         / 3
///      \-------/
///          4
///         
/// </summary>
public class Hex : MonoBehaviour
{
    [System.Serializable]
    public enum HexVisibility { Undefined, Unknown, Known, Discovered };

    public enum HexSubType { Undefined, Home, Waystation, Hazard };
    Color m_HexSubTypeColorHome = Color.gray;
    Color m_HexSubTypeColorWaystation = Color.green;
    Color m_HexSubTypeColorHazard = Color.red;
    [SerializeField]
    private HexSubType m_HexSubType;
    //public HexSubType[] m_HexSubTypes = new HexSubType[7]
    //{
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //    HexSubType.Undefined,
    //};

    [System.Serializable]
    public class HexIndex
    {
        public int ix;
        public int iy;
        public int hexPointMask;
        public HexIndex()
        {
            ix = 0;
            iy = 0;
            hexPointMask = 0;
        }
        public HexIndex(int x, int y)
        {
            ix = x;
            iy = y;
            hexPointMask = 0;
        }
    };
    public class HexIndexPair
    {
        public int i0;
        public int i1;
        public HexIndexPair(int p0, int p1)
        {
            i0 = p0;
            i1 = p1;
        }
    }
    public HexIndexPair[] m_HexEdges = new HexIndexPair[]
    {
        new HexIndexPair(0, 1),
        new HexIndexPair(1, 3),
        new HexIndexPair(3, 4),
        new HexIndexPair(4, 6),
        new HexIndexPair(6, 5),
        new HexIndexPair(5, 0),
    };

    public class HexPointNeighborIndex
    {
        public int neighborA;
        public int nApt0;
        public int nApt1;
        public int nApt2;
        public int neighborB;
        public int nBpt0;
        public int nBpt1;
        public HexPointNeighborIndex(int nA, int nA0, int nA1, int nA2, int nB, int nB0, int nB1)
        {
            neighborA = nA;
            nApt0 = nA0;
            nApt1 = nA1;
            nApt2 = nA2;
            neighborB = nB;
            nBpt0 = nB0;
            nBpt1 = nB1;
        }
    }
    public HexPointNeighborIndex[] m_HexPointNeighbors = new HexPointNeighborIndex[]
    {
        new HexPointNeighborIndex(0, 1, 2, 4, 5, 2, 4),
        new HexPointNeighborIndex(1, 6, 2, 3, 1, 2, 6),
        new HexPointNeighborIndex(-1, 0, 1, 3, -1, 4, 5),
        new HexPointNeighborIndex(1, 5, 2, 4, 2, 2, 4),
        new HexPointNeighborIndex(2, 0, 2, 6, 3, 0, 2),
        new HexPointNeighborIndex(5, 2, 3, 6, 4, 2, 3),
        new HexPointNeighborIndex(3, 1, 2, 5, 4, 1, 2),
    };

    public HexVisibility m_HexVisibility = HexVisibility.Undefined;
    public Hex[] m_Neighbor = new Hex[7] { null, null, null, null, null, null, null };
    //public Hex[] m_SeaNeighbor = new Hex[7] { null, null, null, null, null, null, null };
    public int[] m_TransitionCost = new int[] { 0, 0, 0, 0, 0, };

    public HexIndex m_ThisHexIndex;
    public HexIndex[] m_HexPointIndeces = new HexIndex[7];
    public Renderer m_Renderer;
    public MaterialPropertyBlock m_Block;

    public Vector4 m_TextureST;
    public Color m_Color = Color.white;
    public Color m_BackgroundColor = Color.white;
    public Vector4 m_BlockTextureST;
    public Color m_BlockColor = Color.white;
    public Color m_BlockBackgroundColor = Color.white;

    static int m_UnknownLayer = 0;
    public static int UnknownLayer
    {
        get
        {
            return m_UnknownLayer;
        }
    }
    static int m_KnownLayer = 0;
    public static int KnownLayer
    {
        get
        {
            return m_KnownLayer;
        }
    }
    static int m_DiscoveredLayer = 0;
    public static int DiscoveredLayer
    {
        get
        {
            return m_DiscoveredLayer;
        }
    }

    int m_HexVertexIndex = -1;

    string m_HexName = string.Empty;
    bool m_IsTitleHex = false;

    //Island m_Island = null;

    //public void SetIsland(Island island)
    //{
    //    m_Island = island;
    //}

    public string[] GetIslandNames()
    {
        List<string> islandNames = new List<string>();
        for (int i = 0; i < m_HexPointIndeces.Length; i++)
        {
            HexIndex hi = m_HexPointIndeces[i];
            Island island = HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].m_Island;
            if (island != null)
            {
                if (!islandNames.Contains(island.m_IslandName))
                {
                    islandNames.Add(island.m_IslandName);
                }
            }
        }
        return islandNames.ToArray();
    }

    public void SetHexPointName(HexPoint hexPoint, string islandName, bool isTitleHex)
    {
        m_HexName = islandName;
        if (hexPoint.m_Island != null)
        {
            HexIndex myHi = this.m_ThisHexIndex;
            hexPoint.m_Island.SetIslandVisibility(m_HexVisibility);
            if (isTitleHex)
            {
                hexPoint.m_Island.SetTitle(islandName, HexMapBuilder.Instance.Hexes[myHi.iy][myHi.ix], hexPoint);
            }
            HexIndexPair islandHip = hexPoint.indeces;
            hexPoint.m_Island.SetIslandName(islandName);
            Debug.Log($"SetHexPointName hp {islandHip.i0.ToString()},{islandHip.i1.ToString()} hex {myHi.ix.ToString()},{myHi.iy.ToString()} island idx {hexPoint.m_Island.m_IslandIndex.ToString()}, name {islandName}");
        }
    }

    public int GetHexVertexIndex()
    {
        //if (m_HexVertexIndex == -1)
        {
            m_HexVertexIndex = 0;
            for (int i = 0; i < m_HexPointIndeces.Length; i++)
            {
                int ix = m_HexPointIndeces[i].ix;
                int iy = m_HexPointIndeces[i].iy;
                if (HexMapBuilder.Instance.HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
                {
                    m_HexVertexIndex = m_HexVertexIndex | (0x01 << i);
                }
            }
        }

        return m_HexVertexIndex;
    }

    public void SetHexColor(Color foreground, Color background)
    {
        if (m_HexSubType == HexSubType.Undefined)
        {
            m_Color = foreground;
        }
        m_BackgroundColor = background;
    }

    public Material m_HexMaterial;
    public Material HexMaterial
    {
        get 
        { 
            return m_HexMaterial; 
        }
        set 
        { 
            m_HexMaterial = value; 
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_UnknownLayer = LayerMask.NameToLayer(Hex.HexVisibility.Unknown.ToString());
        m_KnownLayer = LayerMask.NameToLayer(Hex.HexVisibility.Known.ToString());
        m_DiscoveredLayer = LayerMask.NameToLayer(Hex.HexVisibility.Discovered.ToString());

        m_Neighbor[6] = this;
    }

    public HexSubType GetHexSubType()
    {
        return (m_HexSubType);
    }
    public void SetHexVisibility(HexVisibility vis)
    {
        if (m_HexVisibility != vis)
        {
            m_HexVisibility = vis;
            switch (vis)
            {
                case HexVisibility.Unknown:
                    gameObject.layer = m_UnknownLayer;
                    break;
                case HexVisibility.Known:
                    gameObject.layer = m_KnownLayer;
                    break;
                case HexVisibility.Discovered:
                    gameObject.layer = m_DiscoveredLayer;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < m_HexPointIndeces.Length; i++)
            {
                HexIndex hi = m_HexPointIndeces[i];
                HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].pointVisibility = vis;
                Island island = HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].m_Island;
                if (island != null)
                {
                    island.SetIslandVisibility(vis);
                }
            }
            //Debug.Log($"layer in {vis.ToString()} obj layer {gameObject.layer.ToString()}");
        }
    }

    public bool HexHasLand()
    {
        for (int i = 0; i < m_HexPointIndeces.Length; i++)
        {
            HexIndex hi = m_HexPointIndeces[i];
            if (hi != null)
            {
                if (HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexPointType == HexPoint.HexPointType.Land)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void SetHexIndeces(HexIndex[] hexIndecies)
    {
        for (int i = 0; i < hexIndecies.Length; i++)
        {
            m_HexPointIndeces[i] = hexIndecies[i];
        }
    }
    public void SetNeighbors(Hex[] neighbors)
    {
        if ((neighbors == null) || (neighbors.Length != 6))
        {
            Debug.LogError("Bad neighbors list");
        }
        for (int i = 0; i < neighbors.Length; i++)
        {
            m_Neighbor[i] = neighbors[i];
        }
    }
    //public void SetSeaNeighbors(Hex[] neighbors)
    //{
    //    if ((neighbors == null) || (neighbors.Length != 6))
    //    {
    //        Debug.LogError("Bad neighbors list");
    //    }
    //    for (int i = 0; i < neighbors.Length; i++)
    //    {
    //        m_SeaNeighbor[i] = neighbors[i];
    //    }
    //}

    public Vector3 GetHexPosition()
    {
        return transform.position;
    }

    void Update()
    {
        if ((m_BlockColor != m_Color)
            || (m_BlockBackgroundColor != m_BackgroundColor)
            || (m_BlockTextureST != m_TextureST)
            )
        { 
            m_Block = new MaterialPropertyBlock();
            m_Block.SetVector("_MainTex_ST", m_TextureST);
            m_Block.SetColor("_Color", m_Color);
            m_Block.SetColor("_BackgroundColor", m_BackgroundColor);
            m_Renderer.SetPropertyBlock(m_Block);
            m_BlockColor = m_Color;
            m_BlockBackgroundColor = m_BackgroundColor;
            m_BlockTextureST = m_TextureST;
        }
    }

    public HexPoint.HexPointType GetHexCenterPointType()
    {
        HexIndex hi = m_HexPointIndeces[HexMapBuilder.CENTER_INDEX];
        return (HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexPointType);
    }

    //public HexSubType[] GetHexSubTypes()
    //{
    //    HexSubType[] hexSubTypes = new HexSubType[m_HexPointIndeces.Length];
    //    for (int i = 0; i < m_HexPointIndeces.Length; i++)
    //    {
    //        HexIndex hi = m_HexPointIndeces[i];
    //        if (hi != null)
    //        {
    //            hexSubTypes[i] = HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexSubType;
    //        }
    //        else
    //        {
    //            hexSubTypes[i] = HexSubType.Undefined;
    //        }
    //    }
    //    return hexSubTypes;
    //}
    public void SetHexSubType(HexSubType hst)
    {
        HexIndex hi = m_ThisHexIndex;

        if ((hi.iy == 48) && (hi.ix == 50))
        {
            Debug.LogError($"SetHexSubType change HOME to {hst.ToString()}");
        }
        if (hst == HexSubType.Home)
        {
            Debug.LogError($"SetHexSubType change to HOME for Hex_{hi.iy.ToString()}_{hi.ix.ToString()}");
        }

        //if (hst != HexMapBuilder.Instance.Hexes[hi.iy][hi.ix].GetHexSubType())
        if (hst != m_HexSubType)
        {
            m_HexSubType = hst;
            switch (hst)
            {
                case HexSubType.Undefined:
                    break;
                case HexSubType.Home:
                    m_Color = m_HexSubTypeColorHome;
                    break;
                case HexSubType.Waystation:
                    m_Color = m_HexSubTypeColorWaystation;
                    break;
                case HexSubType.Hazard:
                    m_Color = m_HexSubTypeColorHazard;
                    break;
                default:
                    break;
            }
        }

        //if (hst != HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexSubType)
        //{
        //    if (HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexSubType == HexSubType.Home)
        //    {
        //        Debug.LogError("About to change HOME");
        //    }


        //    HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix].hexSubType = hst;
        //    switch (hst)
        //    {
        //        case HexSubType.Undefined:
        //            break;
        //        case HexSubType.Home:
        //            m_Color = m_HexSubTypeColorHome;
        //            break;
        //        case HexSubType.Waystation:
        //            m_Color = m_HexSubTypeColorWaystation;
        //            break;
        //        case HexSubType.Hazard:
        //            m_Color = m_HexSubTypeColorHazard;
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
