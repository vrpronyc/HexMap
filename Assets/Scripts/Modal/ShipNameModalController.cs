using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ShipNameModalController : MonoBehaviour, IModalDialog
{
    public const string DIALOG_NAME = "SetShipName";
    public TMP_InputField m_ShipNameInputField;
    public Button m_ContinueButton;

    public string GetDialogName()
    {
        return DIALOG_NAME;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool Initialize(UnityAction<object[]> callback, params object[] args)
    {
        string name = string.Empty;
        if (args.Length > 0)
        {
            name = args[0] as string;
        }

        m_ShipNameInputField.text = name;
        return true;
    }
    public void OnContinue()
    {
        GameController.Instance.SetShipNameDisplay(m_ShipNameInputField.text);
    }

    public void OnTextChanged(string text)
    {
        m_ShipNameInputField.text = text;
        if(m_ShipNameInputField.text.Length > 0)
        {
            if (!m_ContinueButton.interactable)
            {
                m_ContinueButton.interactable = true;
            }
        }
        else
        {
            if (m_ContinueButton.interactable)
            {
                m_ContinueButton.interactable = false;
            }
        }
    }
}
