using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientCommon;
using System;
//using SimpleDebugLog;

public enum EnFxDun { Betrayal, D1, D2, D3, D4, D5SaintDock, D6Chris, D7AnKou }
public enum EnMonType { Normal, Dominate, Boss, SaintDock, Object, LaChiristiana }

public class CsFxDunMonData
{
	int m_nMonsterId;
	string m_strName;
	string m_strPrefabName;
	int m_nLevel;
	float m_flScale;
	int m_nMaxHp;
	int m_nPhysicalOffense;
	Vector3 m_vecPosition;
	float m_flRotationY;
	CsMonster.EnOwnership m_enOwnership;
	MonsterInstanceType m_monsterInstanceType;
	float m_flCreationDelay;
	bool m_bDominateMonster;
	bool m_bBoss;
	EnMonType m_enMonType;

	int m_nSetStep;							// 본인의 스텝 정보를 가짐.(산체용)
	int m_nSetWave;							// 본인의 웨이브 정보를 가짐.(산체용)

	public int MonsterId { get { return m_nMonsterId; } }
	public string Name { get { return m_strName; } }
	public string PrefabName { get { return m_strPrefabName; } }
	public int Level { get { return m_nLevel; } }
	public float Scale { get { return m_flScale; } }
	public int MaxHp { get { return m_nMaxHp; } }
	public int PhysicalOffense { get { return m_nPhysicalOffense; } }
	public Vector3 Position { get { return m_vecPosition; } set { m_vecPosition = value; } }
	public float RotationY { get { return m_flRotationY; } }
    public CsMonster.EnOwnership Ownership { get { return m_enOwnership; } }
	public MonsterInstanceType MonsterInstanceType { get { return m_monsterInstanceType; } }
	public float CreationDelay { get { return m_flCreationDelay; } }
	public bool IsDominateMonster { get { return m_bDominateMonster; } }
	public bool IsBoss { get { return m_bBoss; } }
	public EnMonType MonType { get { return m_enMonType; } set { m_enMonType = value; } }

	public int SetStep { get { return m_nSetStep; } set { m_nSetStep = value; } }
	public int SetWave { get { return m_nSetWave; } set { m_nSetWave = value; } }

	//public CsFxDunMonData(int nMonsterId, string strName, string strPrefabName, int nLevel, float flScale, int nMaxHp, int nPhysicalOffense, Vector3 vecPosition, float flRotationY,
	//    CsMonster.EnOwnership enOwnership, MonsterInstanceType monsterInstanceType, float flCreationDelay, bool bDominateMonster = false, bool bBoss = false)
	public CsFxDunMonData(int nMonsterId, string strName, string strPrefabName, int nLevel, float flScale, int nMaxHp, int nPhysicalOffense, Vector3 vecPosition, float flRotationY,
		CsMonster.EnOwnership enOwnership, MonsterInstanceType monsterInstanceType, EnMonType enMonsType, float flCreationDelay, int nStep = 0, int nWave = 0)
	{
		m_nMonsterId			=	nMonsterId;
		m_strName				=	strName;	
		m_strPrefabName			=	strPrefabName;	
		m_nLevel				=	nLevel;	
		m_flScale				=	flScale;	
		m_nMaxHp				=	nMaxHp;		
		m_nPhysicalOffense		=	nPhysicalOffense;			
		m_vecPosition			=	vecPosition;		
		m_flRotationY			=	flRotationY;		
		m_enOwnership			=	enOwnership;		
		m_monsterInstanceType	=	monsterInstanceType;		
		//m_bDominateMonster		=	bDominateMonster;		
		//m_bBoss				 	=	bBoss;       
		m_flCreationDelay 		= 	flCreationDelay;
		m_enMonType				=	enMonsType;
		m_nSetStep					=	nStep;			// 데이터 접근용 데이터가 아닌, 몬스터가 자신의 스텝 정보를 기억하도록 함.
		m_nSetWave					=	nWave;			// 데이터 접근용 데이터가 아닌, 몬스터가 자신의 웨이브 정보를 기억하도록 함.
	}
}

public class CsFxDunMonDeployment
{
    public int SetWave;
	//---------------------------------------------------------------------------------------------------
	public static CsFxDunMonDeployment Instance
	{
		get { return CsSingleton<CsFxDunMonDeployment>.GetInstance(); }
	}

