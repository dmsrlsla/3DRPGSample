using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnAnimStatus { Idle = 0, Walk, Run, Skill01_01, Skill01_02, Skill01_03, Skill02, Skill03, Skill04, Skill05, Interact, Dead, Riding, Fishing, Hanging, Avoid, TamingSkill, TamingTry }
public enum EnState { Idle = 0, MoveByJoystic, Attack, Damage, Interaction, Dead, Fishing, TamingSkill, TamingTry, EnterDungeon }

public class CsSkillCombo : MonoBehaviour
{
    int m_nChainSkillIndex;
    int m_nTempIndex = 0;
    public bool m_bChained;
    Animator m_animatorPlayer;

    //EnClipStatus m_enClipStatus = EnClipStatus.No;
    Queue<int> m_QueueNextSaveChained = new Queue<int>();


    public int ChainSkillIndex { set { m_nChainSkillIndex = value; } }
    public int TempIndex { get { return TempIndex; } set { m_nTempIndex = value; } }
    public Animator AnimatorPlayer { get { return m_animatorPlayer; } set { m_animatorPlayer = value; } }
    //public EnClipStatus ClipStatus { get { return m_enClipStatus; } }
    public bool Chained { get { return m_bChained; } set { m_bChained = value; } }

    public void CheckCombo()
    {
        // 다음 큐에 저장된 체인공격이 있고
        if (m_QueueNextSaveChained.Count > 0 && !AnimatorPlayer.GetCurrentAnimatorStateInfo(0).IsName(((EnAnimStatus)((int)EnAnimStatus.Skill01_01 + m_QueueNextSaveChained.Peek() - 1)).ToString()))
        {
            PlayCombo();
        }
        else if (m_QueueNextSaveChained.Count <= 0)
        {
            if (AnimatorPlayer.GetCurrentAnimatorStateInfo(0).IsName((EnAnimStatus.Idle).ToString())) return;

            // 이전 애니메이션 타임진행이 0.8초 이상인 경우 다음 동작 수행.
            if (AnimatorPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f) 
            {
                m_nTempIndex = 0;
                m_bChained = false;
                m_QueueNextSaveChained.Clear();
                return;
            }
        }
    }

    void PlayCombo() // 콤보 플레이 1_1~1_3까지.
    {
        AnimatorPlayer.SetInteger("status", (int)EnAnimStatus.Skill01_01 + m_QueueNextSaveChained.Dequeue());
    }

    public void StartCombo()
    {
        if (m_QueueNextSaveChained.Count < 2 && m_nTempIndex < 3)
        {
            m_bChained = true;
            m_QueueNextSaveChained.Enqueue(m_nTempIndex);
            m_nTempIndex++;
        }
    }
}


public class CsMyPlayer : MonoBehaviour
{
    static int s_nAnimatorHash_status = Animator.StringToHash("status");
    bool m_bRush = false;
    bool m_bRushStart = false;
    bool m_bBattleMode = false;
    bool m_bEnterDungeon = false;
    bool m_bIsTaming = false;

    public bool Rush { get { return m_bRush; } set { m_bRush = value; } }
    public bool RushStart { get { return m_bRushStart; } set { m_bRushStart = value; } }
    public bool BattleMode { get { return m_bBattleMode; } set { m_bBattleMode = value; } }
    public bool EnterDungeon { get { return m_bEnterDungeon; } set { m_bEnterDungeon = value; } }
    public bool IsTaming { get { return m_bIsTaming; } set { m_bIsTaming = value; } }
    NavMeshAgent m_navMeshAgent;
    Animator m_animator;
    Transform m_trLHand;
    Transform m_trRHand;

    List<CsMonster> m_listTargetMonster = new List<CsMonster>();
    float m_flMoveSpeed;
    int m_attackDamage;

    const int c_nMoveDirectionCount = 16;
    const float c_flDirectionAngle = 2 * Mathf.PI / c_nMoveDirectionCount;
    const float c_flDirectionAngleHalf = c_flDirectionAngle / 2;
    Vector3[] m_avtMoveDirection = new Vector3[c_nMoveDirectionCount];

    int m_nSkillIndex;

    EnState m_enState;

    CsSkillCombo m_csSkillCombo = new CsSkillCombo();

    public void Awake()
    {
        CsGameEvent.Instance.EventChangeState += OnEventChangeState;
        CsGameEvent.Instance.EventUseSkill += OnEventUseSkill; // 스킬 사용 이벤트 등록
    }

