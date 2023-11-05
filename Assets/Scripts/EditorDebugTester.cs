using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorDebugTester : MonoBehaviour
{
    public UnityEvent TestFunction;
    public virtual void Click()
    {
        if (Application.isPlaying)
        {
            TestFunction.Invoke();
        }
    }
}
