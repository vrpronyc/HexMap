using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HexMapBuilder : MonoBehaviour
{

    public string m_HexMapResourcePath = "HexMap_";
    public Texture2D m_SourceTexture;
    public Material m_SeaMtl;
    public Material m_LandMtl;
    public Material m_HexMtl;

    Hex[][] m_Hexes;
    HexPoint[][] m_HexPoints;

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
    /// </summary>

    public int m_Width = 12;
    public int m_Height = 10;
    int m_WidthCount;
    int m_HeightCount;

    public float m_Scale = 1.0f;

    public Transform m_HexTexQuadTransform;
    public string m_TexturePropertyName = "_DecalTex";

    Dictionary<string, Texture2D> m_HexTextures = new Dictionary<string, Texture2D>();

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

        float maxHeight = mapHeight * 0.5f ;
        float xOffset = -mapWidth * 0.5f * m_Scale;

        GameObject pntParent = new GameObject();
        pntParent.name = "Points";
        pntParent.transform.parent = transform;
        pntParent.SetActive(false);
        float x = 0;
        float y = 0;
        for (int iy = 0; iy < m_HeightCount; iy++)
        {
            for (int ix = 0; ix < m_WidthCount; ix++)
            {
                m_HexPoints[iy][ix] = new HexPoint();
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
                    if ((ix < 2) ||
                        (ix > (m_WidthCount - 3)) ||
                        (iy == 0) ||
                        (iy == (m_HeightCount - 1))
                        )
                    {
                        m_HexPoints[iy][ix].hexPointType = HexPoint.HexPointType.Sea;
                    }
                    else
                    {
                        m_HexPoints[iy][ix].hexPointType = (Random.Range(0.0f, 1.0f) > 0.75f ? HexPoint.HexPointType.Land : HexPoint.HexPointType.Sea);
                    }
                }

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = string.Format("pnt_{0:D2}_{1:D2}", iy, ix);
                Collider collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }
                go.transform.localScale = Vector3.one * 0.1f;
                go.transform.position = m_HexPoints[iy][ix].position + new Vector3(0, 0, -0.01f); ;
                go.transform.SetParent(pntParent.transform, false);

                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                mr.material = (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land ? m_LandMtl : m_SeaMtl) ;
                m_HexPoints[iy][ix].go = go;
            }
        }

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
                            verts[i] = m_HexPoints[iy+1][i + startIx].position;
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
                        MeshRenderer mr = hexGo.GetComponent<MeshRenderer>();
                        mr.material = (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land ? m_LandMtl : m_SeaMtl);

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

                mf.mesh.vertices = verts;
                mf.mesh.uv = uvs;
                mf.mesh.uv2 = uv2s;
                mf.mesh.normals = norms;
                mf.mesh.triangles = fs;
                go.transform.SetParent(hexParent.transform, false);

                SetDecal(hex);
            }
        }

        ConfigureQuad();
    }

    void SetDecal(Hex hex)
    {
        int index = 0;
        for (int i = 0; i < hex.m_Indeces.Length; i++)
        {
            int ix = hex.m_Indeces[i].ix;
            int iy = hex.m_Indeces[i].iy;
            if (m_HexPoints[iy][ix].hexPointType == HexPoint.HexPointType.Land)
            {
                index = index | (0x01 << i);
            }
        }

        if (index == 0)
        {
            hex.m_Renderer.enabled = false;
        }
        else
        {
            //string bitsStr = Bits(index);
            //string path = m_HexMapResourcePath + bitsStr;
            //Texture2D hexTexture = null;
            //if (m_HexTextures.ContainsKey(path))
            //{
            //    hexTexture = m_HexTextures[path];
            //}
            //else
            //{
            //    hexTexture = Resources.Load<Texture2D>(path);
            //    m_HexTextures[path] = hexTexture;
            //}
            //hex.HexMaterial.SetTexture(m_TexturePropertyName, hexTexture);

            int ix = index & 0x0f;
            int iy = (index & 0xf0) >> 4;
            float ox = (float)ix / 16.0f;
            float oy = (float)(7 - iy) / 8.0f;
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetVector("_MainTex_ST", new Vector4(0.0625f, 0.125f, ox, oy));
            hex.m_Renderer.SetPropertyBlock(block);

        }
    }
    void ConfigureQuad()
    {
        float xScl = (((float)m_Width) * 0.75f) + 0.25f;
        float yScl = (float)((2 * m_Height) + 1) * (HexMapManager.COS_30 * 0.5f);

        float uTile = (((float)m_Width) * 0.5f) + (1.0f / 6.0f);
        float vTile = ((float)m_Height) + 0.5f;

        float uOffset = 0.5f;

        m_HexTexQuadTransform.localScale = new Vector3(xScl, yScl, 1.0f);
        MeshRenderer mr = m_HexTexQuadTransform.GetComponent<MeshRenderer>();
        mr.material.SetTextureOffset("_MainTex", new Vector2(uOffset, 0.0f));
        mr.material.SetTextureScale("_MainTex", new Vector2(uTile, vTile));
    }
    // Start is called before the first frame update
    void Start()
    {
        BuildMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
