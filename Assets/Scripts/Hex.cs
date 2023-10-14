using System.Collections;
using System.Collections.Generic;
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
    public enum HexVisibility { Undefined, Unknown, Known, Discovered };
    public class HexIndex
    {
        public int ix;
        public int iy;
        public HexIndex()
        {
            ix = 0;
            iy = 0;
        }
        public HexIndex(int x, int y)
        {
            ix = x;
            iy = y;
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
    public HexVisibility m_HexVisibility = HexVisibility.Undefined;
    public Hex[] m_Neighbor = new Hex[7] { null, null, null, null, null, null, null };
    public int[] m_TransitionCost = new int[] { 0, 0, 0, 0, 0, };

    public HexIndex m_ThisHexIndex;
    public HexIndex[] m_HexPointIndeces = new HexIndex[7];
    public Renderer m_Renderer;
    public MaterialPropertyBlock m_Block;

    public Vector4 m_TextureST;
    public Color m_Color = Color.white;
    public Vector4 m_BlockTextureST;
    public Color m_BlockColor = Color.white;

    int m_UnknownLayer = 0;
    int m_KnownLayer = 0;
    int m_DiscoveredLayer = 0;


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
            //Debug.Log($"layer in {vis.ToString()} obj layer {gameObject.layer.ToString()}");
        }
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

    public Vector3 GetHexPosition()
    {
        return transform.position;
    }

    void Update()
    {
        if ((m_BlockColor != m_Color)
            || (m_BlockTextureST != m_TextureST)
            )
        { 
            m_Block = new MaterialPropertyBlock();
            m_Block.SetVector("_MainTex_ST", m_TextureST);
            m_Block.SetColor("_Color", m_Color);
            m_Renderer.SetPropertyBlock(m_Block);
            m_BlockColor = m_Color;
            m_BlockTextureST = m_TextureST;
        }
    }
}
