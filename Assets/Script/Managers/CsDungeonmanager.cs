using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CsDungeonmanager : CsSceneManager
{
    int m_nWave = 0;
    int m_nKillMonCount = 0;
    int m_nKillCountNextStep = 0;
    // Start is called before the first frame update

    Transform m_trMyPlyer;
    CsMyPlayer m_csPlayer;

    private void Awake()
    {
        Transform trPos = transform.Find("StartPos");
        CreateHero(trPos.position, 0, "Brute");
        SceneManager.LoadScene("MainUI", LoadSceneMode.Additive);
        CsGameEvent.Instance.EventKillMonster += OnEventKillMonster;
        CsGameEvent.Instance.EventTaming += OnEventTaming;
        CsGameEvent.Instance.EventBossMonsterApear += OnEventBossMonsterApear;
        CsGameEvent.Instance.EventStageClear += OnEventStageClear;
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventKillMonster -= OnEventKillMonster;
        CsGameEvent.Instance.EventTaming -= OnEventTaming;
        CsGameEvent.Instance.EventBossMonsterApear -= OnEventBossMonsterApear;
        CsGameEvent.Instance.EventStageClear -= OnEventStageClear;
    }

    void Start()
    {
        Init();
    }

    //---------------------------------------------------------------------------------------------------
    protected IEnumerator DirectingByDungeon() // 던전 입장 연출.
    {
        CsIngameCamera EnterCamera = CsGameData.Instance.InGameCamera;
        //DepthOfField depthOfFieldSet = CsIngameData.Instance.InDungeonCamera.depthOfField;
        CsGameEvent.Instance.OnEventFade(true,3f);
        // 던전 이름 출력(나중에 텍스트도 설정 가능하게 해두면 좋을듯)
        //CsGameEventToUI.Instance.OnEventDungeonName(true);

        Vector3 vtOffestPos = m_csPlayer.transform.forward * 7;
        Vector3 vtHeroStartPos = m_csPlayer.transform.position;
        m_csPlayer.transform.position = m_csPlayer.transform.position - vtOffestPos;
        m_csPlayer.LookAtPosition(vtHeroStartPos);

        // 보류
        //CsGameEventToUI.Instance.OnEventHideMainUI(true);

        EnterCamera.Height = EnterCamera.m_dungeonEnterStep_1_Set.Start_Height;
        EnterCamera.Length = EnterCamera.m_dungeonEnterStep_1_Set.Start_Length;
        EnterCamera.m_flScreenYHeight = EnterCamera.m_dungeonEnterStep_1_Set.Start_ScreenYHeight;
        EnterCamera.Zoom = EnterCamera.m_dungeonEnterStep_1_Set.Start_Zoom;
        EnterCamera.DelayWatingTime = 0.0f;
        EnterCamera.ActionCamera = EnActionCameraType.Enter1;
        EnterCamera.StartEnterStpe1();

        yield return new WaitForSeconds(0.3f);
        m_csPlayer.BattleMode = false;
        Debug.Log("DirectingByDungeon     ===Run===");
        yield return new WaitForSeconds(0.1f);
        m_csPlayer.MoveByDirecting(vtHeroStartPos, 0.2f, true);
        m_csPlayer.ChangeState(EnState.EnterDungeon);
        yield return new WaitForSeconds(1f);

        m_csPlayer.BattleMode = false;
        Debug.Log("DirectingByDungeon     ===Walk===");
        m_csPlayer.MoveByDirecting(vtHeroStartPos, 0.2f, false);
        m_csPlayer.ChangeState(EnState.Idle);
        // 연출블러
        //CsIngameData.Instance.InGameCamera.CameraBlur.BlurStrength = 1.5f;
        //CsIngameData.Instance.InGameCamera.CameraBlur.BlurWidth = 0.3f;


        yield return new WaitForSeconds(0.5f);

        m_csPlayer.ChangeState(EnState.MoveByJoystic);
        m_csPlayer.BattleMode = true;
        m_csPlayer.NavMeshSetting();

        float EnterStpe1_delay;
        EnterStpe1_delay = EnterCamera.m_dungeonEnterStep_1_Set.Delay + EnterCamera.m_dungeonEnterStep_1_Set.Duration - 1f - 0.5f;// -0.5f; 
        if (EnterStpe1_delay < 0)
        {
            EnterStpe1_delay = 0.0f;
        }
        yield return new WaitForSeconds(EnterStpe1_delay);

        EnterCamera.DelayWatingTime = 0.0f;
        EnterCamera.ActionCamera = EnActionCameraType.Enter2;
        EnterCamera.StartEnterStpe2();
        Debug.Log("DirectingByDungeon    ===IDLE===");

        float EnterStpe2_delay;
        EnterStpe2_delay = EnterCamera.m_dungeonEnterStep_2_Set.Delay + EnterCamera.m_dungeonEnterStep_2_Set.Duration;

        if (EnterStpe2_delay < 0)
        {
            EnterStpe2_delay = 0.0f;
        }

        yield return new WaitForSeconds(EnterStpe2_delay);
        CsGameEvent.Instance.OnEventFade(false, 1.5f);
        ////던전 이름 출력 해제.
        //CsGameEventToUI.Instance.OnEventDungeonName(false);
        ////던전 타이머 시작.
        //CsGameEventToUI.Instance.OnEventDungeonTimer(true);
        ////UI 숨김 해제 풀어줌
        //CsGameEventToUI.Instance.OnEventHideMainUI(false);

        //CsGameEventToIngame.Instance.OnEventChangeCameraState();
        EnterCamera.ChangeNewState(EnCameraState.Auto);
        EnterCamera.ActionCamera = EnActionCameraType.None;

        //CsGameData.Instance.MyHeroTransform.GetComponent<CsMyPlayer>().MyHeroNavMeshAgent.enabled = false;
        //CsGameData.Instance.MyHeroTransform.GetComponent<CsMyPlayer>().MyHeroNavMeshAgent.enabled = true;
    }

    void Init()
    {
        m_nKillCountNextStep = 3;
        Dictionary<int, List<CsMonster>> DicMons = CsMonData.Instance.DicMon;

        DicMons.Add(0, new List<CsMonster>());
        DicMons.Add(1, new List<CsMonster>());
        DicMons.Add(2, new List<CsMonster>());
        DicMons.Add(3, new List<CsMonster>());

        //DicMons[0].Add(new CsTamingMon("Mon_11", 150, 150, 10, new Vector3(-10f, 11.9f, -70.85f), 90f, 10));


        DicMons[0].Add(new CsMonster("Mon_3", 150, 150, 10, new Vector3(-5.95f, 11.9f, -70.85f), 90f, 8));

        DicMons[0].Add(new CsMonster("Mon_2", 100, 100, 10, new Vector3(-15f, 11.9f, -75.85f), 90f, 2));
        DicMons[0].Add(new CsMonster("Mon_2", 100, 100, 10, new Vector3(-10f, 11.9f, -75.85f), 90f, 3));
        DicMons[0].Add(new CsMonster("Mon_2", 100, 100, 10, new Vector3(-5f, 11.9f, -75.85f), 90f, 3));

        DicMons[1].Add(new CsTamingMon("Mon_11", 100, 100, 10, new Vector3(-33.88f, 19.9f, -62.3f), 90f, 1));

        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -48.9f), 90f, 4));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -40.4f), 90f, 5));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -32.9f), 90f, 6));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(0, 17.9f, -48.9f), 90f, 7));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -48.9f), 90f, 4));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -40.4f), 90f, 5));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -32.9f), 90f, 6));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(0, 17.9f, -48.9f), 90f, 7));

        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -48.9f), 90f, 4));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -40.4f), 90f, 5));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -32.9f), 90f, 6));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(0, 17.9f, -48.9f), 90f, 7));

        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -48.9f), 90f, 4));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -40.4f), 90f, 5));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -32.9f), 90f, 6));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(0, 17.9f, -48.9f), 90f, 7));

        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -48.9f), 90f, 4));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -40.4f), 90f, 5));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(-7f, 20.3f, -32.9f), 90f, 6));
        DicMons[2].Add(new CsMonster("Mon_5", 150, 150, 10, new Vector3(0, 17.9f, -48.9f), 90f, 7));



        DicMons[3].Add(new CsBossMonster("Mon_3", 150, 150, 10, new Vector3(18.1f, 10.8f, -2.3f), 90f, 10));


        m_nWave = 0;
        m_trMyPlyer = CsGameData.Instance.MyHeroTransform;
        m_csPlayer = m_trMyPlyer.GetComponent<CsMyPlayer>();
        StartCoroutine(DirectingByDungeon()); // 던전 입장.
        StartNextStep(m_nWave);
        IeTest();
    }

    IEnumerator IeTest()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < m_listMonster.Count; i++)
        {
            Destroy(m_listMonster[i]);
        }
        StartNextStep(m_nWave);
    }

    void OnEventKillMonster()
    {
        m_nKillMonCount++;
        if (m_nKillCountNextStep == m_nKillMonCount)
        {
            m_nWave++;
            m_nKillMonCount = 0;
            StartNextStep(m_nWave);
        }
    }

    void OnEventTaming()
    {
        StartCoroutine(StartTamingAction());
    }

    IEnumerator StartTamingAction()
    {
        CsGameEvent.Instance.OnEventTamingMonsterGetButton(false);
        //CsGameEvent.Instance.OnEventFade(true, 3f);
        yield return new WaitForSeconds(1.0f);
        CsTamingMon tamingMon = CsGameData.Instance.TamingMon.GetComponent<CsTamingMon>();
        m_csPlayer.ChangeState(EnState.TamingTry);

        //
        tamingMon.MoveTeleport(new Vector3(-40f, 22.5f, -59.3f));

        m_csPlayer.MoveTeleport(new Vector3(-36.88f, 19.9f, -62.3f));
        m_csPlayer.transform.LookAt(CsGameData.Instance.TamingMon);
        CsGameData.Instance.InGameCamera.StartEnterTaming();
        yield return new WaitForSeconds(CsGameData.Instance.InGameCamera.m_dungeonTaming.Delay);
        yield return new WaitForSeconds(5f);
        m_csPlayer.PlayerHide(false);

        CsGameData.Instance.TamingMon.parent = m_csPlayer.transform;

        //m_csPlayer.transform.parent = CsGameData.Instance.TamingMon;

        CsGameData.Instance.TamingMon.localPosition = Vector3.zero;
        CsGameData.Instance.TamingMon.localRotation = Quaternion.identity;
        tamingMon.ComplateTaming();
        yield return new WaitForSeconds(0.5f);
        CsGameData.Instance.InGameCamera.ChangeNewState(EnCameraState.Auto);
        CsGameData.Instance.InGameCamera.ActionCamera = EnActionCameraType.None;
        //CsGameData.Instance.InGameCamera.SettingTamingCamera(true);
        m_csPlayer.IsTaming = true;
        m_nWave++;
        m_nKillMonCount = 0;
        StartNextStep(m_nWave);
    }

    void OnEventBossMonsterApear()
    {
        StartCoroutine(StartBossApearAction());
    }

    IEnumerator StartBossApearAction() // 보스 등장씬
    {
        CsGameEvent.Instance.OnEventBossApearUI();
        CsGameData.Instance.InGameCamera.trCameraPos = null;
        CsGameData.Instance.InGameCamera.transform.position = 
            new Vector3(CsGameData.Instance.BossMonster.position.x + 2, CsGameData.Instance.BossMonster.position.y + 4, CsGameData.Instance.BossMonster.position.z + 6);
        CsGameData.Instance.InGameCamera.transform.LookAt(CsGameData.Instance.BossMonster);
        CsGameData.Instance.BossMonster.LookAt(new Vector3(CsGameData.Instance.InGameCamera.transform.position.x, 0, CsGameData.Instance.InGameCamera.transform.position.z));
        CsGameData.Instance.BossMonster.GetComponent<CsBossMonster>().BossRoar(true);
        yield return new WaitForSeconds(4f);
        CsGameData.Instance.BossMonster.GetComponent<CsBossMonster>().BossRoar(false);
        CsGameData.Instance.InGameCamera.trCameraPos = CsGameData.Instance.MyHeroTransform;
        // 테이밍 몬스터 사망(보스 연출 끝난 뒤.)
        CsGameEvent.Instance.OnEventTamingMonsterDestroy();
        m_csPlayer.PlayerHide(true); // 플레이어를 다시 드러나게 해줌.
        yield return new WaitForSeconds(1f);
        //CsGameData.Instance.InGameCamera.SettingTamingCamera(false);
        m_csPlayer.IsTaming = false;
    }

    void StartNextStep(int nWave)
    {
        m_nKillCountNextStep = CsMonData.Instance.DicMon[nWave].Count;
        foreach (CsMonster Mon in CsMonData.Instance.DicMon[nWave])
        {
            CreateMonster(Mon);
        }
        CsGameEvent.Instance.OnEventDestroyGate(nWave);
    }

    void OnEventStageClear()
    {
        StartCoroutine(DungeonClearDirection());
    }

    //----------------------------------------------------------------------------------------------------
    IEnumerator DungeonClearDirection()
    {
        // 카메라 위치설정
        CsGameData.Instance.InGameCamera.Height = 4.0f;
        CsGameData.Instance.InGameCamera.Length = 5.0f;
        CsGameData.Instance.InGameCamera.Pivot2D_Y = 2.0f;
        CsGameData.Instance.InGameCamera.CameraMove(0.57f, 3.8f, 1.5f);
        yield return new WaitForSeconds(0.2f);
        CsGameData.Instance.InGameCamera.DungeonClearCameraMove(0.45f, 1f); // 주인공을 빙글빙글 도는 카메라.
        Debug.Log("끝");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