	//---------------------------------------------------------------------------------------------------
	Dictionary<EnFxDun, Dictionary<int, Dictionary<int, List<CsFxDunMonData>>>> m_dic;

	//---------------------------------------------------------------------------------------------------
	//void AddMonData(EnFxDun enDun, int nStep, int nWave, int nMonsterId, string strName, string strPrefabName, int nLevel, float flScale, int nMaxHp, int nPhysicalOffense, Vector3 vecPosition, float flRotationY,
	//    CsMonster.EnOwnership enOwnership, MonsterInstanceType monsterInstanceType, float flCreationDelay = 0, bool bDominateMonster = false, bool bBoss = false)
	void AddMonData(EnFxDun enDun, int nStep, int nWave, int nMonsterId, string strName, string strPrefabName, int nLevel, float flScale, int nMaxHp, int nPhysicalOffense, Vector3 vecPosition, float flRotationY,
		CsMonster.EnOwnership enOwnership, MonsterInstanceType monsterInstanceType, EnMonType enMonsType, float flCreationDelay = 0)
	{
		Dictionary<int, Dictionary<int, List<CsFxDunMonData>>> dicDun;
		if (!m_dic.TryGetValue(enDun, out dicDun))
		{
			dicDun = new Dictionary<int, Dictionary<int, List<CsFxDunMonData>>>();
			m_dic.Add(enDun, dicDun);
		}

		Dictionary<int, List<CsFxDunMonData>> dicStep;
		if (!dicDun.TryGetValue(nStep, out dicStep))
		{
			dicStep = new Dictionary<int, List<CsFxDunMonData>>();
			dicDun.Add(nStep, dicStep);
		}

		List<CsFxDunMonData> listWave;
		if (!dicStep.TryGetValue(nWave, out listWave))
		{
			listWave = new List<CsFxDunMonData>();
			dicStep.Add(nWave, listWave);
		}
		// 추가적인 SetStep, Wave 저장.
		listWave.Add(new CsFxDunMonData(nMonsterId, strName, strPrefabName, nLevel, flScale, nMaxHp, nPhysicalOffense, vecPosition, flRotationY, enOwnership, monsterInstanceType, enMonsType, flCreationDelay, nStep, nWave));
	}

	//---------------------------------------------------------------------------------------------------
	public List<CsFxDunMonData> GetWave(EnFxDun enDun, int nStep, int nWave)
	{
//		dd.d("GetWave1", enDun, nStep, nWave);
		if (!m_dic.ContainsKey(enDun)) return null;
		Dictionary<int, Dictionary<int, List<CsFxDunMonData>>> dicDun = m_dic[enDun];
//		dd.d("GetWave2");

		if (!dicDun.ContainsKey(nStep)) return null;
		Dictionary<int, List<CsFxDunMonData>> dicStep = dicDun[nStep];
//		dd.d("GetWave3");
				
		if (!dicStep.ContainsKey(nWave)) return null;
//		dd.d("GetWave4");
		return dicStep[nWave];
	}

	//---------------------------------------------------------------------------------------------------
	public CsFxDunMonData GetMonster(EnFxDun enDun, int nStep, int nWave, int nMonsterId)
	{
//		dd.d("GetDunType", enDun, nStep, nWave);
		if (!m_dic.ContainsKey(enDun)) return null;
		Dictionary<int, Dictionary<int, List<CsFxDunMonData>>> dicDun = m_dic[enDun];
//		dd.d("GetStep");

		if (!dicDun.ContainsKey(nStep)) return null;
		Dictionary<int, List<CsFxDunMonData>> dicStep = dicDun[nStep];
//		dd.d("GetWave");

		if (!dicStep.ContainsKey(nWave)) return null;
		List<CsFxDunMonData> listWave = dicStep[nWave];
//		dd.d("GetMonster");

		foreach (CsFxDunMonData item in listWave)
		{
			if (item.MonsterId == nMonsterId)
			{
				return item;
			}
		}
		Debug.Log("해당 몬스터가 없습니다.");
		return null; // 아무것도 없다면, null 리턴
	}

