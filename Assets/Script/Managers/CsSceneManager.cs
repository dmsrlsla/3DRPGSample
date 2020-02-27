using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CsSceneManager : MonoBehaviour
{
    CsMyPlayer m_csPlayer;
    int m_MonWave;
    protected List<GameObject> m_listMonster = new List<GameObject>();
    private void Awake()
    {
        Transform trPos = transform.Find("StartPos");
        CreateHero(trPos.position, 90, "Brute");
        SceneManager.LoadScene("MainUI", LoadSceneMode.Additive);
    }

    protected void CreateHero(Vector3 vtPos, float flRotationY, string strHeroName)
    {
        Transform trPlayerManager = transform.Find("PlayerManager");

        GameObject goPlayer =
            Instantiate(Resources.Load("Prefab/Player/My/" + strHeroName)
            , vtPos, Quaternion.Euler(new Vector3(0f, flRotationY, 0)), 
            trPlayerManager) as GameObject;

        CsGameData.Instance.HeroMid = 1;

        CsGameData.Instance.MyHeroTransform = goPlayer.transform;
    }

    protected void CreateMonster(CsMonster Mon)
    {
        
        Transform trMonsterManager = transform.Find("MonsterList");
        GameObject go = (Instantiate(Resources.Load("Prefab/Mon/" + Mon.MonName), Mon.CreatePos,
                Quaternion.Euler(new Vector3(0f, Mon.RotationY, 0)), trMonsterManager) as GameObject);
        if (go.GetComponent<CsMonster>() != null)
        {
            //Debug.LogError("몬스터 포즈 : " + Mon.CreatePos);
            //Debug.LogError("몬스터 이름 : " + Mon.MonName);
            go.GetComponent<CsMonster>().InitMonster(Mon.MonName, Mon.MaxHP, Mon.Hp, Mon.AttackDamage, Mon.CreatePos, Mon.RotationY, Mon.MonsterID);
        }
        else
        {
            Debug.LogError("못찾겠다 꾀꼬리");
        }

        Transform trHUDPos = go.transform.Find("HUDPos");

        Instantiate(Resources.Load("Prefab/UI/HUDCanvas"), trHUDPos.position, trHUDPos.rotation, trHUDPos);

        m_listMonster.Add(go);
    }
}
