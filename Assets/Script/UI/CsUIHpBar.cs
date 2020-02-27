using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CsUIHpBar : MonoBehaviour
{
    Slider m_HpSlider;

    private void Awake()
    {
        CsGameEvent.Instance.EventHpSet += SetSlider;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventHpSet -= SetSlider;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_HpSlider = transform.GetComponent<Slider>();
    }

    public void SetSlider(int nCurrentHp, int nMaxHp) // 현재 HP/최대 HP로 백분률을 만듬.
    {
        m_HpSlider.value = nCurrentHp / nMaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
