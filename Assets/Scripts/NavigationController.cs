using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    static NavigationController m_instance;
    public static NavigationController Instance
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
    public enum NavigationState { Undef, Plotting, Sailing };
    public NavigationState m_NavigationState = NavigationState.Undef;

    public ShipManager m_ShipManager;

    public GameObject m_HexPathPrefab;

    public float m_SailDwellTime = 1.0f;

    public class PathEntry
    {
        public Hex hex;
        public GameObject marker;
    }
    //List<PathEntry> m_Path;
    //public List<PathEntry> GetCurrentShipPath()
    //{
    //    if (GameController.Instance.m_CurrentShip == null)
    //    {
    //        return null;
    //    }
    //    return GameController.Instance.m_CurrentShip.GetPath(); 
    //}

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartPath(Hex hex, ShipManager shipManager)
    {
        //ClearPath(GameController.Instance.m_CurrentShip);
        //AddHexToPath(hex);
        ClearPath(shipManager);
        AddHexToPath(hex, shipManager);
        
    }
    bool HexIsValid(Hex hex)
    {
        if (GameController.Instance.m_CurrentShip == null)
        {
            return false;
        }
        List<NavigationController.PathEntry> path = GameController.Instance.m_CurrentShip.GetPath();
        if (hex == null)
        {
            return false;
        }    
        if (path.Count == 0)
        {
            return true;
        }
        Hex lastHex = path[path.Count - 1].hex;
        if (hex == lastHex)
        {
            return false;
        }
        for (int i = 0; i < lastHex.m_Neighbor.Length; i++)
        {
            if (hex == lastHex.m_Neighbor[i])
            {
                return true;
            }
        }
        return false;
    }
    void AddHexToPath(Hex hex, ShipManager shipManager)
    {
        if (shipManager == null)
        {
            return;
        }
        Transform parent = shipManager.transform;
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        if (HexIsValid(hex))
        {
            GameObject go = Instantiate(m_HexPathPrefab);
            go.layer = shipManager.m_ShipLayer;
            for (int j = 0; j < go.transform.childCount; j++)
            {
                go.transform.GetChild(j).gameObject.layer = go.layer;
            }
            HexPathController hpc = go.GetComponent<HexPathController>();
            if (hpc == null)
            {
                Debug.LogError("No HexPathController on HexPathPrefab");
                return;
            }
            go.transform.position = hex.GetHexPosition();
            go.transform.SetParent(parent, false);
            go.name = "path_" + (path.Count + 1).ToString();
            PathEntry entry = new PathEntry();
            entry.hex = hex;
            entry.marker = go;
            path.Add(entry);
            hpc.SetLabel(path.Count.ToString());
        }
    }
    public void ClearPathSteps(ShipManager shipManager, int iNumberOfSteps)
    {
        if (shipManager == null)
        {
            return;
        }
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        int start = Mathf.Min(iNumberOfSteps, path.Count);
        for (int i = start - 1; i >= 0; i--)
        {
            Destroy(path[i].marker);
        }
        path.RemoveRange(0, iNumberOfSteps); 
    }
    public void ClearPath(ShipManager shipManager)
    {
        if (shipManager == null)
        {
            return;
        }
        List<NavigationController.PathEntry> path = shipManager.GetPath();
        for (int i = path.Count - 1; i >= 0 ; i--)
        {
            Destroy(path[i].marker);
        }
        path.Clear();
    }

    public void Sail()
    {
        StartCoroutine(SailCoroutine());
    }
    IEnumerator SailCoroutine()
    {
        bool keepSailing = true;
        int iDayCount = 0;
        while (keepSailing)
        {
            Hex hex = null;
            if (iDayCount > 0)
            {
                GameController.Instance.IncrementDay();
            }
            for (int iShipIndex = 0; iShipIndex < GameController.Instance.m_ShipManagers.Count; iShipIndex++)
            {
                List<PathEntry> path = GameController.Instance.m_ShipManagers[iShipIndex].GetPath();
                if (path == null)
                {
                    keepSailing = false;
                    yield break;
                }
                hex = path[0].hex;
                hex.SetHexVisibility(Hex.HexVisibility.Discovered);
                if (path.Count == 1)
                {
                    ClearPath(GameController.Instance.m_ShipManagers[iShipIndex]);
                    if (hex != null)
                    {
                        AddHexToPath(hex, GameController.Instance.m_ShipManagers[iShipIndex]);
                    }
                    keepSailing = false;
                }
                else
                {
                    ClearPathSteps(GameController.Instance.m_ShipManagers[iShipIndex], 1);
                }
            }
            if (iDayCount > 0)
            {
                yield return new WaitForSeconds(m_SailDwellTime);
            }
            iDayCount++;
        }
    }
    public void AddToCurrentShipPath()
    {
        if (GameController.Instance.m_CurrentShip != null)
        {
            AddToPath(GameController.Instance.m_CurrentShip);
        }
    }
    public void AddToPath(ShipManager shipManager)
    {
        if (HexMapBuilder.Instance == null)
        {
            return;
        }

        Hex hex = HexMapBuilder.Instance.FetchHexFromClickPoint();
        if (hex != null)
        {
            AddHexToPath(hex, shipManager);
        }
    }
}
