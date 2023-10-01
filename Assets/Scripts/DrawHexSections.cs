using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHexSections : MonoBehaviour
{
    public Transform m_ObjTransform;

    Vector3 P0;
    Vector3 P1;
    Vector3 P2;

    List<Vector3> wires = new List<Vector3>();

    Color lineColor;
    public GameObject myMesh;

    public Material lineMaterial;
    public Material m_TriMaterial0;
    public Material m_TriMaterial1;

    public Vector3[] m_Verts =
    {
        new Vector3(-0.5f, 0, 0),
        new Vector3(-0.25f, HexMapManager.COS_30 * 0.5f, 0),
        new Vector3(0, 0, 0),
        new Vector3(0.25f, HexMapManager.COS_30 * 0.5f, 0),
        new Vector3(0.5f, 0, 0),
        new Vector3(-0.25f, -HexMapManager.COS_30 * 0.5f, 0),
        new Vector3(0.25f, -HexMapManager.COS_30 * 0.5f, 0),
    };

    void Start()
    {

        ////CreateLineMaterial();

        //MeshFilter filter = myMesh.GetComponent<MeshFilter>();
        //var mesh = filter.mesh;
        //var vertices = mesh.vertices;
        //var triangles = mesh.triangles;

        //print(vertices.Length);

        //for (int k = 0; k < triangles.Length / 3; k++)
        //{
        //    wires.Add(vertices[triangles[k * 3]]);
        //    wires.Add(vertices[triangles[k * 3 + 1]]);
        //    wires.Add(vertices[triangles[k * 3 + 2]]);
        //}

        //wires.Add(vertices[triangles[triangles.Length - 2]]);
        //wires.Add(vertices[triangles[triangles.Length - 1]]);
    }

    void SplitTriangle(Vector3 p0, Vector3 p1, Vector3 p2, bool startWithBlack)
    {
        Vector3 p01 = (p0 + p1) * 0.5f;
        Vector3 p12 = (p1 + p2) * 0.5f;
        Vector3 p20 = (p2 + p0) * 0.5f;

        Color colorA = startWithBlack ? Color.black : Color.white;
        Color colorB = startWithBlack ? Color.white : Color.black;

        GL.Color(colorA);
        GL.Vertex(p0);
        GL.Vertex(p01);
        GL.Vertex(p20);

        GL.Vertex(p01);
        GL.Vertex(p1);
        GL.Vertex(p12);

        GL.Vertex(p12);
        GL.Vertex(p2);
        GL.Vertex(p20);

        GL.Color(colorB);
        GL.Vertex(p01);
        GL.Vertex(p12);
        GL.Vertex(p20);
    }
    void OnPostRender()
    {
        m_TriMaterial0.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(m_ObjTransform.localToWorldMatrix);

        GL.Begin(GL.TRIANGLES);

        //GL.Color(new Color(0.0f, 0.0f, 0.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[0]);
        //GL.Vertex(m_Verts[1]);
        SplitTriangle(m_Verts[2], m_Verts[0], m_Verts[1], true);

        //GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[1]);
        //GL.Vertex(m_Verts[3]);
        SplitTriangle(m_Verts[2], m_Verts[1], m_Verts[3], false);

        //GL.Color(new Color(0.0f, 0.0f, 0.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[3]);
        //GL.Vertex(m_Verts[4]);
        SplitTriangle(m_Verts[2], m_Verts[3], m_Verts[4], true);

        //GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[5]);
        //GL.Vertex(m_Verts[0]);
        SplitTriangle(m_Verts[2], m_Verts[5], m_Verts[0], false);

        //GL.Color(new Color(0.0f, 0.0f, 0.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[6]);
        //GL.Vertex(m_Verts[5]);
        SplitTriangle(m_Verts[2], m_Verts[6], m_Verts[5], true);

        //GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0F));
        //GL.Vertex(m_Verts[2]);
        //GL.Vertex(m_Verts[4]);
        //GL.Vertex(m_Verts[6]);
        SplitTriangle(m_Verts[2], m_Verts[4], m_Verts[6], false);

        GL.End();
        GL.PopMatrix();
    }
}
