using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionButton { Skill0 = 0, Skill1, Skill2, Skill3, Skill4 }

public class CsMainUI : MonoBehaviour
{
    Transform m_trActionButton;
    Transform m_trTamingButton;
    Transform m_trBossUI;
    Transform m_trJoyStick;
    Transform m_trStageClear;

    List<Transform> m_listButtoSkill = new List<Transform>();

    // Start is called before the first frame update
    void Awake()
    {
        m_trActionButton = transform.Find("AttackButton");
        m_trTamingButton = transform.Find("PanelTaming");
        m_trBossUI = transform.Find("PanelBossAppear");
        m_trJoyStick = transform.Find("JoyBackGround");
        m_trStageClear = transform.Find("PanelClear");
        for (int i = 0; i < m_trActionButton.childCount - 1; i++)
        {
            Transform trTemp = m_trActionButton.GetChild(i);
            Button btSkill = trTemp.GetComponent<Button>();

            btSkill.onClick.RemoveAllListeners();
            btSkill.onClick.AddListener(() => OnClickEventActionButton(0));
        }

        Button btTaming = m_trTamingButton.Find("Button").GetComponent<Button>();

        btTaming.onClick.RemoveAllListeners();
        btTaming.onClick.AddListener(() => OnClickEventTamingButton());

        m_trTamingButton.gameObject.SetActive(false);
        m_trBossUI.gameObject.SetActive(false);
        m_trStageClear.gameObject.SetActive(false);
        CsGameEvent.Instance.EventTamingMonsterGetButton += OnEventTamingMonsterGetButton;
        CsGameEvent.Instance.EventBossApearUI += OnEventBossApearUI;
        CsGameEvent.Instance.EventStageClearUI += OnEventStageClearUI;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventTamingMonsterGetButton -= OnEventTamingMonsterGetButton;
        CsGameEvent.Instance.EventBossApearUI -= OnEventBossApearUI;
        CsGameEvent.Instance.EventStageClearUI -= OnEventStageClearUI;
    }

    void OnEventTamingMonsterGetButton(bool bActive)
    {
        m_trTamingButton.gameObject.SetActive(bActive);
    }

    void OnClickEventActionButton(int nIndex)
    {
        CsGameEvent.Instance.OnEventUseSkill(nIndex);
        CsGameEvent.Instance.OnEventChangeState(EnState.Attack);
    }

    void OnClickEventTamingButton()
    {
        Debug.Log("테이밍 시작");
        CsGameEvent.Instance.OnEventTaming();
    }

    void OnEventBossApearUI()
    {
        StartCoroutine(BossApearUI());
    }

    void OnEventStageClearUI()
    {
        m_trStageClear.gameObject.SetActive(true);
    }

    IEnumerator BossApearUI()
    {
        m_trBossUI.gameObject.SetActive(true);
        m_trActionButton.gameObject.SetActive(false);
        m_trJoyStick.gameObject.SetActive(false);
        yield return new WaitForSeconds(4.0f);
        m_trBossUI.gameObject.SetActive(false);
        m_trActionButton.gameObject.SetActive(true);
        m_trJoyStick.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
