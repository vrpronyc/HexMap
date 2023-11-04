using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    [System.Serializable]
    public enum ShipMovementType { Sailing, Docked, Anchored, HeavedTo };

    public int m_ShipStores = 10;
    public int m_Doubloons = 0;
    public int m_TradeGoods = 0;
    public string m_ShipName = "";
    public int m_ShipLayer;
    public int m_ShipIndex;

    ShipMovementType m_MovementType = ShipMovementType.Sailing;
    public ShipMovementType MovementType
    {
        set
        {
            m_MovementType = value;
        }
        get
        {
            return m_MovementType;
        }
    }

    List<Hex> m_DiscoveredHexes;

    List<NavigationController.PathEntry> m_Path;
    public List<NavigationController.PathEntry> GetPath()
    {
        if (m_Path == null)
        {
            m_Path = new List<NavigationController.PathEntry>();
        }
        return m_Path;
    }

    public void AddToDiscoveredHexes(Hex hex)
    {
        if (m_DiscoveredHexes == null)
        {
            m_DiscoveredHexes = new List<Hex>();
        }
        m_DiscoveredHexes.Add(hex);
    }

    public List<Hex> GetDiscoveredHexes()
    {
        return m_DiscoveredHexes;
    }

    public void ClearDiscoveredHexes()
    {
        if (m_DiscoveredHexes == null)
        {
            m_DiscoveredHexes = new List<Hex>();
        }
        m_DiscoveredHexes.Clear();
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
    public int GetDoubloons()
    {
        return m_Doubloons;
    }
    public int GetTradeGoods()
    {
        return m_TradeGoods;
    }
    public int DepleteStores(int amount)
    {
        m_ShipStores = Mathf.Max(m_ShipStores - 1, 0);
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
