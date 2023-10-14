using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public enum NavigationState { Undef, Plotting, Sailing };
    public NavigationState m_NavigationState = NavigationState.Undef;

    public ShipManager m_ShipManager;

    public GameObject m_HexPathPrefab;

    public class PathEntry
    {
        public Hex hex;
        public GameObject marker;
    }
    List<PathEntry> m_Path;
    public List<PathEntry> Path
    {
        get
        {
            if (m_Path == null)
            {
                m_Path = new List<PathEntry>();
            }
            return m_Path;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    bool HexIsValid(Hex hex)
    {
        if (hex == null)
        {
            return false;
        }    
        if (Path.Count == 0)
        {
            return true;
        }
        Hex lastHex = Path[Path.Count - 1].hex;
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
    void AddHexToPath(Hex hex)
    {
        if (HexIsValid(hex))
        {
            GameObject go = Instantiate(m_HexPathPrefab);
            HexPathController hpc = go.GetComponent<HexPathController>();
            if (hpc == null)
            {
                Debug.LogError("No HexPathController on HexPathPrefab");
                return;
            }
            go.transform.position = hex.GetHexPosition();
            go.transform.SetParent(transform, false);
            go.name = "path_" + (Path.Count + 1).ToString();
            PathEntry entry = new PathEntry();
            entry.hex = hex;
            entry.marker = go;
            Path.Add(entry);
            hpc.SetLabel(Path.Count.ToString());
        }
    }
    public void ClearPath()
    {
        for (int i = Path.Count - 1; i >= 0 ; i--)
        {
            Destroy(m_Path[i].marker);
        }
        Path.Clear();
    }

    public void Sail()
    {
        StartCoroutine(SailCoroutine());
    }
    IEnumerator SailCoroutine()
    {
        Hex hex = null;
        for (int i = 0; i < Path.Count; i++)
        {
            hex = Path[i].hex;
            Path[i].hex.SetHexVisibility(Hex.HexVisibility.Known);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        ClearPath();
        if (hex != null)
        {
            AddHexToPath(hex);
        }
    }
    public void AddToPath()
    {
        if (HexMapBuilder.Instance == null)
        {
            return;
        }

        Hex hex = HexMapBuilder.Instance.FetchHexFromClickPoint();
        if (hex != null)
        {
            AddHexToPath(hex);
        }
    }
}
