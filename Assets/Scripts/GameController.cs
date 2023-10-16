using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI m_DateText;
    DateTime m_Date = new DateTime(1501, 1, 1);
    public TMP_InputField m_ShipNameInputField;

    public TextMeshProUGUI m_ShipStoresText;

    public List<ShipManager> m_ShipManagers = new List<ShipManager>();

    static GameController m_instance;
    public static GameController Instance
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
    }

    public void RegisterShipManager(ShipManager sm)
    {
        if (!m_ShipManagers.Contains(sm))
        {
            m_ShipManagers.Add(sm);
        }
        if (string.IsNullOrEmpty(sm.GetShipName()))
        {
            sm.SetShipName(m_ShipNameInputField.text);
        }
        else
        {
            SetShipNameDisplay(sm.GetShipName());
        }
        UpdateShipStoresDisplay();

    }
    public void UnregisterShipManager(ShipManager sm)
    {
        if (m_ShipManagers.Contains(sm))
        {
            m_ShipManagers.Remove(sm);
        }
    }
    void UpdateDateTimeDisplay()
    {
        string dtString = m_Date.ToString("dddd, MMMM d, yyyy");
        m_DateText.text = dtString;
    }

    void UpdateShipStoresDisplay()
    {
        if (m_ShipManagers.Count > 0)
        {
            m_ShipStoresText.text = m_ShipManagers[0].GetShipStores().ToString();
        }
    }

    void SetShipNameDisplay(string name)
    {
        m_ShipNameInputField.text = name;
    }

    public void UpdateShipName(string name)
    {
        if (m_ShipManagers.Count > 0)
        {
            m_ShipManagers[0].SetShipName(name);
        }
    }

    void Start()
    {
        UpdateDateTimeDisplay();
        UpdateShipStoresDisplay();
    }

    void Update()
    {
        
    }
}
