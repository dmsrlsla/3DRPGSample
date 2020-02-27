using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsBossMonster : CsMonster
{
    public CsBossMonster(string monName, int nMaxhp, int nHp, int nattackDamage, Vector3 vtCreatePos, float flRotation, int nMonsterID) : base(monName, nMaxhp, nHp, nattackDamage, vtCreatePos, flRotation, nMonsterID)
    {
        m_strMonName = monName;
        m_MaxHp = nMaxhp;
        m_Hp = nHp;
        m_AttackDamage = nattackDamage;
        m_vtCreatePos = vtCreatePos;
        m_flRotationY = flRotation;
        m_nMonsterID = nMonsterID;
    }

    private void Awake()
    {
        CsGameData.Instance.BossMonster = transform;
    }

    private void OnDestroy()
    {

    }

    void Start()
    {
        base.Start();
        OnAnimEffectApear();
    }

    void OnAnimEffectApear()
    {
        StartCoroutine(StartBossApear());
    }

    IEnumerator StartBossApear()
    {
        yield return new WaitForSeconds(5.0f);
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[5].name, transform, transform.position, transform.rotation, 0, 5);
        CsGameEvent.Instance.OnEventBossMonsterApear();
    }

    protected override IEnumerator DeadStart()
    {
        m_MonAnimator.SetTrigger("Dead");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        CsGameEvent.Instance.OnEventStageClearUI();
        CsGameEvent.Instance.OnEventStageClear();
    }

    public void BossRoar(bool bActive)
    {
        m_MonAnimator.SetBool("Roar", bActive);
    }
}
