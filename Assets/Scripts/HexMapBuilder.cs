using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using UnityEngine;

public class HexMapBuilder : MonoBehaviour
{
    static HexMapBuilder m_instance;
    public static HexMapBuilder Instance
    {
        get
        {
            return m_instance;
        }
    }

    [System.Serializable]
    public class HexBuildPass
    {
        public float m_HexLandProbability = 0.01f;
        public float m_HexEdge0NeighborsProbability = 0.0f;
        public float m_HexEdge1NeighborsProbability = 0.0f;
        public float m_HexEdge2NeighborsProbability = 0.0f;
        public float m_HexEdge3NeighborsProbability = 0.0f;
        public float m_HexEdge4NeighborsProbability = 0.0f;
        public float m_HexEdge5NeighborsProbability = 0.0f;
        public float m_HexEdge6NeighborsProbability = 0.0f;
        public HexBuildPass()
        {
            m_HexLandProbability = 0.01f;
            m_HexEdge0NeighborsProbability = 0.0f;
            m_HexEdge1NeighborsProbability = 0.0f;
            m_HexEdge2NeighborsProbability = 0.0f;
            m_HexEdge3NeighborsProbability = 0.0f;
            m_HexEdge4NeighborsProbability = 0.0f;
            m_HexEdge5NeighborsProbability = 0.0f;
            m_HexEdge6NeighborsProbability = 0.0f;
        }
    }

    public List<HexBuildPass> m_HexBuildPasses;
    public float m_HexWaystationProbability = 0.01f;
    public float m_HexHazardProbability = 0.01f;

    public string m_HexMapResourcePath = "HexMap_";
    public Texture2D m_SourceTexture;
    public Color m_DefaultColor = Color.white;
    public Color m_DefaultDiscoveredColor = Color.Lerp(Color.grey, Color.white, 0.5f);
    public Color m_SeaColor = Color.blue;
    public Color m_SeaDiscoveredColor = Color.Lerp(Color.grey, Color.blue, 0.5f);
    public Material m_SeaMtl;
    public Material m_LandMtl;
    public Material m_HexMtl;

    Hex[][] m_Hexes;
    public Hex[][] Hexes
    {
        get
        {
            return m_Hexes;
        }
    }

    HexPoint[][] m_HexPoints;
    public HexPoint[][] HexPoints
    {
        get
        {
            return m_HexPoints;
        }
    }

    Vector3 m_HexMapMin;
    Vector3 m_HexMapMax;
    Vector3 m_HexMapZero;

    public List<Island> m_Islands = new List<Island>();

    //HexPointType[][] m_HexPointType;

    /// <summary>
    ///  Rows will oscilate:
    ///            1------3    5     7------9
    /// Row A     / Hex A0 \        / Hex A2 \
    ///          0    2     4------6    8     10 
    ///           \        / Hex A1 \        /
    ///            1------3    5     7------9
    /// Row B     / Hex B0 \        / Hex B2 \
    ///          0    2     4------6    8     10 
    ///           \        / Hex B1 \        /
    ///            1------3    5     7------9
    /// Row C     /        \        /        \
    ///          0    2     4------6    8     10 
    ///
    ///
    ///               1-----2
    /// Hex Point    /       \ 
    /// Order       0    6    3
    ///              \       / 
    ///               4-----5 
    ///
    /// 
    ///                 m_Scale
    ///               |---------|
    ///                 1-----2   ---
    /// Dimensions     /       \   |
    ///               0    6    3  | Cos(30) * m_Scale
    ///                \       /   |
    ///                 4-----5   ---
    ///               |-|-------|-|
    ///              Rise   |   Drop
    ///                    Run 
    ///                    
    ///              Rise, Drop = m_Scale * 0.25f ;
    ///              Run = m_Scale * 0.5f ;
    /// </summary>
    public const int CENTER_INDEX = 2;

    public int m_Width = 12;
    public int m_Height = 10;
    public int m_WidthCount;
    public int m_HeightCount;

    public float m_Scale = 1.0f;

    public Transform m_HexTexQuadTransform;
    public string m_TexturePropertyName = "_DecalTex";

    const float RISE_DROP = 0.25f;
    const float RUN = 0.5f;

    Mesh m_HexMesh = null;

    Dictionary<string, Texture2D> m_HexTextures = new Dictionary<string, Texture2D>();

    public Transform m_TraceBox;
    public Transform m_TraceSphere;

    public Transform m_NullHexPicked;
    public Transform m_HexPicked;

    public float m_HexNeighborDwellTime = 0.1f;

    public Hex m_Home;
    public NavigationController.HexTri m_HomeTri;
    public Hex.HexIndex m_HomeIndex;

    public Transform m_PointParent;

    public bool m_DBG = false;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    static string Bits(int i)
    {
        StringBuilder sb = new StringBuilder();

        for (int j = 0; j < 7; j++)
        {
            int bit = 0x40 >> j;
            if ((i & bit) == bit)
            {
                sb.Append("1");
            }
            else
            {
                sb.Append("0");
            }
        }
        return sb.ToString();
    }

