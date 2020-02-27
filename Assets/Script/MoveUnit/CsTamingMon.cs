using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CsTamingMon : CsMonster
{
    #region 생성자 관련
    bool m_bTaming = false;

    [SerializeField]
    Transform m_trDragonHead;

    List<CsMonster> m_listTargetMonster;

    public CsTamingMon(string monName, int nMaxhp, int nHp, int nattackDamage, Vector3 vtCreatePos, float flRotation, int nMonsterID) : base (monName, nMaxhp, nHp, nattackDamage, vtCreatePos, flRotation, nMonsterID)
    {
        m_strMonName = monName;
        m_MaxHp = nMaxhp;
        m_Hp = nHp;
        m_AttackDamage = nattackDamage;
        m_vtCreatePos = vtCreatePos;
        m_flRotationY = flRotation;
        m_nMonsterID = nMonsterID;
    }
    #endregion
    private void Awake()
    {
        CsGameEvent.Instance.EventTamingMonsterAttack += OnEventTamingMonsterAttack;
        CsGameEvent.Instance.EventTamingMonsterDestroy += OnEventTamingMonsterDestroy;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventTamingMonsterAttack -= OnEventTamingMonsterAttack;
        CsGameEvent.Instance.EventTamingMonsterDestroy -= OnEventTamingMonsterDestroy;
    }



    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        m_bTaming = false;
        m_listTargetMonster = new List<CsMonster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bTaming)
        {
            if (m_enMonState != EnMonsterState.Taming)
            {
                base.Update();
            }
            else
            {
                m_MonAnimator.SetBool("Groogy", true);
            }
        }
    }

    public override void ApplyDamage(int nDamage)
    {
        if (m_enMonState == EnMonsterState.Taming) return;
        Hp -= nDamage;

        if (Hp <= 0)
        {
            m_enMonState = EnMonsterState.Taming;
            m_MonAnimator.SetBool("Groogy",true);
            CsGameEvent.Instance.OnEventTamingMonsterGetButton(true);
            CsGameData.Instance.TamingMon = transform;
        }
    }

    void OnEventTamingMonsterDestroy()
    {
        StartCoroutine(DeadStart());
    }

    void OnEventTamingMonsterAttack()
    {
        //m_enMonState = EnMonsterState.Attack;
        m_MonAnimator.SetInteger("State", (int)EnMonsterState.Attack);
    }

    protected override IEnumerator DeadStart()
    {
        m_MonAnimator.SetTrigger("Dead");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void MoveTeleport(Vector3 vtMove)
    {
        m_navMesh.enabled = false;
        transform.position = vtMove;
        m_navMesh.enabled = true;
    }

    public void ComplateTaming()
    {
        m_bTaming = true;
        m_MonAnimator.SetBool("Groogy", false);
        m_MonAnimator.SetInteger("State", 0);
        m_navMesh.enabled = false;
    }

    void OnAnimEffectFire()
    {
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[6].name, transform, transform.position + transform.forward * 3, transform.rotation, 0, 2);
    }

    void OnAnimTamingAttack()
    {
        Collider[] CheckCollider = Physics.OverlapSphere(transform.position + transform.forward * 2, 10f);
        foreach (Collider col in CheckCollider)
        {
            if (col.tag == "Monster")
            {
                if (col.GetComponent<CsMonster>() != null)
                {
                    m_listTargetMonster.Add(col.GetComponent<CsMonster>());
                }
            }
        }
    }

    void OnAnimApplyDamage()
    {
        foreach (CsMonster Monster in m_listTargetMonster)
        {
            int RanDamage = UnityEngine.Random.Range(80, 120);
            Monster.ApplyDamage(RanDamage);
            CsGameEvent.Instance.OnEventDamageText(RanDamage);
        }
        m_listTargetMonster.Clear();
    }


    void OnAnimEvnetAttackEnd()
    {
        if (m_bTaming)
        {
            Debug.LogError("멈춤");
            m_MonAnimator.SetInteger("State", (int)EnMonsterState.Idle);
            m_enMonState = EnMonsterState.Idle;
        }
    }
}
