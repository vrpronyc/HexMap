using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class HexGlobe : MonoBehaviour
{
    public float m_Radius = 6.0f;
    public int m_LatCount = 8;
    public int m_LngCount = 8;
    MeshFilter m_MeshFilter;

    // Start is called before the first frame update
    void Start()
    {
        StringBuilder sb = new StringBuilder();
        m_MeshFilter = GetComponent<MeshFilter>();

        float lat = 90.0f;
        float dLat = -lat / (float)(m_LatCount);
        List<Vector3> vertList = new List<Vector3>();
        List<Vector3> vnList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<int> rOffset = new List<int>();
        Vector3[][] verts = new Vector3[m_LatCount + 1][];
        Vector3[][] vns = new Vector3[m_LatCount + 1][];
        Vector2[][] uvs = new Vector2[m_LatCount + 1][];
        int[] startingV = new int[m_LatCount + 1];
        for (int i = 0; i <= m_LatCount; i++)
        {
            int lngCount = i * 6;
            if (lngCount == 0)
            {
                lngCount = 1;
            }
            int offset = (i - 1) / 2;
            rOffset.Add(offset);
            verts[i] = new Vector3[lngCount];
            vns[i] = new Vector3[lngCount];
            uvs[i] = new Vector2[lngCount];
            float dLng = 0;
            if (i > 0)
            {
                dLng = 1.0f / (float)lngCount;
            }
            dLng = dLng * 360.0f;
            float lng = 0.0f;
            float latRadius = Mathf.Cos(lat * Mathf.Deg2Rad);
            float maxZ = latRadius * Mathf.Sin(60.0f * Mathf.Deg2Rad);
            if (i == 0)
            {
                startingV[i] = 0;
            }
            else
            {
                startingV[i] = startingV[i - 1] + lngCount;
            }
            sb.Append($"row {i.ToString()} maxZ {maxZ.ToString("R")} Start {startingV[i].ToString()}: ");
            for (int j = 0; j < lngCount; j++)
            {
                float y = Mathf.Sin(lat * Mathf.Deg2Rad) * m_Radius;
                float x = Mathf.Cos(lng * Mathf.Deg2Rad) * latRadius;
                float z = Mathf.Sin(lng * Mathf.Deg2Rad) * latRadius;
                Vector3 vert = new Vector3 (x, y, z);
                verts[i][j] = vert * m_Radius;
                vertList.Add(verts[i][j]);

                vns[i][j] = vert;
                vnList.Add(vns[i][j]);

                uvs[i][j] = new Vector2((float)j / (float)lngCount, 1.0f - ((float)i / (float)m_LatCount));
                uvList.Add(uvs[i][j]);

                lng = lng + dLng;
                sb.Append($"{vert.ToString("R")}, ");
            }
            sb.Append("\n");
            lat = lat + dLat;
        }

        List<int>triList = new List<int>();
        for (int i = 0; i < m_LatCount; i+=2)
        {
            int numHex = (i * 2) + 1;
            int rowAStart = startingV[i];
            int rowBStart = startingV[i + 1];
            int rowCStart = startingV[i + 2];
            int offsetA = 0;
            int offsetB = 0;
            int t0 = 0;
            int t1 = 0;
            int t2 = 0;
            for (int j = 0; j < ((i * 2) + 1); j++)
            {
                if ((j & 0x01) != 0x01)
                {
                    // one from A, two from B
                    t0 = rowBStart + offsetB;
                    t1 = rowAStart + offsetA;
                    t2 = rowBStart + offsetB + 1;
                }
                else
                {
                    // two from A, one from B
                    t0 = rowBStart + offsetB + 1;
                    t1 = rowAStart + offsetA;
                    t2 = rowAStart + offsetA + 1;

                    offsetA++;
                    offsetB++;
                }
                triList.Add(t0);
                triList.Add(t1);
                triList.Add(t2);
                sb.Append($"f {t0.ToString()},{t1.ToString()},{t2.ToString()} ");
            }
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
        m_MeshFilter.mesh.triangles = triList.ToArray();
        m_MeshFilter.mesh.vertices = vertList.ToArray();
        m_MeshFilter.mesh.normals = vnList.ToArray();
        m_MeshFilter.mesh.uv = uvList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
