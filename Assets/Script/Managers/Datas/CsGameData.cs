using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsGameData : MonoBehaviour
{
    public static CsGameData Instance
    {
        get { return CsSingleton<CsGameData>.GetInstance(); }
    }

    Transform m_trMyHero;
    Transform m_trTamingMon;
    Transform m_trBossMonster;
    CsIngameCamera m_csInGameCamera;
    float m_flHeroMid;
    float m_flJoysticCosValue;
    float m_flJoysticAngle;
    bool m_bBoostMode;
    bool m_bJoysticDragging;
    bool m_bInDuengeon;
    bool m_bDungeonClear;

    public Transform MyHeroTransform { get { return m_trMyHero; } set { m_trMyHero = value; } }
    public Transform TamingMon { get { return m_trTamingMon; } set { m_trTamingMon = value; } }
    public Transform BossMonster { get { return m_trBossMonster; } set { m_trBossMonster = value; } }
    public CsIngameCamera InGameCamera { get { return m_csInGameCamera; } set { m_csInGameCamera = value; } }
    public float HeroMid { get { return m_flHeroMid; } set { m_flHeroMid = value; } }
    public float JoysticCosValue { get { return m_flJoysticCosValue; } set { m_flJoysticCosValue = value; } }
    public float JoysticAngle { get { return m_flJoysticAngle; } set { m_flJoysticAngle = value; } }
    public bool BoostMode { get { return m_bBoostMode; } set { m_bBoostMode = value; } }
    public bool JoysticDragging { get { return m_bJoysticDragging; } set { m_bJoysticDragging = value; } }
    public bool InDungeon { get { return m_bInDuengeon; } set { m_bInDuengeon = value; } }
    public bool DungeonClear { get { return m_bDungeonClear; } set { m_bDungeonClear = value; } }

    public bool IsHeroStateIdle()
    {
        return false;
    }


    public bool IsHeroStateAttack()
    {
        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
