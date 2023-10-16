using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public int m_ShipStores = 10;
    public string m_ShipName = "";
    public int m_ShipLayer;
    public int m_ShipIndex;

    List<NavigationController.PathEntry> m_Path;
    public List<NavigationController.PathEntry> GetPath()
    {
        if (m_Path == null)
        {
            m_Path = new List<NavigationController.PathEntry>();
        }
        return m_Path;
    }

    public void SetShipIndex(int idx)
    {
        m_ShipIndex = idx;
    }
    public void SetShipLayer(int layer)
    {
        m_ShipLayer = layer; 
        gameObject.layer = layer;
    }
    public int GetShipStores()
    {
        return m_ShipStores;
    }

    public void SetShipStores(int stores)
    {
        m_ShipStores = stores;
    }

    public string GetShipName()
    {
        return (m_ShipName);
    }

    public void SetShipName(string name)
    {
        m_ShipName = name;
    }

    private void Awake()
    {
        GameController.Instance.RegisterShipManager(this);
    }
    void Start()
    {

    }

    void Update()
    {
        
    }
}
