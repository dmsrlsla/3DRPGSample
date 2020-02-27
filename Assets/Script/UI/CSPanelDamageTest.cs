using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSPanelDamageTest : MonoBehaviour
{
    private void Awake()
    {
        CsGameEvent.Instance.EventDamageText += OnEventDamageText;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventDamageText -= OnEventDamageText;
    }

    void OnEventDamageText(int nDamage)
    {
        GameObject goDamageText = Instantiate(Resources.Load("Prefab/UI/TextDamage"),transform) as GameObject;
        goDamageText.GetComponent<CsDamageText>().SetText(nDamage);
    }
}
