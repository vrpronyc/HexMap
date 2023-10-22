using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SailHeaveToButtonController : MonoBehaviour
{
    public Image m_SailButton;
    public Image m_HeaveToButton;
    public Image m_DockButton;

    public UnityEvent m_OnSailEvent;
    public UnityEvent m_OnHeaveToEvent;
    public UnityEvent m_OnDockEvent;

    public void OnClicked()
    {
        if ((m_HeaveToButton.gameObject.activeSelf)
            || (m_DockButton.gameObject.activeSelf))
        {
            m_SailButton.gameObject.SetActive(true);
            m_DockButton.gameObject.SetActive(false);
            m_HeaveToButton.gameObject.SetActive(false);
            m_OnSailEvent.Invoke();
        }
        else if (m_SailButton.gameObject.activeSelf)
        {
            m_SailButton.gameObject.SetActive(false);
            m_DockButton.gameObject.SetActive(false);
            m_HeaveToButton.gameObject.SetActive(true);

            // should check to see if Dock is an option
            m_OnHeaveToEvent.Invoke();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
