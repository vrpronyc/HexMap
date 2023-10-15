using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexQuadController : MonoBehaviour
{
    public UnityEvent m_OnMouseDown;
    public UnityEvent m_OnRightMouseDown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log($"Got Right Mouse Down");
            m_OnRightMouseDown.Invoke();
        }
    }

    public void OnMouseDown()
    {
        if (!CameraMover.Instance.IsPointerOverUIElement())
        {
            Debug.Log($"Got Mouse Down");
            m_OnMouseDown.Invoke();
        }
    }
}
