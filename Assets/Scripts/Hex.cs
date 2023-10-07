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

/// </summary>
public class Hex : MonoBehaviour
{
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
    public Hex[] m_Neighbor = new Hex[7] { null, null, null, null, null, null, null };
    public HexIndex[] m_Indeces = new HexIndex[7];
    public Renderer m_Renderer;
    public MaterialPropertyBlock m_Block;

    public Vector4 m_TextureST;
    public Color m_Color = Color.white;
    public Vector4 m_BlockTextureST;
    public Color m_BlockColor = Color.white;

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
    void Start()
    {
        m_Neighbor[6] = this;
    }

    public void SetHexIndeces(HexIndex[] hexIndecies)
    {
        for (int i = 0; i < hexIndecies.Length; i++)
        {
            m_Indeces[i] = hexIndecies[i];
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

    // Update is called once per frame
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