    int CountNeighbors(int ix, int iy, int iPt)
    {
        int count = 0;
        // 
        Hex hex = m_Hexes[iy][ix];
        Hex.HexPointNeighborIndex hni = hex.m_HexPointNeighbors[iPt];
        Hex hexA = hex.m_Neighbor[hni.neighborA];
        if (hexA != null)
        {
            Hex.HexIndex hiA = hexA.m_HexPointIndeces[hni.nApt0];
            if (m_HexPoints[hiA.iy][hiA.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                count++;
            }
            hiA = hexA.m_HexPointIndeces[hni.nApt1];
            if (m_HexPoints[hiA.iy][hiA.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                count++;
            }
            hiA = hexA.m_HexPointIndeces[hni.nApt2];
            if (m_HexPoints[hiA.iy][hiA.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                count++;
            }
        }
        Hex hexB = hex.m_Neighbor[hni.neighborB];
        if (hexB != null)
        {
            Hex.HexIndex hiB = hexB.m_HexPointIndeces[hni.nBpt0];
            if (m_HexPoints[hiB.iy][hiB.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                count++;
            }
            hiB = hexB.m_HexPointIndeces[hni.nBpt1];
            if (m_HexPoints[hiB.iy][hiB.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                count++;
            }
        }
        Hex.HexIndex hi = hex.m_HexPointIndeces[CENTER_INDEX];
        if (m_HexPoints[hi.iy][hi.ix].hexPointType == HexPoint.HexPointType.Land)
        {
            count++;
        }

        return count;
    }

    void GenerateHome(int ix)
    {
        Hex.HexIndex homeIndex = new Hex.HexIndex(ix, m_Height - 1);

        int visCount = 0;
        Hex hex = m_Hexes[m_Height - 1][ix];
        for (int i = 0; i < hex.m_Neighbor.Length; i++)
        {
            if (hex.m_Neighbor[i] != null)
            {
                Hex neighbor = hex.m_Neighbor[i];
                for (int j = 0; j < neighbor.m_Neighbor.Length; j++)
                {
                    Hex neighborsNeighbor = neighbor.m_Neighbor[j];
                    if (neighborsNeighbor != null)
                    {
                        Hex.HexIndex hi = neighborsNeighbor.m_HexPointIndeces[CENTER_INDEX];
                        m_HexPoints[hi.iy][hi.ix].hexPointType = HexPoint.HexPointType.Sea;
                        neighborsNeighbor.SetHexVisibility(Hex.HexVisibility.Known);
                    }
                }
            }
        }
        for (int i = 0; i < hex.m_Neighbor.Length; i++)
        {
            if (hex.m_Neighbor[i] != null)
            {
                Hex neighbor = hex.m_Neighbor[i];
                Hex.HexIndex hi = neighbor.m_HexPointIndeces[CENTER_INDEX];
                m_HexPoints[hi.iy][hi.ix].hexPointType = HexPoint.HexPointType.Land;
                neighbor.SetHexVisibility(Hex.HexVisibility.Known);
                visCount++;
                if (visCount == 2)
                {
                    homeIndex = neighbor.m_ThisHexIndex;
                    m_Home = neighbor;
                    m_HomeTri = NavigationController.HexTri.N;
                }
            }
        }
        for (int e = 0; e < hex.m_HexPointIndeces.Length; e++)
        {
            Hex.HexIndex hi = hex.m_HexPointIndeces[e];
            m_HexPoints[hi.iy][hi.ix].hexPointType = HexPoint.HexPointType.Land;
        }
        hex.SetHexVisibility(Hex.HexVisibility.Known);

        m_HomeIndex = homeIndex;
    }

    void AssignRandomSubType(Hex hex)
    {
        if (hex.m_HexVisibility != Hex.HexVisibility.Known)
        {
            int index = hex.GetHexVertexIndex();

            if (index < 0x3f)
            {
                if (Random.Range(0f, 1f) < m_HexWaystationProbability)
                {
                    hex.SetHexSubType(Hex.HexSubType.Waystation);
                }
                else if (Random.Range(0f, 1f) < m_HexHazardProbability)
                {
                    hex.SetHexSubType(Hex.HexSubType.Hazard);
                }
            }
        }
    }

    void AreaFillHexPoints(int ix, int iy, Island island)
    {
        if (island.m_HexPointIndeces == null)
        {
            island.m_HexPointIndeces = new List<Hex.HexIndex>();
        }

        if (m_HexPoints[iy][ix].m_Island == null)
        {
            if (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
            {
                //if (!island.m_HexPoints.Contains(m_HexPoints[iy][ix]))
                if (!island.m_HexPointIndeces.Contains(new Hex.HexIndex(ix, iy)))
                {
                    island.m_HexPointIndeces.Add(new Hex.HexIndex(ix, iy));
                    m_HexPoints[iy][ix].m_Island = island;
                }

                for (int i = 0; i < 6; i++)
                {
                    int dx = 0;
                    int dy = 0;
                    if ((ix & 0x01) == 0)
                    {
                        dx = ix + HexPoint.HEX_POINT_NEIGHBOR_DELTA[0][i].i0;
                        dy = iy + HexPoint.HEX_POINT_NEIGHBOR_DELTA[0][i].i1;
                    }
                    else
                    {
                        dx = ix + HexPoint.HEX_POINT_NEIGHBOR_DELTA[1][i].i0;
                        dy = iy + HexPoint.HEX_POINT_NEIGHBOR_DELTA[1][i].i1;
                    }

                    if ((dx >= 0)
                        && (dx < m_WidthCount)
                        && (dy >= 0)
                        && (dy < m_HeightCount))
                    {
                        //if (!island.m_HexPoints.Contains(m_HexPoints[dy][dx]))
                        if (!island.m_HexPointIndeces.Contains(new Hex.HexIndex(dx, dy)))
                        {
                            AreaFillHexPoints(dx, dy, island);
                        }
                    }
                }
            }
        }
    }
    void IdentifyIslands()
    {
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            for (int ix = 0; ix < m_WidthCount; ix++)
            {
                if ((m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
                    && (m_HexPoints[iy][ix].m_Island == null))
                {
                    Island island = new Island();
                    island.m_IslandIndex = m_Islands.Count;
                    m_Islands.Add(island);
                    AreaFillHexPoints(ix, iy, island);
                }
            }
        }
    }

    void ClearUnknownHexPoints()
    {
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            for (int ix = 0; ix < m_WidthCount; ix++)
            {
                if (m_HexPoints[iy][ix].pointVisibility == Hex.HexVisibility.Unknown)
                {
                    m_HexPoints[iy][ix].hexPointType = HexPoint.HexPointType.Sea;
                }
            }
        }
        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                SetDecalAndHexPointMask(m_Hexes[iy][ix]);
            }
        }

    }
    void GenerateIslands()
    {
        HexPoint.HexPointType[][] tempSea = new HexPoint.HexPointType[m_HeightCount][];
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            tempSea[iy] = new HexPoint.HexPointType[m_WidthCount];
        }
        for (int pass = 0; pass < m_HexBuildPasses.Count; pass++)
        {
            for (int iy = 0; iy < m_HeightCount; iy++)
            {
                for (int ix = 0; ix < m_WidthCount; ix++)
                {
                    tempSea[iy][ix] = m_HexPoints[iy][ix].hexPointType;
                }
            }

            for (int iy = 0; iy < m_Height; iy++)
            {
                for (int ix = 0; ix < m_Width; ix++)
                {
                    if (m_Hexes[iy][ix].m_HexVisibility == Hex.HexVisibility.Known)
                    {
                        continue;
                    }

                    Hex.HexIndex hi = m_Hexes[iy][ix].m_HexPointIndeces[CENTER_INDEX];
                    if ((hi.ix < 2) ||
                        (hi.ix > (m_WidthCount - 3)) ||
                        (hi.iy == 0) ||
                        (hi.iy == (m_HeightCount - 1))
                        )
                    {
                        tempSea[hi.iy][hi.ix] = HexPoint.HexPointType.Sea;
                    }
                    else
                    {
                        // First Hex Land probability
                        if (pass == 0)
                        {
                            tempSea[hi.iy][hi.ix] = (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexLandProbability ? HexPoint.HexPointType.Land : HexPoint.HexPointType.Sea);
                        }
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexLandProbability)
                            {
                                tempSea[hi.iy][hi.ix] = HexPoint.HexPointType.Land;
                            }
                        }
                        // now each edge point
                        for (int e = 0; e < 7; e++)
                        {
                            if (e != CENTER_INDEX)
                            {
                                Hex.HexIndex hiE = m_Hexes[iy][ix].m_HexPointIndeces[e];
                                // Now count neighbors
                                if (m_HexPoints[hiE.iy][hiE.ix].pointVisibility == Hex.HexVisibility.Known)
                                {
                                    continue;
                                }

                                int count = 0;
                                if (tempSea[hi.iy][hi.ix] == HexPoint.HexPointType.Land)
                                {
                                    count = CountNeighbors(ix, iy, e);
                                }
                                if (count == 0)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge0NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 1)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge1NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 2)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge2NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 3)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge3NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 4)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge4NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 5)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge5NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                                else if (count == 6)
                                {
                                    if (Random.Range(0.0f, 1.0f) < m_HexBuildPasses[pass].m_HexEdge6NeighborsProbability)
                                    {
                                        tempSea[hiE.iy][hiE.ix] = HexPoint.HexPointType.Land;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int iy = 0; iy < m_HeightCount; iy++)
            {
                for (int ix = 0; ix < m_WidthCount; ix++)
                {
                    m_HexPoints[iy][ix].hexPointType = tempSea[iy][ix];
                }
            }
        }
    }

    void GenerateSubTypes()
    {
        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                if (m_Hexes[iy][ix].m_HexVisibility != Hex.HexVisibility.Known)
                {
                    if (m_Hexes[iy][ix].GetHexCenterPointType() == HexPoint.HexPointType.Land)
                    {
                        AssignRandomSubType(m_Hexes[iy][ix]);
                    }
                }
            }
        }
    }

    public void BuildMap()
    {
        float xStep = m_Scale * 0.25f;
        float yStep = HexMapManager.COS_30 * 0.5f;

        float mapWidth = ((float)m_Width * 0.75f) + 0.25f;
        float mapHeight = ((float)m_Height + 0.5f) * HexMapManager.COS_30;

        m_WidthCount = (m_Width * 3) + 2;
        m_HeightCount = m_Height + 1;

        m_HexPoints = new HexPoint[m_HeightCount][];
        for (int i = 0; i < m_HeightCount; i++)
        {
            m_HexPoints[i] = new HexPoint[m_WidthCount];
        }
        m_Hexes = new Hex[m_Height][];
        for (int i = 0; i < m_Height; i++)
        {
            m_Hexes[i] = new Hex[m_Width];
        }

        float maxHeight = mapHeight * 0.5f;
        float xOffset = -mapWidth * 0.5f * m_Scale;

        if (m_PointParent == null)
        {
            GameObject pntParent = new GameObject();
            pntParent.name = "Points";
            //pntParent.SetActive(false);
            m_PointParent = pntParent.transform;
        }
        m_PointParent.parent = transform;

        float x = 0;
        float y = 0;
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            for (int ix = 0; ix < m_WidthCount; ix++)
            {
                m_HexPoints[iy][ix] = new HexPoint();
                m_HexPoints[iy][ix].indeces = new Hex.HexIndexPair(ix, iy);
                if ((ix & 0x01) == 0)
                {
                    m_HexPoints[iy][ix].position = new Vector3(((float)ix * xStep) + xOffset, maxHeight - ((float)((iy * 2) + 1) * yStep), 0);
                }
                else
                {
                    m_HexPoints[iy][ix].position = new Vector3(((float)ix * xStep) + xOffset, maxHeight - ((float)(iy * 2) * yStep), 0);
                }

                if (m_SourceTexture == null)
                {
                    m_HexPoints[iy][ix].hexPointType = HexPoint.HexPointType.Sea;
                }
            }
        }

        float minX = m_HexPoints[0][0].position.x;
        float minY = m_HexPoints[m_HeightCount - 1][0].position.y;

        m_HexMapMin = new Vector3(minX, minY, 0);
        m_HexMapZero = m_HexPoints[0][2].position;

        GameObject hexParent = new GameObject();
        hexParent.name = "Hexes";
        hexParent.transform.parent = transform;
        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                int startIx = ix * 3;
                int centerIx = startIx + 2;

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = string.Format("hex_{0:D2}_{1:D2}", iy, ix);
                Hex hex = go.AddComponent<Hex>();
                Collider collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }
                Renderer renderer = go.GetComponent<Renderer>();
                hex.m_Renderer = renderer;
                renderer.sharedMaterial = m_HexMtl;
                hex.HexMaterial = renderer.sharedMaterial;

                MeshFilter mf = go.GetComponent<MeshFilter>();

                Vector3[] verts = new Vector3[7];
                Vector3[] norms = new Vector3[7];
                for (int i = 0; i < 7; i++)
                {
                    norms[i] = new Vector3(0, 0, -1);
                }
                float yTextureScale = 0.5f - (HexMapManager.COS_30 / 2.0f);
                float yTextureTop = 1.0f - yTextureScale;
                float yTextureBottom = yTextureScale;
                Vector2[] uvs = new Vector2[7]
                {
                    new Vector2(0f, 0.5f),
                    new Vector2(0.25f, yTextureTop),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.75f, yTextureTop),
                    new Vector2(1.0f, 0.5f),
                    new Vector2(0.25f, yTextureBottom),
                    new Vector2(0.75f, yTextureBottom),
                };
                Hex.HexIndex[] hexIndices = new Hex.HexIndex[7];
                if ((ix & 0x01) == 0)
                {
                    int i = 0;
                    for (i = 0; i < 5; i++)
                    {
                        verts[i] = m_HexPoints[iy][i + startIx].position;
                        hexIndices[i] = new Hex.HexIndex(i + startIx, iy);
                    }
                    hexIndices[i] = new Hex.HexIndex(startIx + 1, iy + 1);
                    verts[i++] = m_HexPoints[iy + 1][startIx + 1].position;
                    hexIndices[i] = new Hex.HexIndex(startIx + 3, iy + 1);
                    verts[i++] = m_HexPoints[iy + 1][startIx + 3].position;

                    hex.SetHexIndeces(hexIndices);
                }
                else
                {
                    int i = 0;
                    for (i = 0; i < 5; i++)
                    {
                        if ((i & 0x01) == 0)
                        {
                            verts[i] = m_HexPoints[iy + 1][i + startIx].position;
                            hexIndices[i] = new Hex.HexIndex(i + startIx, iy + 1);
                        }
                        else
                        {
                            verts[i] = m_HexPoints[iy][i + startIx].position;
                            hexIndices[i] = new Hex.HexIndex(i + startIx, iy);
                        }
                    }
                    hexIndices[i] = new Hex.HexIndex(startIx + 1, iy + 1);
                    verts[i++] = m_HexPoints[iy + 1][startIx + 1].position;
                    hexIndices[i] = new Hex.HexIndex(startIx + 3, iy + 1);
                    verts[i++] = m_HexPoints[iy + 1][startIx + 3].position;

                    hex.SetHexIndeces(hexIndices);
                }
                Vector2[] uv2s = new Vector2[7];

                for (int iuv = 0; iuv < 7; iuv++)
                {
                    uv2s[iuv] = new Vector2((verts[iuv].x / mapWidth) + 0.5f, (verts[iuv].y / mapHeight) + 0.5f);

                    if (m_SourceTexture != null)
                    {
                        int texIx = (int)((float)(m_SourceTexture.width - 1) * uv2s[iuv].x);
                        int texIy = (int)((float)(m_SourceTexture.height - 1) * uv2s[iuv].y);

                        if ((texIx < 0) ||
                            (texIx >= m_SourceTexture.width) ||
                            (texIy < 0) ||
                            (texIy >= m_SourceTexture.height))
                        {
                            Debug.LogError($"Bad uv {uv2s[iuv].x.ToString("R")},{uv2s[iuv].y.ToString("R")} tex {texIx.ToString()},{texIy.ToString()}");
                        }
                        Color c = m_SourceTexture.GetPixel(texIx, texIy);

                        HexPoint.HexPointType hpt = (c.r < 0.5f ? HexPoint.HexPointType.Land : HexPoint.HexPointType.Sea);
                        m_HexPoints[hexIndices[iuv].iy][hexIndices[iuv].ix].hexPointType = hpt;
                        GameObject hexGo = m_HexPoints[iy][ix].go;
                        if (hexGo != null)
                        {
                            MeshRenderer mr = hexGo.GetComponent<MeshRenderer>();
                            mr.material = (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land ? m_LandMtl : m_SeaMtl);
                        }
                    }

                }

                int[] fs = new int[18]
                {
                    0, 1, 2,
                    1, 3, 2,
                    2, 3, 4,
                    0, 2, 5,
                    5, 2, 6,
                    6, 2, 4
                };

                Vector3 hexPos = verts[verts.Length - 1];
                if (m_HexMesh != null)
                {
                    mf.mesh = m_HexMesh;
                }
                else
                {
                    // recenter
                    Vector3 minV = verts[0];
                    Vector3 maxV = verts[0];
                    for (int j = 1; j < verts.Length; j++)
                    {
                        if (verts[j].x < minV.x)
                        {
                            minV.x = verts[j].x;
                        }
                        if (verts[j].y < minV.y)
                        {
                            minV.y = verts[j].y;
                        }
                        if (verts[j].x > maxV.x)
                        {
                            maxV.x = verts[j].x;
                        }
                        if (verts[j].y > maxV.y)
                        {
                            maxV.y = verts[j].y;
                        }
                    }
                    Vector3 ctr = (minV + maxV) * 0.5f;
                    for (int j = 0; j < verts.Length; j++)
                    {
                        verts[j] -= ctr;
                    }

                    mf.mesh.vertices = verts;
                    mf.mesh.uv = uvs;
                    mf.mesh.uv2 = uv2s;
                    mf.mesh.normals = norms;
                    mf.mesh.triangles = fs;

                    m_HexMesh = mf.mesh;
                }
                go.transform.SetParent(hexParent.transform, false);
                go.transform.localPosition = hexPos;

                hex.SetHexVisibility(Hex.HexVisibility.Unknown);

                hex.m_ThisHexIndex = new Hex.HexIndex(ix, iy);
                m_Hexes[iy][ix] = hex;

                SetDecalAndHexPointMask(hex);

            }
        }

        SetHexNeighbors();

        if (m_SourceTexture == null)
        {
            GenerateIslands();
        }
        GenerateSubTypes();

        GenerateHome(m_Width / 2);

        SetHexSeaNeighbors();

        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                if ((ix == 50) && (iy == 48))
                {
                    Debug.Log("Check it");
                }
                SetDecalAndHexPointMask(m_Hexes[iy][ix]);
            }
        }
        m_Hexes[m_HomeIndex.iy][m_HomeIndex.ix].SetHexSubType(Hex.HexSubType.Home);
        //NavigationController.Instance.StartPath(m_Home);

        ConfigureQuad();

        IdentifyIslands();
        //for (int i = 0; i < m_Islands.Count; i++)
        //{
        //    if (m_Islands[i].m_HexPointIndeces.Count > 0)
        //    {
        //        Debug.Log($"Island {i.ToString()}: {m_Islands[i].m_HexPointIndeces.Count.ToString()}");
        //    }
        //}
    }

    void SetHexNeighbors()
    {
        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                Hex[] neighbors = new Hex[6];
                for (int i = 0; i < neighbors.Length; i++)
                {
                    neighbors[i] = null;
                    if ((ix & 0x01) == 0x01)
                    {
                        neighbors[0] = m_Hexes[iy][ix - 1];
                        if (iy > 0)
                        {
                            neighbors[1] = m_Hexes[iy - 1][ix];
                        }
                        if (ix < (m_Hexes[iy].Length - 1))
                        {
                            neighbors[2] = m_Hexes[iy][ix + 1];
                        }
                        if (iy < (m_Hexes.Length - 1))
                        {
                            if (ix < (m_Hexes[iy + 1].Length - 1))
                            {
                                neighbors[3] = m_Hexes[iy + 1][ix + 1];
                            }
                            neighbors[4] = m_Hexes[iy + 1][ix];
                            neighbors[5] = m_Hexes[iy + 1][ix - 1];
                        }
                    }
                    else
                    {
                        if (ix > 0)
                        {
                            if (iy > 0)
                            {
                                neighbors[0] = m_Hexes[iy - 1][ix - 1];
                            }
                        }
                        if (iy > 0)
                        {
                            neighbors[1] = m_Hexes[iy - 1][ix];
                            if (ix < (m_Hexes[iy - 1].Length - 1))
                            {
                                neighbors[2] = m_Hexes[iy - 1][ix + 1];
                            }
                        }
                        if (ix < (m_Hexes[iy].Length - 1))
                        {
                            neighbors[3] = m_Hexes[iy][ix + 1];
                        }
                        if (iy < (m_Hexes.Length - 1))
                        {
                            neighbors[4] = m_Hexes[iy + 1][ix];
                        }
                        if (ix > 0)
                        {
                            neighbors[5] = m_Hexes[iy][ix - 1];
                        }
                    }
                }

                m_Hexes[iy][ix].SetNeighbors(neighbors);
            }
        }
    }
    void SetHexSeaNeighbors()
    {
        for (int iy = 0; iy < m_Height; iy++)
        {
            for (int ix = 0; ix < m_Width; ix++)
            {
                Hex[] neighbors = new Hex[6];
                for (int i = 0; i < neighbors.Length; i++)
                {
                    neighbors[i] = null;
                    if ((ix & 0x01) == 0x01)
                    {
                        neighbors[0] = m_Hexes[iy][ix - 1];
                        if (iy > 0)
                        {
                            neighbors[1] = m_Hexes[iy - 1][ix];
                        }
                        if (ix < (m_Hexes[iy].Length - 1))
                        {
                            neighbors[2] = m_Hexes[iy][ix + 1];
                        }
                        if (iy < (m_Hexes.Length - 1))
                        {
                            if (ix < (m_Hexes[iy + 1].Length - 1))
                            {
                                neighbors[3] = m_Hexes[iy + 1][ix + 1];
                            }
                            neighbors[4] = m_Hexes[iy + 1][ix];
                            neighbors[5] = m_Hexes[iy + 1][ix - 1];
                        }
                    }
                    else
                    {
                        if (ix > 0)
                        {
                            if (iy > 0)
                            {
                                neighbors[0] = m_Hexes[iy - 1][ix - 1];
                            }
                        }
                        if (iy > 0)
                        {
                            neighbors[1] = m_Hexes[iy - 1][ix];
                            if (ix < (m_Hexes[iy - 1].Length - 1))
                            {
                                neighbors[2] = m_Hexes[iy - 1][ix + 1];
                            }
                        }
                        if (ix < (m_Hexes[iy].Length - 1))
                        {
                            neighbors[3] = m_Hexes[iy][ix + 1];
                        }
                        if (iy < (m_Hexes.Length - 1))
                        {
                            neighbors[4] = m_Hexes[iy + 1][ix];
                        }
                        if (ix > 0)
                        {
                            neighbors[5] = m_Hexes[iy][ix - 1];
                        }
                    }
                }

                //m_Hexes[iy][ix].SetNeighbors(neighbors);

                //Hex[] seaNeighbors = new Hex[6];
                //for (int i = 0; i < neighbors.Length; i++)
                //{
                //    if (neighbors[i] == null)
                //    {
                //        seaNeighbors[i] = null;
                //    }
                //    else
                //    {
                //        Hex hex = m_Hexes[iy][ix];
                //        // Test for valid hexTri movement
                //        // Can we get from our current hextri to this neighbor?
                //        if ((NavigationController.m_TravelIsPossible[hex.m_ThisHexIndex.hexPointMask][i] & neighborBit) == neighborBit)
                //        {

                //        }
                //        //Hex.HexIndexPair hexIP = m_Hexes[iy][ix].m_HexEdges[i];
                //        //Hex.HexIndex hiA = m_Hexes[iy][ix].m_HexPointIndeces[hexIP.i0];
                //        //Hex.HexIndex hiB = m_Hexes[iy][ix].m_HexPointIndeces[hexIP.i1];
                //        //if ((m_HexPoints[hiA.iy][hiA.ix].hexPointType == HexPoint.HexPointType.Land)
                //        // && (m_HexPoints[hiB.iy][hiB.ix].hexPointType == HexPoint.HexPointType.Land))
                //        //{
                //        //    seaNeighbors[i] = null;
                //        //}
                //        //else
                //        //{
                //        //    seaNeighbors[i] = neighbors[i];
                //        //}
                //    }
                //}
                //m_Hexes[iy][ix].SetSeaNeighbors(seaNeighbors);
            }
        }
    }
    void SetDecalAndHexPointMask(Hex hex)
    {
        //int index = 0;
        //for (int i = 0; i < hex.m_HexPointIndeces.Length; i++)
        //{
        //    int ix = hex.m_HexPointIndeces[i].ix;
        //    int iy = hex.m_HexPointIndeces[i].iy;
        //    if (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
        //    {
        //        index = index | (0x01 << i);
        //    }
        //}
        int index = hex.GetHexVertexIndex();

        if (index == 0)
        {
            //hex.m_Renderer.enabled = false;
            float ox = 0.9375f;
            float oy = 0.0f;
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            hex.m_TextureST = new Vector4(0.0625f, 0.125f, ox, oy);
            block.SetVector("_MainTex_ST", hex.m_TextureST);
            if (hex.m_HexVisibility == Hex.HexVisibility.Known)
            {
                hex.SetHexColor(m_SeaColor, m_SeaDiscoveredColor);
                //block.SetColor("_Color", m_SeaColor);
            }
            else
            {
                hex.SetHexColor(m_SeaDiscoveredColor, m_SeaDiscoveredColor);
                //block.SetColor("_Color", m_SeaDiscoveredColor);
            }
            hex.m_Renderer.SetPropertyBlock(block);
            hex.m_Block = block;
        }
        else
        {
            int ix = index & 0x0f;
            int iy = (index & 0xf0) >> 4;
            float ox = (float)ix / 16.0f;
            float oy = (float)(7 - iy) / 8.0f;
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            hex.m_TextureST = new Vector4(0.0625f, 0.125f, ox, oy);
            block.SetVector("_MainTex_ST", hex.m_TextureST);
            if (hex.m_HexVisibility == Hex.HexVisibility.Known)
            {
                hex.SetHexColor(m_DefaultColor, m_SeaColor);
                //block.SetColor("_Color", m_DefaultColor);
            }
            else
            {
                hex.SetHexColor(m_DefaultDiscoveredColor, m_SeaDiscoveredColor);
                //block.SetColor("_Color", m_DefaultDiscoveredColor);
            }
            hex.m_Renderer.SetPropertyBlock(block);
            hex.m_Block = block;

            // This needs tweaking.  A hex may have hex points from different islands.
            // This methodology does not allow for this.
            if (hex.m_HexVisibility != Hex.HexVisibility.Known)
            {
                if (index < 0x3f)
                {
                    if (Random.Range(0f, 1f) < m_HexWaystationProbability)
                    {
                        hex.SetHexSubType(Hex.HexSubType.Waystation);
                    }
                    else if (Random.Range(0f, 1f) < m_HexHazardProbability)
                    {
                        hex.SetHexSubType(Hex.HexSubType.Hazard);
                    }
                }
            }
        }

        int maskBits = 0;
        for (int i = 0; i < 7; i++)
        {
            Hex.HexIndex hpIdx = hex.m_HexPointIndeces[i];
            if (m_HexPoints[hpIdx.iy][hpIdx.ix].hexPointType == HexPoint.HexPointType.Land)
            {
                maskBits = maskBits | (1 << i);
            }
        }
        if (hex.m_ThisHexIndex == null)
        {
            Debug.LogError("m_ThisHexIndex null, cant set hexPointMask");
        }
        else
        {
            hex.m_ThisHexIndex.hexPointMask = maskBits;
        }
    }
    void ConfigureQuad()
    {
        float xScl = (((float)m_Width) * 0.75f) + 0.25f;
        float yScl = (float)((2 * m_Height) + 1) * (HexMapManager.COS_30 * 0.5f);

        float uTile = (((float)m_Width) * 0.5f) + (1.0f / 6.0f);
        float vTile = ((float)m_Height) + 0.5f;

        float uOffset = 0.5f;

        m_HexTexQuadTransform.localPosition = new Vector3(RISE_DROP, -(HexMapManager.COS_30 * 0.5f), 0f);
        m_HexTexQuadTransform.localScale = new Vector3(xScl, yScl, 1.0f);
        MeshRenderer mr = m_HexTexQuadTransform.GetComponent<MeshRenderer>();
        mr.material.SetTextureOffset("_MainTex", new Vector2(uOffset, 0.0f));
        mr.material.SetTextureScale("_MainTex", new Vector2(uTile, vTile));

    }

    public Hex FetchHexFromClickPoint()
    {
        Vector3 pos = GetPickPoint();
        Hex hex = FetchHex(pos);
        return hex;
    }

    Hex FetchHex(Vector3 samplePoint)
    {
        Hex hexHit = null;

        Vector3 flippedPnt = new Vector3(samplePoint.x, -samplePoint.y, samplePoint.z);

        Vector3 pnt = flippedPnt - m_HexMapMin;
        pnt = pnt - new Vector3(RISE_DROP, HexMapManager.HALF_COS_30, 0);

        int ix = (int)(pnt.x / 0.75f);
        int iy = 0;
        if ((ix & 0x01) == 0x01)
        {
            iy = (int)((pnt.y - HexMapManager.HALF_COS_30) / HexMapManager.COS_30);
        }
        else
        {
            iy = (int)(pnt.y / HexMapManager.COS_30);
        }

        if (m_DBG)
        {
            Debug.Log($"pnt {pnt.x.ToString("R")},{pnt.y.ToString("R")} i {ix.ToString()},{iy.ToString()}");
        }

        if ((iy >= 0)
            && (iy < m_Height)
            && (ix >= 0)
            && (ix < m_Width)
            )
        {
            if (iy >= m_Hexes.Length)
            {
                if (m_DBG)
                {
                    Debug.Log($"iy {iy.ToString()} m_Hexes.Length {m_Hexes.Length.ToString()}");
                }
                return hexHit;
            }

            if (ix >= m_Hexes[iy].Length)
            {
                if (m_DBG)
                {
                    Debug.Log($"ix {ix.ToString()} m_Hexes[{iy.ToString()}].Length {m_Hexes[iy].Length.ToString()}");
                }
                return hexHit;
            }

            if (m_Hexes[iy][ix] == null)
            {
                if (m_DBG)
                {
                    Debug.Log($"Null Hex at {ix.ToString()},{iy.ToString()}");
                }
                return hexHit;
            }
            if (m_Hexes[iy][ix].m_HexPointIndeces == null)
            {
                if (m_DBG)
                {
                    Debug.Log($"Null Indeces {ix.ToString()},{iy.ToString()}");
                }
                return hexHit;
            }
            if (m_Hexes[iy][ix].m_HexPointIndeces.Length < 7)
            {
                if (m_DBG)
                {
                    Debug.Log($"Indeces Length {ix.ToString()},{iy.ToString()} len {m_Hexes[iy][ix].m_HexPointIndeces.Length.ToString()}");
                }
                return hexHit;
            }
            Hex.HexIndex hi = m_Hexes[iy][ix].m_HexPointIndeces[6];

            hexHit = m_Hexes[iy][ix];

            ix = hi.ix;
            iy = hi.iy;

            if (m_DBG)
            {
                Debug.Log($"hi {ix.ToString()},{iy.ToString()}");
            }


            Vector3 ctr = m_HexPoints[iy][ix].position;
            m_TraceBox.position = ctr;
        }

        return hexHit;
    }

    public void OnMouseDown()
    {
        Vector3 pos = GetPickPoint();
        Debug.Log($"OnHexClick {pos.ToString("R")}");
        Hex hex = FetchHex(pos);
        if (hex != null)
        {
            StartCoroutine(ShowNeighbors(hex));
        }
        else
        {
            Debug.Log($"OnHexClick NO HEX");
        }
    }

    Vector3 GetHexPos(Hex hex)
    {
        Hex.HexIndex hi = hex.m_HexPointIndeces[6];

        int ix = hi.ix;
        int iy = hi.iy;

        Vector3 ctr = m_HexPoints[iy][ix].position;
        return ctr;
    }
    IEnumerator ShowNeighbors(Hex hex)
    {
        Vector3 ctr = GetHexPos(hex);

        m_NullHexPicked.position = ctr;
        m_NullHexPicked.gameObject.SetActive(false);

        Debug.Log($"Hex {hex.m_ThisHexIndex.ix.ToString()},{hex.m_ThisHexIndex.iy.ToString()}");

        for (int i = 0; i < 6; i++)
        {
            if (hex.m_Neighbor[i] == null)
            {
                Debug.Log($"N {i.ToString()}: NULL");
                m_HexPicked.gameObject.SetActive(false);
                m_NullHexPicked.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"N {i.ToString()}: {hex.m_Neighbor[i].m_ThisHexIndex.ix.ToString()},{hex.m_Neighbor[i].m_ThisHexIndex.iy.ToString()}");
                ctr = GetHexPos(hex.m_Neighbor[i]);
                m_HexPicked.position = ctr;
                m_HexPicked.gameObject.SetActive(true);
                m_NullHexPicked.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(m_HexNeighborDwellTime);
            m_HexPicked.gameObject.SetActive(false);
            m_NullHexPicked.gameObject.SetActive(false);
            yield return new WaitForSeconds(m_HexNeighborDwellTime);
        }
        yield return 0;
    }

    //public void MoveDiscoveredHexesToKnownHexes()
    //{
    //    for (int iy = 0; iy < m_Height; iy++)
    //    {
    //        for (int ix = 0; ix < m_Width; ix++)
    //        {
    //            if (m_Hexes[iy][ix].m_HexVisibility == Hex.HexVisibility.Discovered)
    //            {
    //                Hex.HexSubType subType = m_Hexes[iy][ix].GetHexSubType();
    //                m_Hexes[iy][ix].SetHexVisibility(Hex.HexVisibility.Known);
    //                for (int e = 0; e < m_Hexes[iy][ix].m_HexPointIndeces.Length; e++)
    //                {
    //                    Hex.HexIndex hi = m_Hexes[iy][ix].m_HexPointIndeces[e];
    //                    if (hi != null)
    //                    {
    //                        m_HexPoints[hi.iy][hi.ix].pointVisibility = Hex.HexVisibility.Known;
    //                    }
    //                }
    //                SetDecal(m_Hexes[iy][ix]);
    //            }
    //        }
    //    }
    //}

    public void MoveDiscoveredHexesFromListToKnownHexes(List<Hex> discoveredHexes)
    {
        for (int i = 0; i < discoveredHexes.Count; i++)
        {
            Hex hex = discoveredHexes[i];
            if (hex.m_HexVisibility == Hex.HexVisibility.Discovered)
            {
                Hex.HexSubType subType = hex.GetHexSubType();
                hex.SetHexVisibility(Hex.HexVisibility.Known);
                for (int e = 0; e < hex.m_HexPointIndeces.Length; e++)
                {
                    Hex.HexIndex hi = hex.m_HexPointIndeces[e];
                    if (hi != null)
                    {
                        m_HexPoints[hi.iy][hi.ix].pointVisibility = Hex.HexVisibility.Known;
                    }
                }
                SetDecalAndHexPointMask(hex);
            }
        }
    }

    public void RegenerateUnknownHexes()
    {
        //ClearUnknownHexPoints();
        //if (m_SourceTexture == null)
        //{
        //    GenerateIslands();
        //}
        //GenerateSubTypes();
    }
    void Start()
    {
        BuildMap();
    }

    Vector3 GetPickPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float s = -(ray.origin.z / ray.direction.z); // finding where z=0 is in x and y

        Vector3 pos = new Vector3(ray.origin.x + (ray.direction.x * s),
                                  ray.origin.y + (ray.direction.y * s),
                                  0);
        return pos;
    }
    void Update()
    {
        Vector3 pos = GetPickPoint();
        if (m_DBG)
        {
            Debug.Log($"Pos {pos.ToString("R")}");
        }
        m_TraceSphere.position = pos;

        //Hex hex = FetchHex(pos);
        //if (hex != null)
        //{
        //    hex.SetHexVisibility(Hex.HexVisibility.Known);
        //}
    }
    public void DumpIslandInfo()
    {
        for (int i = 0; i < m_Islands.Count; i++)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{i.ToString()}: \"{m_Islands[i].m_IslandName}\" HexPoint Count {m_Islands[i].m_HexPointIndeces.Count}\n");
            for (int j = 0; j < m_Islands[i].m_HexPointIndeces.Count; j++)
            {
                Hex.HexIndex hi = m_Islands[i].m_HexPointIndeces[j];
                HexPoint hp = HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix];

                sb.Append($"i {j.ToString()} ix {hp.indeces.i0.ToString()} iy {hp.indeces.i1.ToString()}\n");
            }

            Debug.Log(sb.ToString());
        }
    }
    public void DumpIslandAt(int idx)
    {
        if ((idx >= 0) && (idx < m_Islands.Count))
        {
            Island island = m_Islands[idx];
            for (int j = 0; j < island.m_HexPointIndeces.Count; j++)
            {
                Hex.HexIndex hi = island.m_HexPointIndeces[j];
                HexPoint hp = HexMapBuilder.Instance.HexPoints[hi.iy][hi.ix];
                Vector3 pos = hp.position;
                pos = pos + new Vector3(0.25f, -HexMapManager.COS_30 * 0.5f, 0);
                Vector3 hp0x = pos - new Vector3(0.1f, 0, 0);
                Vector3 hp1x = pos + new Vector3(0.1f, 0, 0);
                Vector3 hp0y = pos - new Vector3(0, 0.1f, 0);
                Vector3 hp1y = pos + new Vector3(0, 0.1f, 0);
                if (j == 0)
                {
                    Debug.DrawLine(hp0x, hp1x, Color.red, 5.0f);
                    Debug.DrawLine(hp0y, hp1y, Color.red, 5.0f);
                }
                else
                {
                    Debug.DrawLine(hp0x, hp1x, Color.magenta, 5.0f);
                    Debug.DrawLine(hp0y, hp1y, Color.magenta, 5.0f);
                }
            }
        }
    }
    public void DrawHexPoints()
    {
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            for (int ix = 0; ix < m_WidthCount; ix++)
            {
                HexPoint hp = m_HexPoints[iy][ix];
                Vector3 pos = hp.position;
                pos = pos + new Vector3(0.25f, -HexMapManager.COS_30 * 0.5f, 0);
                Vector3 hp0x = pos - new Vector3(0.1f, 0, 0);
                Vector3 hp1x = pos + new Vector3(0.1f, 0, 0);
                Vector3 hp0y = pos - new Vector3(0, 0.1f, 0);
                Vector3 hp1y = pos + new Vector3(0, 0.1f, 0);
                if (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
                {
                    Debug.DrawLine(hp0x, hp1x, Color.yellow, 5000.0f);
                    Debug.DrawLine(hp0y, hp1y, Color.yellow, 5000.0f);
                }
                else
                {
                    Debug.DrawLine(hp0x, hp1x, Color.cyan, 5000.0f);
                    Debug.DrawLine(hp0y, hp1y, Color.cyan, 5000.0f);
                }
            }
        }
    }
}
