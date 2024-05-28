using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressController : MonoBehaviour
{
    public GameObject m_RadialProgressBar;
    public Image m_ProgressImage;

    void Start()
    {
        m_RadialProgressBar.SetActive(false);
        m_ProgressImage.fillAmount = 0;
    }

    public void SetProgressVisible(bool isVisible)
    {
        //Debug.Log("SetProgressVisible " + isVisible.ToString());
        if (isVisible && !m_RadialProgressBar.activeSelf)
            m_RadialProgressBar.SetActive(true);
        else if (!isVisible && m_RadialProgressBar.activeSelf)
            m_RadialProgressBar.SetActive(false);
    }

    public void SetProgress (float val)
    {
        //Debug.Log("SetProgress " + val.ToString("R"));
        m_ProgressImage.fillAmount = val;
    }
}
