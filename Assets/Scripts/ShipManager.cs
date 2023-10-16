using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public int m_ShipStores = 10;
    public string m_ShipName = "";

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

    void Start()
    {
        GameController.Instance.RegisterShipManager(this);
    }

    void Update()
    {
        
    }
}
