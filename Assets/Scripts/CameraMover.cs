using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMover : MonoBehaviour
{
    public enum MoveEvent { None, CamLeft, CamRight, CamUp, CamDown, CamIn, CamOut};
    MoveEvent m_CurrentMoveEvent = MoveEvent.None;
    public float m_FirstMoveDwellTime = 1.0f;
    public float m_MoveDwellTime = 0.25f;
    float m_StartDwellTime;
    float m_NextDwellTime;

    static CameraMover m_instance;
    public static CameraMover Instance
    {
        get
        {
            return m_instance;
        }
    }

    public Transform m_CameraTransform;
    public Transform m_CameraLookAtTransform;
    public float m_InOutStep = 1.0f;
    public float m_MaxIn = -4.0f;
    public float m_2DStep = 1.0f;

    int m_UILayer;

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

        m_UILayer = LayerMask.NameToLayer("UI");
    }

    void UpdateEventAndDwellTime(MoveEvent current)
    {
        if (m_CurrentMoveEvent != current)
        {
            m_StartDwellTime = Time.time;
            m_NextDwellTime = m_StartDwellTime + m_FirstMoveDwellTime;
        }
        else
        {
            m_StartDwellTime = m_NextDwellTime;
            m_NextDwellTime = m_StartDwellTime + m_MoveDwellTime;
        }
        m_CurrentMoveEvent = current;
    }

    void MoveCamera2D(Vector2 delta)
    {
        m_CameraLookAtTransform.position = m_CameraLookAtTransform.position + new Vector3(delta.x, delta.y, 0);
    }

    void MoveCameraInOut(float delta)
    {
        Vector3 pos = m_CameraTransform.position + new Vector3(0, 0, delta);
        if (pos.z < m_MaxIn)
        {
            m_CameraTransform.position = m_CameraTransform.position + new Vector3(0, 0, delta);
        }
    }
    public void MoveCameraRight()
    {
        UpdateEventAndDwellTime(MoveEvent.CamRight);
        Debug.Log("MoveCameraRight");
        MoveCamera2D(new Vector2(m_2DStep, 0));
    }
    public void MoveCameraLeft()
    {
        UpdateEventAndDwellTime(MoveEvent.CamLeft);
        Debug.Log("MoveCameraLeft");
        MoveCamera2D(new Vector2(-m_2DStep, 0));
    }
    public void MoveCameraUp()
    {
        UpdateEventAndDwellTime(MoveEvent.CamUp);
        Debug.Log("MoveCameraUp");
        MoveCamera2D(new Vector2(0, m_2DStep));
    }
    public void MoveCameraDown()
    {
        UpdateEventAndDwellTime(MoveEvent.CamDown);
        Debug.Log("MoveCameraDown");
        MoveCamera2D(new Vector2(0, -m_2DStep));
    }
    public void MoveCameraIn()
    {
        UpdateEventAndDwellTime(MoveEvent.CamIn);
        Debug.Log("MoveCameraIn");
        MoveCameraInOut(m_InOutStep);
    }
    public void MoveCameraOut()
    {
        UpdateEventAndDwellTime(MoveEvent.CamOut);
        Debug.Log("MoveCameraOut");
        MoveCameraInOut(-m_InOutStep);
    }
    void Start()
    {
        
    }

    void Update()
    {
        float t = Time.time;
        if (Input.GetMouseButtonUp(0))
        {
            m_CurrentMoveEvent = MoveEvent.None;
        }
        else
        {
            if (t > m_NextDwellTime)
            {
                switch (m_CurrentMoveEvent)
                {
                    case MoveEvent.None:
                        break;
                    case MoveEvent.CamLeft:
                        MoveCameraLeft();
                        break;
                    case MoveEvent.CamRight:
                        MoveCameraRight();
                        break;
                    case MoveEvent.CamUp:
                        MoveCameraUp();
                        break;
                    case MoveEvent.CamDown:
                        MoveCameraDown();
                        break;
                    case MoveEvent.CamIn:
                        MoveCameraIn();
                        break;
                    case MoveEvent.CamOut:
                        MoveCameraOut();
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == m_UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
