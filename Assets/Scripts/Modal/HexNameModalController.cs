using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HexNameModalController : MonoBehaviour, IModalDialog
{
    public const string DIALOG_NAME = "SetHexName";
    string m_Format = "Enter name for {0}";

    public TextMeshProUGUI m_SetHexInfoText;
    public TMP_InputField m_HexNameInputField;
    public TextMeshProUGUI m_PlaceholderText;
    bool m_IsTitleHex = false;
    public Button m_ContinueButton;

    UnityAction<object[]> m_Callback;
    Hex m_Hex = null;
    int m_HexPointIndex = -1;

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
        if (callback == null)
        {
            Debug.LogError("Can't have NULL callback");
            return false;
        }
        m_Callback = callback;

        m_Hex = null;
        string label = string.Empty;
        string name = string.Empty;
        if (args.Length > 0)
        {
            m_Hex = args[0] as Hex;
        }
        if (args.Length > 1)
        {
            label = args[1] as string;
        }

        bool? isTitleHex = null;
        if (args.Length > 2)
        {
            isTitleHex = args[2] as bool?;
        }
        m_SetHexInfoText.text = string.Format(m_Format, label);
        m_PlaceholderText.text = label;
        m_HexNameInputField.text = name;
        m_IsTitleHex = (isTitleHex == null ? false : isTitleHex.Value);

        return true;
    }
    public void OnContinue()
    {
        object[] args = new object[3];
        args[0] = m_Hex;
        args[1] = m_HexNameInputField.text;
        args[2] = m_IsTitleHex;
        Debug.Log($"ABOUT to callback with \"{m_HexNameInputField.text}\"");
        m_Callback(args);
    }

    public void OnTextChanged(string text)
    {
        m_HexNameInputField.text = text;
        if (m_HexNameInputField.text.Length > 0)
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
