using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnMonsterState { Idle = 0, Attack, Chase, Walk, Return, Damage, Dead, Stend, Falling, Taming, Groogy } // 보스 등장 상태(스탠드)추가
public enum EnMonsterInstanceType { Normal, Boss, Money }

public class CsMonster : MonoBehaviour
{
    public enum EnOwnership { None = 0, Controller = 1, Target = 2 }

    protected string m_strMonName;

    protected int m_MaxHp;
    protected int m_Hp;
    protected int m_AttackDamage;
    protected int m_nMonsterID;
    protected float m_flRotationY;
    protected Vector3 m_vtCreatePos;
    protected Animator m_MonAnimator;
    protected NavMeshAgent m_navMesh;
    protected Transform m_Player;
    protected Slider m_HUDSlider;
    protected Coroutine m_DeadIe;

    protected EnMonsterInstanceType m_enMonType;
    [SerializeField]
    protected EnMonsterState m_enMonState;
    [SerializeField]
    protected float m_flChaseDistance;

    protected List<CsMonster> m_listTargetMonster = new List<CsMonster>();

    public string MonName { get { return m_strMonName; } set { m_strMonName = value; } }
    public int MaxHP { get { return m_MaxHp; } set { m_MaxHp = value; } }
    public int Hp { get { return m_Hp; } set { m_Hp = value; } }
    public int AttackDamage { get { return m_AttackDamage; } set { m_AttackDamage = value; } }
    public Vector3 CreatePos { get { return m_vtCreatePos; } set { m_vtCreatePos = value; } }
    public float RotationY { get { return m_flRotationY; } set { m_flRotationY = value; } }
    public int MonsterID { get { return m_nMonsterID; } set { m_nMonsterID = value; } }

    public CsMonster(string monName, int nMaxhp, int nHp, int nattackDamage, Vector3 vtCreatePos, float flRotation, int nMonsterID)
    {
        m_strMonName = monName;
        m_MaxHp = nMaxhp;
        m_Hp = nHp;
        m_AttackDamage = nattackDamage;
        m_vtCreatePos = vtCreatePos;
        m_flRotationY = flRotation;
        m_nMonsterID = nMonsterID;
    }

    public void InitMonster(string monName, int nMaxhp, int nHp, int nattackDamage, Vector3 vtCreatePos, float flRotation, int nMonsterID)
    {
        m_strMonName = monName;
        m_MaxHp = nMaxhp;
        m_Hp = nHp;
        m_AttackDamage = nattackDamage;
        m_vtCreatePos = vtCreatePos;
        m_flRotationY = flRotation;
        m_nMonsterID = nMonsterID;
    }


    // Start is called before the first frame update
    protected void Start()
    {
        m_MonAnimator = gameObject.GetComponent<Animator>();
        m_navMesh = gameObject.GetComponent<NavMeshAgent>();
        m_enMonState = EnMonsterState.Idle;
        m_Player = CsGameData.Instance.MyHeroTransform;
        m_HUDSlider = transform.Find("HUDPos").GetComponentInChildren<Slider>();
        //m_flChaseDistance = 10;
    }

    // Update is called once per frame
    protected void Update()
    {
        switch(m_enMonState)
        {
            case EnMonsterState.Idle:
                MonIdleState();
                break;
            case EnMonsterState.Attack:
                MonAttackState();
                break;
            case EnMonsterState.Chase:
                MonChaseState();
                break;
            case EnMonsterState.Return:
                MonReturnState();
                break;
            case EnMonsterState.Dead:
                MonDeadState();
                break;
        }
        m_HUDSlider.value = (float)Hp / (float)MaxHP;
    }

    protected void MonIdleState()
    {
        m_MonAnimator.SetInteger("State", (int)EnMonsterState.Idle);
        if (Vector3.Distance(m_Player.position, transform.position) < m_flChaseDistance) // 반경 10미터 안에 들어왔으면
        {
            m_enMonState = EnMonsterState.Chase;
        }
    }

    protected void MonAttackState()
    {
        m_MonAnimator.SetInteger("State", (int)EnMonsterState.Attack);
        if (Vector3.Distance(m_Player.position, transform.position) > 2.5f) // 공격후, 1미터 밖으로 벗어났으면
        {
            m_enMonState = EnMonsterState.Chase; // 추적상태로 변경
        }
    }

    protected void MonChaseState()
    {
        m_MonAnimator.SetInteger("State", (int)EnMonsterState.Walk);
        if (Vector3.Distance(m_Player.position,transform.position) > 1f
            && Vector3.Distance(m_Player.position, transform.position) < m_flChaseDistance
            && Vector3.Distance(m_vtCreatePos, transform.position) < m_flChaseDistance) // 1미터 안에 올때까지 추적상태 유지
        {
            m_navMesh.SetDestination(m_Player.position); // 플레이어 위치 갱신
            m_enMonState = EnMonsterState.Chase;
        }
        else if(Vector3.Distance(m_Player.position, transform.position) < 2.5f)
        {
            m_navMesh.ResetPath();
            m_enMonState = EnMonsterState.Attack;
        }

        if (Vector3.Distance(m_vtCreatePos, transform.position) > m_flChaseDistance) // 추적중 스타트 지점에서 10미터 이상 벌어졌다면
        {
            m_navMesh.SetDestination(m_vtCreatePos);
            m_enMonState = EnMonsterState.Return;
        }
    }

    protected void MonReturnState()
    {
        if (Vector3.Distance(m_vtCreatePos, transform.position) >= 0.5f) // 추적중 스타트 지점에서 10미터 이상 벌어졌다면
        {
            m_navMesh.SetDestination(m_vtCreatePos);
            m_enMonState = EnMonsterState.Return;
        }
        else if (Vector3.Distance(m_vtCreatePos, transform.position) < 0.5f)
        {
            m_navMesh.ResetPath();
            m_enMonState = EnMonsterState.Idle;
        }
    }

    protected void MonDeadState()
    {
        if(m_DeadIe == null)
        {
            m_DeadIe = StartCoroutine(DeadStart());
        }
    }

    protected virtual IEnumerator DeadStart()
    {
        m_MonAnimator.SetTrigger("Dead");
        CsGameEvent.Instance.OnEventKillMonster();
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    //void MonReturnState()
    //{
    //    m_MonAnimator.SetInteger("State", (int)EnMonsterState.Walk);
    //    if (Vector3.Distance(m_vtCreatePos, transform.position) < 0.1f) // 원래 자리로 돌아왔다면
    //    {
    //        m_navMesh.ResetPath();
    //        m_enMonState = EnMonsterState.Idle;
    //    }
    //    else if (Vector3.Distance(m_Player.position, transform.position) < 0.5f) // 플레이어가 쫒아왔다면
    //    {
    //        Debug.LogError("재추적 : " + Vector3.Distance(m_Player.position, transform.position));
    //        m_navMesh.SetDestination(m_Player.position);
    //        m_enMonState = EnMonsterState.Chase;
    //    }
    //}

    public virtual void ApplyDamage(int nDamage)
    {
        if (m_enMonState == EnMonsterState.Dead) return;
        Hp -= nDamage;

        if(Hp <= 0)
        {
            m_enMonState = EnMonsterState.Dead;
        }
    }
}
