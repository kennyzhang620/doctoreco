using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class EventTrigger_C : MonoBehaviour
{
    // Start is called before the first frame update
    [Serializable]
    public class TriggerGB : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("OnTrigger")]
    [SerializeField]
    public TriggerGB m_TGB = new TriggerGB();

    [Serializable]
    public class UIGB : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("OnUI")]
    [SerializeField]
    private UIGB m_UIOnly = new UIGB();
    public bool onStart = false;

    void OnEnable()
    {
        if (onStart) { m_TGB.Invoke(); }
    }
    public void Trigger()
    {
        m_TGB.Invoke();
        if (enabled)
        {
            m_UIOnly.Invoke();
        }
    }

    public void TriggerTGB()
    {
        m_TGB.Invoke();
    }

    public void TriggerUI()
    {
            m_UIOnly.Invoke();
    }
}