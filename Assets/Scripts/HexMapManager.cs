using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapManager : MonoBehaviour
{
    public const float COS_30 = 0.86602540378443864676372317075294f;
    public const float HALF_COS_30 = 0.43301270189221932338186158537647f;
    public const float HALF_COS_30_SQ = 0.1875f;

    public int m_Height = 10;
    public int m_Width = 12;
    public float m_CellSize = 1.0f;
    public GameObject m_HexPrefab;

    // Start is called before the first frame update
    void Start()
    {
        float yStep = 0.75f;
        float yOffsetStep = 0;
        float xStep = COS_30;
        float xOffsetStep = -COS_30 / 2.0f;

        float startX = -((float)m_Width / 2.0f) * xStep;
        float startY = -((float)m_Width / 2.0f) * yStep;

        float yPos = startY;
        for (int y = 0; y < m_Height; y++)
        {
            float xPos = startX;
            if ((y & 1) == 1)
            {
                xPos += xOffsetStep;
            }
            for (int x = 0; x < m_Width; x++)
            {
                GameObject go = Instantiate(m_HexPrefab);
                go.name = "hex_" + x.ToString() + "_" + y.ToString();
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3(xPos, yPos, 0);

                xPos = xPos + xStep;
            }
            yPos = yPos + yStep;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
