using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI m_DateText;
    DateTime m_Date = new DateTime(1501, 1, 1);
    public TMP_InputField m_ShipNameInputField;

    public TextMeshProUGUI m_DoubloonsText;
    public TextMeshProUGUI m_TradeGoodsText;
    public TextMeshProUGUI m_ShipStoresText;

    public Button m_PrevShipButton;
    public Button m_NextShipButton;

    public ShipManager m_CurrentShip = null;
    int m_CurrentShipIndex = -1;

    public List<ShipManager> m_ShipManagers = new List<ShipManager>();

    int[] m_ShipLayer;
    public int m_NumShipLayers = 10;
    int m_AllShipLayers;
    public string m_ShipLayerNameFormat = "Ship{0}";
    Camera m_Camera = null;

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

        m_PrevShipButton.gameObject.SetActive(false);
        m_NextShipButton.gameObject.SetActive(false);

        m_ShipLayer = new int[m_NumShipLayers];
        for (int i = 0; i < m_NumShipLayers; i++)
        {
            string layerName = string.Format(m_ShipLayerNameFormat, i);
            m_ShipLayer[i] = LayerMask.NameToLayer(layerName);
            m_AllShipLayers = m_AllShipLayers | (1 << m_ShipLayer[i]);
        }
    }

    public void RegisterShipManager(ShipManager sm)
    {
        m_CurrentShip = sm;
        if (!m_ShipManagers.Contains(sm))
        {
            sm.SetShipLayer(m_ShipLayer[m_ShipManagers.Count]);
            sm.SetShipIndex(m_ShipManagers.Count);
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

    public void SetShipVisibility(int shipIdx, bool isVisible)
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
        if (shipIdx == -1)
        {
            if (!isVisible)
            {
                m_Camera.cullingMask = m_Camera.cullingMask & ~m_AllShipLayers;
            }
            else
            {
                m_Camera.cullingMask = m_Camera.cullingMask | m_AllShipLayers;
            }
        }
        else
        {
            if (!isVisible)
            {
                m_Camera.cullingMask = m_Camera.cullingMask & ~(1 << m_ShipLayer[shipIdx]);
            }
            else
            {
                m_Camera.cullingMask = m_Camera.cullingMask | (1 << m_ShipLayer[shipIdx]);
            }
        }
    }
    public void UnregisterShipManager(ShipManager sm)
    {
        if (m_CurrentShip == sm)
        {
            m_CurrentShip = null;
        }
        if (m_ShipManagers.Contains(sm))
        {
            m_ShipManagers.Remove(sm);
        }
    }
    void UpdateDateTimeDisplay()
    {
        string dtString = m_Date.ToString("dddd\nMMMM d, yyyy");
        m_DateText.text = dtString;
    }

    public void UpdateShipStoresDisplay()
    {
        if (m_CurrentShip == null)
        {
            m_ShipStoresText.text = "";
        }
        else
        {
            m_ShipStoresText.text = m_ShipManagers[0].GetShipStores().ToString();
        }
    }

    public void UpdateTradeGoodsDisplay()
    {
        if (m_CurrentShip == null)
        {
            m_TradeGoodsText.text = "";
        }
        else
        {
            m_TradeGoodsText.text = m_ShipManagers[0].GetTradeGoods().ToString();
        }
    }
    public void UpdateDoubloonsDisplay()
    {
        if (m_CurrentShip == null)
        {
            m_DoubloonsText.text = "";
        }
        else
        {
            m_DoubloonsText.text = m_ShipManagers[0].GetDoubloons().ToString();
        }
    }

    public void FocusOnShipName()
    {
        ModalCanvasController.Instance.ActivateDialog(ShipNameModalController.DIALOG_NAME, null, GetShipName());
    }

    public void SetShipNameDisplay(string name)
    {
        m_ShipNameInputField.text = name;
    }

    public void UpdateShipName(string name)
    {
        if (m_CurrentShip != null)
        {
            m_CurrentShip.SetShipName(name);
        }
    }

    public string GetShipName()
    {
        if (m_CurrentShip != null)
        {
            return m_CurrentShip.GetShipName();
        }
        else
        {
            return string.Empty;
        }
    }
    public void IncrementDay()
    {
        m_Date += TimeSpan.FromDays(1);
        UpdateDateTimeDisplay();
    }
    public void SetDate(int month, int day, int year)
    {
        m_Date = new DateTime(year, month, day);
        UpdateDateTimeDisplay();
    }

    public void StartNavigationPaths(Hex hex)
    {

    }

    void SetHexNameFromType(Hex hex)
    {
        switch (hex.GetHexSubType())
        {
            case Hex.HexSubType.Undefined:
                if (hex.GetIslandName() == string.Empty)
                {
                    NameHex(hex, "this new land");
                }
                break;
            case Hex.HexSubType.Home:
                NameHex(hex, "your home port");
                break;
            case Hex.HexSubType.Waystation:
                NameHex(hex, "this waystation");
                break;
            case Hex.HexSubType.Hazard:
                NameHex(hex, "this hazard");
                break;
            default:
                break;
        }
    }
    public bool HandleMovementEffect(ShipManager ship, Hex hex, NavigationController.HexMovementEffect effect)
    {
        if (hex == null)
        {
            Debug.LogError("Null hex");
            return true;
        }

        bool keepSailing = true;

        switch (effect)
        {
            case NavigationController.HexMovementEffect.Undef:
                break;
            case NavigationController.HexMovementEffect.None:
                break;
            case NavigationController.HexMovementEffect.Discovery:
                SetHexNameFromType(hex);
                keepSailing = false;
                break;
            case NavigationController.HexMovementEffect.Sink:
                SetHexNameFromType(hex);
                keepSailing = false;
                break;
            case NavigationController.HexMovementEffect.Dock:
                {
                    if (hex == HexMapBuilder.Instance.m_Home)
                    {
                        HexMapBuilder.Instance.MoveDiscoveredHexesFromListToKnownHexes(ship.GetDiscoveredHexes());
                        HexMapBuilder.Instance.RegenerateUnknownHexes();
                    }
                    else if ((hex.GetHexCenterPointType() == HexPoint.HexPointType.Land)
                        && (hex.m_HexVisibility == Hex.HexVisibility.Known)
                        && (hex.GetHexSubType() == Hex.HexSubType.Waystation))
                    {
                        HexMapBuilder.Instance.MoveDiscoveredHexesFromListToKnownHexes(ship.GetDiscoveredHexes());
                        HexMapBuilder.Instance.RegenerateUnknownHexes();
                    }
                    keepSailing = false;
                }
                break;
            default:
                break;
        }

        return keepSailing;
    }
    void Start()
    {
        UpdateDateTimeDisplay();
        for (int i = 0; i < m_ShipManagers.Count; i++)
        {
            NavigationController.Instance.StartPath(HexMapBuilder.Instance.m_Home, m_ShipManagers[i]);
        }
        if (m_ShipManagers.Count > 0)
        {
            m_CurrentShip = m_ShipManagers[0];
            m_CurrentShipIndex = 0;
            SetShipVisibility(0, true);
            if (m_ShipManagers.Count > 1)
            {
                m_NextShipButton.gameObject.SetActive(true);
            }

            UpdateShipStoresDisplay();
            SetShipNameDisplay(m_CurrentShip.GetShipName());

        }
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame()
    {
        FocusOnShipName();
        do
        {
            yield return new WaitForEndOfFrame();
        } while (ModalCanvasController.Instance.ModalCanvasActive());
        NameHex(HexMapBuilder.Instance.m_Home, "your home port");
    }

    public void NameHex(Hex hex, string message)
    {
        ModalCanvasController.Instance.ActivateDialog(HexNameModalController.DIALOG_NAME, SetHexNameCallback, hex, message);
    }

    void SetHexNameCallback(params object[] args)
    {
        if (args.Length < 2)
        {
            Debug.LogError("Invalid SetCurrentHexNameCallback");
            return;
        }
        Hex hex = args[0] as Hex;
        string name = args[1] as string;
        if (hex == null)
        {
            Debug.LogError("Invalid Null Hex SetCurrentHexNameCallback");
        }
        else
        {
            Hex.HexIndex hi = hex.m_ThisHexIndex;
            Debug.Log($"Set Hex Name hex [{hi.iy.ToString()}][{hi.ix.ToString()}] name \"{name}\"");
            hex.SetHexName(name);
        }
    }
    public void NextShip()
    {
        if (m_CurrentShip != null)
        {
            if (m_CurrentShipIndex < (m_ShipManagers.Count - 1))
            {
                SetShipVisibility(m_CurrentShipIndex, false);
                m_CurrentShipIndex++;
                m_CurrentShip = m_ShipManagers[m_CurrentShipIndex];
                SetShipVisibility(m_CurrentShipIndex, true);

                UpdateShipStoresDisplay();
                SetShipNameDisplay(m_CurrentShip.GetShipName());
            }
        }
        if (m_CurrentShipIndex >= (m_ShipManagers.Count - 1))
        {
            m_NextShipButton.gameObject.SetActive(false);
        }
        else
        {
            m_NextShipButton.gameObject.SetActive(true);
        }
        if (m_CurrentShipIndex > 0)
        {
            m_PrevShipButton.gameObject.SetActive(true);
        }
        else
        {
            m_PrevShipButton.gameObject.SetActive(false);
        }
    }
    public void PrevShip()
    {
        if (m_CurrentShip != null)
        {
            if (m_CurrentShipIndex > 0)
            {
                SetShipVisibility(m_CurrentShipIndex, false);
                m_CurrentShipIndex--;
                m_CurrentShip = m_ShipManagers[m_CurrentShipIndex];
                SetShipVisibility(m_CurrentShipIndex, true);

                UpdateShipStoresDisplay();
                SetShipNameDisplay(m_CurrentShip.GetShipName());
            }
        }
        if (m_CurrentShipIndex >= (m_ShipManagers.Count - 1))
        {
            m_NextShipButton.gameObject.SetActive(false);
        }
        else
        {
            m_NextShipButton.gameObject.SetActive(true);
        }
        if (m_CurrentShipIndex > 0)
        {
            m_PrevShipButton.gameObject.SetActive(true);
        }
        else
        {
            m_PrevShipButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
