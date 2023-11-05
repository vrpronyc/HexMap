using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public interface IModalDialog
{
    public string GetDialogName();
    public GameObject GetGameObject();
    public bool Initialize(UnityAction<object[]> callback, params object[] args);
}

[RequireComponent(typeof(Canvas))]
public class ModalCanvasController : MonoBehaviour
{
    static ModalCanvasController m_instance;
    public static ModalCanvasController Instance
    {
        get
        {
            return m_instance;
        }
    }
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
        AwakeInit();
    }

    Canvas m_Canvas;

    public IModalDialog[] m_ModalDialogs;

    // Start is called before the first frame update
    void AwakeInit()
    {
        m_Canvas = GetComponent<Canvas>();
        if (m_Canvas == null)
        {
            Debug.LogError("NO CANVAS");
        }
        else
        {
            m_Canvas.enabled = false;
        }

        m_ModalDialogs = GetComponentsInChildren<IModalDialog>(true);
    }

    public bool ModalCanvasActive()
    {
        return m_Canvas.enabled;
    }

    public void DeactivateModalCanvas()
    {
        m_Canvas.enabled = false;
    }

    public void ActivateDialog(string dialogName, UnityAction<object[]> callback, params object[] args)
    {
        int idx = Array.FindIndex(m_ModalDialogs, n => n.GetDialogName() == dialogName);
        if (idx > -1)
        {
            GameObject obj = m_ModalDialogs[idx].GetGameObject();
            if (obj != null)
            {
                if (m_ModalDialogs[idx].Initialize(callback, args))
                {
                    m_Canvas.enabled = true;
                    obj.SetActive(true);
                }
            }
            else
            {
                Debug.LogError($"No Game Object on Dialong \"{dialogName}\"");
            }
        }
        else
        {
            Debug.Log($"No Dialog \"{dialogName}\" found");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
