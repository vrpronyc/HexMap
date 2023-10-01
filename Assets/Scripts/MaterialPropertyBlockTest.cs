using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertyBlockTest : MonoBehaviour
{
    MaterialPropertyBlock m_block = null;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        m_block = new MaterialPropertyBlock();
        float ox = Random.Range(0.0f, 1.0f);
        float oy = Random.Range(0.0f, 1.0f);
        m_block.SetVector("_MainTex_ST", new Vector4(0.5f, 0.5f, ox, oy));
        //m_block.SetVector("_MainTex_ST", new Vector4(0.5f, 0.5f, 0.25f, 0.25f));
        mr.SetPropertyBlock(m_block);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