    public void OnDestroy()
    {
        CsGameEvent.Instance.EventChangeState -= OnEventChangeState;
        CsGameEvent.Instance.EventUseSkill -= OnEventUseSkill; // 스킬 사용 이벤트 해제
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        m_animator = gameObject.GetComponent<Animator>();
        m_flMoveSpeed = 4f;
        InitMovePoint();
        m_nSkillIndex = -1;

        m_csSkillCombo = new CsSkillCombo();
        m_csSkillCombo.AnimatorPlayer = m_animator;
        m_trRHand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/RightAxe");
        m_trLHand = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/LeftAxe");
        m_attackDamage = 30;
        m_navMeshAgent.speed = 8;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_enState)
        {
            case EnState.MoveByJoystic:
                UpdateMoveByJoystick(CsGameData.Instance.JoysticAngle);
                m_animator.SetInteger(s_nAnimatorHash_status, (int)EnAnimStatus.Run);
                break;
            case EnState.Idle:
                m_animator.SetInteger(s_nAnimatorHash_status, (int)EnAnimStatus.Idle);
                break;
            case EnState.Attack:
                m_csSkillCombo.CheckCombo();
                SelectAttackAnim(m_nSkillIndex);
                break;
            case EnState.EnterDungeon:
                m_animator.SetInteger(s_nAnimatorHash_status, (int)EnAnimStatus.Run);
                break;
            case EnState.TamingTry:
                m_animator.SetInteger(s_nAnimatorHash_status, (int)EnAnimStatus.TamingTry);
                break;
        }

    }

    bool UpdateMoveByJoystick(float flAngle)
    {
        if(CsGameData.Instance.JoysticDragging) // 조이스틱 드래깅 중이라면
        {

            float flRotationSpeed = 4.0f;

            Vector3 vtDir = Camera.main.transform.TransformDirection(GetMoveDirection(flAngle));
            vtDir.y = 0;
            vtDir.Normalize();

            if (vtDir != Vector3.zero)
            {
                Quaternion qtnFrom = transform.rotation;
                Quaternion qtnTo = Quaternion.LookRotation(vtDir);
                transform.rotation = Quaternion.Lerp(qtnFrom, qtnTo, flRotationSpeed * Time.deltaTime);
            }

            Vector3 vtOldPos = transform.position;
            m_navMeshAgent.velocity = vtDir * m_flMoveSpeed;
            ChangeState(EnState.MoveByJoystic);
        }
        else
        {
            ChangeState(EnState.Idle);
        }
        return !CsGameData.Instance.JoysticDragging;
    }

    public void InitMovePoint()
    {
        for (int i = 0; i < c_nMoveDirectionCount; i++)
        {
            Vector3 vt = new Vector3(Mathf.Cos(i * c_flDirectionAngle), 0, Mathf.Sin(i * c_flDirectionAngle));
            vt.Normalize();
            m_avtMoveDirection[i] = vt;
        }

    }

    //---------------------------------------------------------------------------------------------------
    public static int GetMoveDirectionIndex(float flRad)
    {
        if (flRad < 0)
        {
            flRad = (flRad % (2 * Mathf.PI)) + (2 * Mathf.PI);
        }

        float flRad2 = (flRad + c_flDirectionAngleHalf) % (2 * Mathf.PI);

        return (int)(flRad2 / c_flDirectionAngle);
    }

    public void ChangeState(EnState enStatus)
    {
        m_enState = enStatus;
    }

    void SelectAttackAnim(int nSkillIndex)
    {
        if(m_csSkillCombo.m_bChained == false)
        {
            ChangeState(EnState.Idle);
        }
    }

    //---------------------------------------------------------------------------------------------------
    public Vector3 GetMoveDirection(float flRad)
    {
        return m_avtMoveDirection[GetMoveDirectionIndex(flRad)];
    }

    #region OnEvent
    void OnEventChangeState(EnState enState)
    {
        m_enState = enState;
    }

    void OnEventUseSkill(int nSkillIndex)
    {
        //m_nSkillIndex = nSkillIndex;
        if (!m_bIsTaming)
        {
            if (nSkillIndex == 0)
            {
                m_csSkillCombo.StartCombo();
            }
            ChangeState(EnState.Attack);
        }
        CsGameEvent.Instance.OnEventTamingMonsterAttack();
    }
    #endregion


    void OnAnimAttackSkillEffect01_02()
    {
       CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[0].name, m_trRHand, m_trRHand.position, m_trRHand.rotation);
    }

    void OnAnimAttackSkillEffect01_01()
    {
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[0].name, m_trLHand, m_trLHand.position, m_trLHand.rotation);
    }

    void OnAnimAttackSkillEffect01_03()
    {
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[1].name, m_trRHand, m_trRHand.position, m_trRHand.rotation);
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[2].name, m_trLHand, m_trLHand.position, m_trLHand.rotation);
    }

    void OnAnimAttackSkillEffect01_03_Ground()
    {
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[3].name, transform, transform.position + transform.forward * 2f, transform.rotation);
    }

    void OnAnimEffectTaming()
    {
        CsEffectManager.Instance.PlayEffectTake2(CsEffectManager.Instance.m_listEffects[4].name, transform, transform.position + transform.forward * 2f, transform.rotation, 0,10);
    }

    void OnAnimSkill01_01Sound()
    {

    }

    void OnAnimSkill01_02Sound()
    {

    }

    void OnAnimSkill01_04Sound()
    {

    }

    void OnAnimTargetSelect()
    {
        Collider[] CheckCollider = Physics.OverlapSphere(transform.position + transform.forward, 1.7f);
        foreach(Collider col in CheckCollider)
        {
            if(col.tag == "Monster" || col.tag == "TamingMon")
            {
                m_listTargetMonster.Add(col.GetComponent<CsMonster>());
            }
        }
    }

    void OnAnimCustomStep()
    {

    }

    void OnAnimApplyDamage()
    {
        foreach(CsMonster Monster in m_listTargetMonster)
        {
            int RanDamage = UnityEngine.Random.Range(20, 40);
            Monster.ApplyDamage(RanDamage);
            CsGameEvent.Instance.OnEventDamageText(RanDamage);
        }
        m_listTargetMonster.Clear();
    }

    void OnEventAnimCameraActionStart() // 점프
    {

    }

    void OnEventAnimCameraActionEnd()
    {

    }

    //---------------------------------------------------------------------------------------------------
    public void LookAtPosition(Vector3 vtPos)
    {
        Vector3 dir = vtPos - this.transform.position;
        dir.y = 0f;
        dir.Normalize();
        if (dir == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    //---------------------------------------------------------------------------------------------------
    public void MoveByDirecting(Vector3 vtMovePos, float flStopRange, bool bRun)
    {
        BattleMode = false;
        // 연출중에 배틀모드 끔
        if (bRun)
        {
            SetDestination(vtMovePos, flStopRange);
            //Move(EnState.MoveByDirecting, vtMovePos, flStopRange, true);
            m_navMeshAgent.speed = m_flMoveSpeed = 6f;
        }
        else
        {
            m_navMeshAgent.speed = m_flMoveSpeed = 4f;
        }
    }

    //---------------------------------------------------------------------------------------------------
    protected virtual void SetDestination(Vector3 vtPos, float flStopRange)
    {
        m_navMeshAgent.SetDestination(vtPos);
        //m_vtMovePos = vtPos;
        //m_flMoveStopRange = flStopRange;
    }


    //---------------------------------------------------------------------------------------------------
    public void NavMeshSetting()
	{
		m_navMeshAgent.radius = 0.2f;
		m_navMeshAgent.height = 2f;
		m_navMeshAgent.baseOffset = 0f;

		m_navMeshAgent.angularSpeed = 720f;
		m_navMeshAgent.acceleration = 100f;
		m_navMeshAgent.stoppingDistance = 0.1f;
		m_navMeshAgent.autoBraking = true;
		m_navMeshAgent.autoRepath = true;
		m_navMeshAgent.avoidancePriority = 50;
		m_navMeshAgent.autoTraverseOffMeshLink = false;
		m_navMeshAgent.autoRepath = false;

		m_navMeshAgent.speed = m_flMoveSpeed;
		m_navMeshAgent.enabled = false;
		m_navMeshAgent.enabled = true;
	}

    public void MoveTeleport(Vector3 vtMove)
    {
        m_navMeshAgent.enabled = false;
        transform.localPosition = vtMove;
        m_navMeshAgent.enabled = true;
    }

    public void PlayerHide(bool bActive)
    {
        SkinnedMeshRenderer[] skins = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer skin in skins)
        {
            skin.enabled = bActive;
        }

        if(!bActive)
        {
            m_navMeshAgent.speed = 50f;
        }
        else
        {
            m_navMeshAgent.speed = 8f;
        }
    }
}
