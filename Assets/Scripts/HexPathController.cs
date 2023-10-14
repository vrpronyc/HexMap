using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexPathController : MonoBehaviour
{
    public TextMeshPro m_TextMeshPro;

    public void SetLabel(string label)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.text = label;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