	//---------------------------------------------------------------------------------------------------
	public void Init()
	{
		m_dic = new Dictionary<EnFxDun, Dictionary<int, Dictionary<int, List<CsFxDunMonData>>>>();
		Load();

		foreach (KeyValuePair<EnFxDun, Dictionary<int, Dictionary<int, List<CsFxDunMonData>>>> kvpDicDun in m_dic)
		{
			foreach (KeyValuePair<int, Dictionary<int, List<CsFxDunMonData>>> kvpDicStep in kvpDicDun.Value)
			{
				foreach (KeyValuePair<int, List<CsFxDunMonData>> kvpListWave in kvpDicStep.Value)
				{
					kvpListWave.Value.Sort((prev, next) => prev.CreationDelay.CompareTo(next.CreationDelay));
				}
			}
		}
	}

	//---------------------------------------------------------------------------------------------------
	void Load()
	{
		EnFxDun en;

		#region EnFxDun.D1

		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.D1;
		//---------------------------------------------------------------------------------------------------
		AddMonData(en, 0, 0, 1, "Test", "8005", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster,EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 2, "Test", "8006", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 3, "Test", "8005", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		AddMonData(en, 1, 0, 1, "Test", "8007", 10, 1.7f, 1739, 65, new Vector3(191.5f, 1f, -301.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Dominate, 0);

		AddMonData(en, 2, 0, 1, "Test", "8031", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 2, "Test", "8031", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 3, "Test", "8031", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 4, "Test", "8031", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

		AddMonData(en, 2, 0, 5, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 6, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 7, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 8, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 0, 9, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(238f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 10, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(234f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 11, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(237f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 12, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(232.5f, 1f, -302f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);

		AddMonData(en, 2, 0, 13, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 14, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 15, "Test", "8051", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 16, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 17, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 18, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 1, 1, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 2, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 3, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 4, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 5, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 6, "Test", "8038", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 1, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 2, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 3, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -304f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 4, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 5, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 6, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -302.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 3, 0, 7, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 8, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -294.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 9, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 4, 0, 1, "Test", "9005", 10, 1.3f, 2560, 65, new Vector3(356.5f, 2.5f, -298.5f), 180, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Boss, 0);
		AddMonData(en, 4, 0, 2, "Test", "8025", 10, 1.5f, 870, 65, new Vector3(359.5f, 2.5f, -285.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 4, 0, 3, "Test", "8025", 10, 1.5f, 870, 65, new Vector3(359.5f, 2.5f, -311.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		#endregion

		#region EnFxDun.Betrayal

		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.Betrayal;
		//---------------------------------------------------------------------------------------------------
		AddMonData(en, 0, 0, 1, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 2, "Test", "8014", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 3, "Test", "8013", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		AddMonData(en, 1, 0, 1, "Test", "8007", 10, 1.5f, 1739, 65, new Vector3(191.5f, 1f, -301.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Dominate, 0);

		AddMonData(en, 2, 0, 1, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 2, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 3, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 4, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

		AddMonData(en, 2, 0, 5, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 6, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 7, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 8, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 0, 9, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(238f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 10, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(234f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 11, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(237f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 12, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(232.5f, 1f, -302f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);

		AddMonData(en, 2, 0, 13, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 14, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 15, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 16, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 17, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 18, "Test", "8054", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 1, 1, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 2, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 3, "Test", "8052", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 4, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 5, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 6, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 1, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 2, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 3, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -304f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 4, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 5, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 6, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -302.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 3, 0, 7, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 8, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -294.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 9, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 4, 0, 1, "Test", "9005", 10, 1.3f, 2560, 65, new Vector3(356.5f, 1f, -298.5f), 180, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Boss, 0);
		AddMonData(en, 4, 0, 2, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(359.5f, 1f, -285.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 4, 0, 3, "Test", "8025", 10, 1.3f, 870, 65, new Vector3(359.5f, 1f, -311.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		#endregion

		#region EnFxDun.D2

		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.D2;
		//---------------------------------------------------------------------------------------------------
		AddMonData(en, 0, 0, 1, "Test", "8039", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 2, "Test", "8039", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 3, "Test", "8039", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		AddMonData(en, 1, 0, 1, "Test", "8007", 10, 1.7f, 1739, 65, new Vector3(191.5f, 1f, -301.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Dominate, 0);

		AddMonData(en, 2, 0, 1, "Test", "8042", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 2, "Test", "8042", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 3, "Test", "8042", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 4, "Test", "8042", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

		AddMonData(en, 2, 0, 5, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 6, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 7, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 8, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 0, 9, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(238f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 10, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(234f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 11, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(237f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 12, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(232.5f, 1f, -302f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);

		AddMonData(en, 2, 0, 13, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 14, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 15, "Test", "8040", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 16, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 17, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 18, "Test", "8044", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 1, 1, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 2, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 3, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 4, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 5, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 6, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 1, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 2, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 3, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -304f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 4, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 5, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 6, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -302.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 3, 0, 7, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 8, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -294.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 9, "Test", "8008", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 4, 0, 1, "Test", "9005", 10, 1.3f, 2560, 65, new Vector3(356.5f, 1f, -298.5f), 180, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Boss, 0);
		AddMonData(en, 4, 0, 2, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(359.5f, 1f, -285.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 4, 0, 3, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(359.5f, 1f, -311.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		#endregion

		#region EnFxDun.D4

		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.D4;
		//---------------------------------------------------------------------------------------------------
		AddMonData(en, 0, 0, 1, "Test", "8045", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 2, "Test", "8045", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 0, 0, 3, "Test", "8045", 10, 1.3f, 870, 65, new Vector3(147.35f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		AddMonData(en, 1, 0, 1, "Test", "8007", 10, 1.7f, 1739, 65, new Vector3(191.5f, 1f, -301.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Dominate, 0);

		AddMonData(en, 2, 0, 1, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 2, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 3, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 4, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

		AddMonData(en, 2, 0, 5, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 6, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 7, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 8, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 0, 9, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(238f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 10, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(234f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 11, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(237f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);
		AddMonData(en, 2, 0, 12, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(232.5f, 1f, -302f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 1.0f);

		AddMonData(en, 2, 0, 13, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 14, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 15, "Test", "8054", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 16, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 17, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 2, 0, 18, "Test", "8020", 10, 1.7f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 2, 1, 1, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -294f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 2, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 3, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 4, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(247f, 1f, -303f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 5, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -297f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 2, 1, 6, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(252f, 1f, -300f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 1, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -299f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 2, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 3, 0, 3, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -304f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		AddMonData(en, 3, 0, 4, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -297.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 5, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -296f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 6, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -302.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 3, 0, 7, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(279f, 1f, -295.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 8, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(288.5f, 1f, -294.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);
		AddMonData(en, 3, 0, 9, "Test", "8028", 10, 1.3f, 870, 65, new Vector3(286.5f, 1f, -301f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0.2f);

		AddMonData(en, 4, 0, 1, "Test", "9005", 10, 1.3f, 2560, 65, new Vector3(356.5f, 2.5f, -298.5f), 180, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Boss, 0);
		AddMonData(en, 4, 0, 2, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(359.5f, 2.5f, -285.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);
		AddMonData(en, 4, 0, 3, "Test", "8047", 10, 1.5f, 870, 65, new Vector3(359.5f, 2.5f, -311.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal);

		#endregion

		#region EnFxDun.D5SaintDock
		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.D5SaintDock;
		//---------------------------------------------------------------------------------------------------
		// 1 스텝
		AddMonData(en, 0, 0, 1, "Test", "Saint_1", 10, 2f, 300, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 0, 0, 2, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 0, 0, 3, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 0, 0, 4, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 0, 0, 5, "Test", "Saint_1", 10, 2f, 150, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 0, 0, 6, "Test", "Saint_1", 10, 2f, 150, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 0, 0, 7, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 0, 0, 8, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 0, 0, 9, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 0, 0, 10, "Test", "Saint_1", 10, 2f, 150, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		// 2스텝
		AddMonData(en, 1, 0, 1, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 1, 0, 2, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 1, 0, 3, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 1, 0, 4, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 1, 0, 5, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 1, 0, 6, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 1, 0, 7, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 1, 0, 8, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 1, 0, 9, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 1, 0, 10, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		// 3스텝
		AddMonData(en, 2, 0, 1, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 2, 0, 2, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 2, 0, 3, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 2, 0, 4, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 2, 0, 5, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 2, 0, 6, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 2, 0, 7, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 2, 0, 8, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 2, 0, 9, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 2, 0, 10, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		// 4스텝
		AddMonData(en, 3, 0, 1, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 3, 0, 2, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 3, 0, 3, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 3, 0, 4, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 3, 0, 5, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 3, 0, 6, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 3, 0, 7, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 3, 0, 8, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 3, 0, 9, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 3, 0, 10, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		// 5스텝
		AddMonData(en, 4, 0, 1, "Test", "Saint_1", 10, 2f, 150, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 4, 0, 2, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 4, 0, 3, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 4, 0, 4, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 4, 0, 5, "Test", "Saint_1", 10, 2f, 150, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 4, 0, 6, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 4, 0, 7, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 4, 0, 8, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 4, 0, 9, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 4, 0, 10, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		// 6스텝
		AddMonData(en, 5, 0, 1, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 3f);
		AddMonData(en, 5, 0, 2, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 6f);
		AddMonData(en, 5, 0, 3, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 9f);
		AddMonData(en, 5, 0, 4, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 12f);
		AddMonData(en, 5, 0, 5, "Test", "Saint_2", 10, 3f, 850, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 15f);
		AddMonData(en, 5, 0, 6, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 18f);
		AddMonData(en, 5, 0, 7, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 21f);
		AddMonData(en, 5, 0, 8, "Test", "Saint_4", 10, 2f, 5180, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 24f);
		AddMonData(en, 5, 0, 9, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 27f);
		AddMonData(en, 5, 0, 10, "Test", "Saint_3", 10, 2f, 2580, 65, new Vector3(9.69f, 1f, -4.63f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.SaintDock, 30f);
		#endregion

		#region EnFxDun.D6Chris
		//---------------------------------------------------------------------------------------------------
		en = EnFxDun.D6Chris;
        
        // 1 - 61004
		AddMonData(en, 1, 0, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 40.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 1, 0, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 40.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 1, 0, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 46f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 1, 0, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 46f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 1, 0, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 52f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3f);
		AddMonData(en, 1, 0, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 52f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3f);
		AddMonData(en, 1, 0, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 58f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3f);
		AddMonData(en, 1, 0, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 58f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3f);

		//// 1 - 61007
		//AddMonData(en, 1, 0, 9, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(3.1f, 4f, 41.93f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 10, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(2f, 4f, 47f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(3.1f, 4f, 51.45f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(1.35f, 4f, 55.4f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-2.77f, 4f, 55.4f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-2f, 4f, 51.21f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-0.76f, 4f, 44.6f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-1.9f, 4f, 39.8f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-0.76f, 4f, 44.6f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 0, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(-1.9f, 4f, 39.8f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // 2 - 61002
		//AddMonData(en, 2, 0, 1, "Test", "mon_11001_Barricade", 10, 1.3f, 173, 65, new Vector3(0.7f, 4f, 100.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Object, 0);
		AddMonData(en, 2, 0, 2, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 90f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 3, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 90f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 4, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 91.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 5, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 91.5f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 6, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(-1.5f, 4f, 93f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 7, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(3f, 4f, 93f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);
		AddMonData(en, 2, 0, 8, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(0.75f, 4f, 95f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 3);


        // 2 - 61005
		//AddMonData(en, 2, 0, 9, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(3.1f, 4f, 77.74f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 10, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(3.1f, 4f, 81.08f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 11, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(3.1f, 4f, 83.58f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 12, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-0.05f, 4f, 83.58f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 13, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-0.05f, 4f, 80.59f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 14, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-2.94f, 4f, 78.92f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 15, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-2.94f, 4f, 86.38f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 2, 0, 16, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(1.46f, 4f, 86.38f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);



        // 3 - 61006
		//AddMonData(en, 3, 0, 1, "Test", "La_Tower", 10, 1.3f, 173, 65, new Vector3(0.37f, 4f, 125.76f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Object, 0);
		AddMonData(en, 3, 0, 2, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(3.49f, 4f, 109.97f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 3, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(3.49f, 4f, 114.88f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 4, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(-0.29f, 4f, 114.88f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 5, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(-2.24f, 4f, 118.64f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 6, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(0.92f, 4f, 120.24f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 7, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(2.73f, 4f, 126.29f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 8, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(-3.06f, 4f, 123f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 9, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(-3.06f, 4f, 125.88f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);
		AddMonData(en, 3, 0, 10, "Test", "8038", 10, 1.3f, 173, 65, new Vector3(-0.04f, 4f, 123.4f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.LaChiristiana, 0);


        /*
		// 무한 반복 웨이브
		AddMonData(en, 2, 0, 1, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-20, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 2, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-15, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 3, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-10, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 4, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-20, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 2, 0, 5, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-15, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 2, 0, 6, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(-10, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		// 무한 반복 웨이브
		AddMonData(en, 3, 0, 1, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(-5, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 3, 0, 2, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(0, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 3, 0, 3, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(5, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 3, 0, 4, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(-5, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		AddMonData(en, 3, 0, 5, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(0, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		AddMonData(en, 3, 0, 6, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(5, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		//AddMonData(en, 1, 3, 1, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-60f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 3, 2, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-55f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 3, 3, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-65f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 3, 4, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-60f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 3, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-55f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		//AddMonData(en, 1, 3, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-65f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);

		//AddMonData(en, 1, 4, 1, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-60f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 4, 2, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-55f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 4, 3, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-65f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 4, 4, "Test", "8054", 10, 1.3f, 173, 65, new Vector3(-60f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
		//AddMonData(en, 1, 4, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-55f, 1f, 32f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		//AddMonData(en, 1, 4, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(-65f, 1f, 37f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0f);
		//---------------------------------------------------------------------------------------------------
        */
        #endregion

        #region EnFxDun.D7AnKou
        //---------------------------------------------------------------------------------------------------
		en = EnFxDun.D7AnKou;
		//---------------------------------------------------------------------------------------------------

        Vector3 basePosition = new Vector3(0.4f, 2f, 2.5f);

        // step 1 - 30 - 241001
        AddMonData(en, 1, 0, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 11, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 12, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 13, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 14, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 15, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 16, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 17, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 18, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 19, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 20, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 21, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 22, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 23, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 24, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 25, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 26, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 27, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 28, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 29, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 0, 30, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 2 - 241001
        AddMonData(en, 1, 1, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 2 - 41002
        AddMonData(en, 1, 1, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 19, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 20, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 21, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 22, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 23, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 24, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 25, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 26, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 27, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 28, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 29, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 1, 30, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);


        // step 3 - 241001
        AddMonData(en, 1, 2, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 3 - 41002
        AddMonData(en, 1, 2, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 19, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 20, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 3 - 41003
        AddMonData(en, 1, 2, 21, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 22, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 23, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 24, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 25, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 26, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 27, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 28, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 29, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 2, 30, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);


        // step 4 - 241001
        AddMonData(en, 1, 3, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 4 - 41002
        AddMonData(en, 1, 3, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 19, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 20, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 4 - 41003
        AddMonData(en, 1, 3, 21, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 22, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 23, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 24, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 25, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 26, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 27, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 28, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 29, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 30, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 4 - 41004
        AddMonData(en, 1, 3, 31, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 32, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 33, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 34, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 35, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 36, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 37, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 38, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 39, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 3, 40, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);


        // step 5 - 241001
        AddMonData(en, 1, 4, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 5 - 41002
        AddMonData(en, 1, 4, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 19, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 20, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 5 - 41003
        AddMonData(en, 1, 4, 21, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 22, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 23, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 24, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 25, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 26, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 27, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 28, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 29, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 30, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 5 - 41004
        AddMonData(en, 1, 4, 31, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 32, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 33, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 34, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 35, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 36, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 37, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 38, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 39, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 4, 40, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);


        // step 6 - 241001
        AddMonData(en, 1, 5, 1, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 2, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 3, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 4, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 5, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 6, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 7, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 8, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 9, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 10, "Test", "8052", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 6 - 41002
        AddMonData(en, 1, 5, 11, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 12, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 13, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 14, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 15, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 16, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 17, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 18, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 19, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 20, "Test", "8002", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 6 - 41003
        AddMonData(en, 1, 5, 21, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 22, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 23, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 24, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 25, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 26, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 27, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 28, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 29, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 30, "Test", "8025", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 6 - 41004
        AddMonData(en, 1, 5, 31, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 32, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 33, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 34, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 35, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 36, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 37, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 38, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 39, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);
        AddMonData(en, 1, 5, 40, "Test", "8013", 10, 1.3f, 173, 65, new Vector3(basePosition.x + UnityEngine.Random.RandomRange(-15f, 15f), basePosition.y, basePosition.z + UnityEngine.Random.RandomRange(-15f, 15f)), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Normal, 0);

        // step 6 - Boss
		AddMonData(en, 1, 5, 41, "Test", "9005", 10, 1.3f, 2560, 65, new Vector3(13f, 2, 3f), 0, CsMonster.EnOwnership.Controller, MonsterInstanceType.TreatOfFarmQuestMonster, EnMonType.Boss, 0);

		#endregion
	}

}
