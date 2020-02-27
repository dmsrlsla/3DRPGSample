using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsGateObject : MonoBehaviour
{
    [SerializeField]
    int m_nGateNum;

    private void Awake()
    {
        CsGameEvent.Instance.EventDestroyGate += OnEventDestroyGate;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventDestroyGate += OnEventDestroyGate;
    }

    void OnEventDestroyGate(int nGatenum)
    {
        if(m_nGateNum == nGatenum)
        {
            gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
