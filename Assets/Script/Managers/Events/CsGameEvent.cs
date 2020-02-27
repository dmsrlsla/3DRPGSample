using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsGameEvent : MonoBehaviour
{
    public static CsGameEvent Instance
    {
        get { return CsSingleton<CsGameEvent>.GetInstance(); }
    }

    public event Delegate EventChangeCameraState;
    public event Delegate EventChangeCameraDistance;
    public event Delegate<int> EventUseSkill;
    public event Delegate<EnState> EventChangeState;
    public event Delegate EventKillMonster;
    public event Delegate<int> EventDamageText;
    public event Delegate<int,int> EventHpSet;
    public event Delegate<bool, float> EventFade;
    public event Delegate<int> EventDestroyGate; // 스테이지 1단계씩 지날시 해당되는 게이트 제거
    public event Delegate EventTamingMonsterDestroy;
    public event Delegate<bool> EventTamingMonsterGetButton;
    public event Delegate EventTaming;
    public event Delegate EventTamingMonsterAttack;
    public event Delegate EventBossMonsterApear; // 보스 등장씬
    public event Delegate EventBossApearUI; // 보스 등장씬
    public event Delegate EventStageClear; // 보스 등장씬
    public event Delegate EventStageClearUI; // 보스 등장씬

    public void OnEventChangeCameraState()
    {
        if(EventChangeCameraState != null)
        {
            EventChangeCameraState();
        }
    }


    public void OnEventChangeCameraDistance()
    {
        if (EventChangeCameraDistance != null)
        {
            EventChangeCameraDistance();
        }
    }

    public void OnEventUseSkill(int nSkillIndex)
    {
        if (EventUseSkill != null)
        {
            EventUseSkill(nSkillIndex);
        }
    }

    public void OnEventChangeState(EnState State)
    {
        if (EventChangeState != null)
        {
            EventChangeState(State);
        }
    }

    public void OnEventKillMonster()
    {
        if (EventKillMonster != null)
        {
            EventKillMonster();
        }
    }

    public void OnEventDamageText(int nDamage)
    {
        if (EventDamageText != null)
        {
            EventDamageText(nDamage);
        }
    }

    public void OnEventHpSet(int nHp, int nMaxHp)
    {
        if (EventDamageText != null)
        {
            EventHpSet(nHp, nMaxHp);
        }
    }

    public void OnEventFade(bool nFade, float flTime)
    {
        if (EventFade != null)
        {
            EventFade(nFade, flTime);
        }
    }

    public void OnEventDestroyGate(int nGateNum)
    {
        if (EventDestroyGate != null)
        {
            EventDestroyGate(nGateNum);
        }
    }

    public void OnEventTamingMonsterDestroy()
    {
        if(EventTamingMonsterDestroy != null)
        {
            EventTamingMonsterDestroy();
        }
    }

    public void OnEventTamingMonsterGetButton(bool bActive)
    {
        if (EventTamingMonsterGetButton != null)
        {
            EventTamingMonsterGetButton(bActive);
        }
    }

    public void OnEventTaming()
    {
        if(EventTaming != null)
        {
            EventTaming();
        }
    }

    public void OnEventTamingMonsterAttack()
    {
        if(EventTamingMonsterAttack != null)
        {
            EventTamingMonsterAttack();
        }
    }

    public void OnEventBossMonsterApear()
    {
        if(EventBossMonsterApear != null)
        {
            EventBossMonsterApear();
        }
    }

    public void OnEventBossApearUI()
    {
        if(EventBossApearUI != null)
        {
            EventBossApearUI();
        }
    }

    public void OnEventStageClear()
    {
        if (EventStageClear != null)
        {
            EventStageClear();
        }
    }

    public void OnEventStageClearUI()
    {
        if(EventStageClearUI != null)
        {
            EventStageClearUI();
        }
    }
}
