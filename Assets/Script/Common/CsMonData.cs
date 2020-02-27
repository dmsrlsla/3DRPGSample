using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsMonData : MonoBehaviour
{
    public static CsMonData Instance
    {
        get { return CsSingleton<CsMonData>.GetInstance(); }
    }

    Dictionary<int, List<CsMonster>> m_DicMonsterdata = new Dictionary<int, List<CsMonster>>();

    public Dictionary<int, List<CsMonster>> DicMon { get { return m_DicMonsterdata; } }

    void AddData(CsMonster monster, int nWave)
    {

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
