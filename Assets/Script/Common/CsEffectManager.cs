using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsEffectManager : MonoBehaviour
{

    static CsEffectManager s_instance;

    // 이펙트 관련 저장
    [Tooltip("이펙트를 Prefab 폴더에 저장하고, 리스트에 동일하게 등록해둡니다.")]
    [SerializeField]
    public List<GameObject> m_listEffects = new List<GameObject>();

    [SerializeField]
    float m_flEffectTime = 1;

    [SerializeField]
    Vector3 m_vtOffset;

    Dictionary<string, GameObject> m_dicEffect = new Dictionary<string, GameObject>();
    // 사운드 클립 관련 저장(임시.)
    Dictionary<string, AudioClip> m_dicSound = new Dictionary<string, AudioClip>();

    Coroutine m_coroutine = null;

    public static CsEffectManager Instance
    {
        get { return s_instance; }
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        Init();
    }

    public void Init()
    {
        m_coroutine = StartCoroutine(AsnyLoadEffect());
    }

    IEnumerator AsnyLoadEffect()
    {
        Debug.Log("StartSoundLoadBundleAssetAsync()");

        for (int i = 0; i < m_listEffects.Count; i++)
        {
            ResourceRequest req = Resources.LoadAsync<GameObject>("Prefab/Effect/" + m_listEffects[i].name);
            yield return req;

            if (req == null)
            {
                Debug.Log("StartSoundLoadBundleAssetAsync     req == null " + m_listEffects[i].name); continue;
            }
            else
            {
                if (req.asset == null)
                {
                    Debug.Log("StartSoundLoadBundleAssetAsync     req.asset == null " + m_listEffects[i].name); continue;
                }
            }

            if (!m_dicEffect.ContainsKey(m_listEffects[i].name))
            {
                GameObject goEffect = Instantiate((GameObject)req.asset, Vector3.zero, Quaternion.identity) as GameObject;
                goEffect.transform.name = m_listEffects[i].ToString();
                goEffect.transform.SetParent(gameObject.transform);
                goEffect.SetActive(false);
                m_dicEffect.Add(m_listEffects[i].name, goEffect);
            }
        }
    }

    public void PlayEffect(string strEffectName, Transform trOwner, Vector3 vtCreatePos, Quaternion qtnRotation, float flSec, float flRotationY = 0f)
    {
        StartCoroutine(NormalEffect(strEffectName, transform, vtCreatePos, transform.rotation, flSec, flRotationY));
    }

    public void PlayEffectTake2(string strEffectName, Transform trOwner, Vector3 vtCreatePos, Quaternion qtnRotation, float flRotationY = 0f, float m_flTime = 1)
    {
        StartCoroutine(NormalEffect(strEffectName, trOwner, vtCreatePos, transform.rotation, m_flEffectTime, flRotationY, m_flTime));
    }

    //public void PlayMoveHitEffect(Transform trHero, Vector3 vtTargetPos, EnInGameEffect enEffectArrow, EnInGameEffect enEffectHit, float flHeightOffset = 0, bool bSmoke = false)
    //{
    //    StartCoroutine(MoveByHitEffect(trHero, vtTargetPos, trHero.rotation, enEffectArrow, enEffectHit, flHeightOffset, bSmoke));
    //}

    IEnumerator NormalEffect(string strEffect, Transform trOwner, Vector3 vtPosition, Quaternion qtnRotation, float flSec, float flRotationY = 0f, float m_flTime = 1)
    {
        if (m_dicEffect.ContainsKey(strEffect))
        {
            Vector3 vtNew = new Vector3(vtPosition.x + m_vtOffset.x, vtPosition.y + m_vtOffset.y, vtPosition.z + m_vtOffset.z);
            GameObject goEffect = Instantiate(m_dicEffect[strEffect].gameObject, vtNew, qtnRotation, trOwner);

            if (flRotationY != 0)
            {
                goEffect.transform.eulerAngles = new Vector3(0f, flRotationY, 0f);
            }

            goEffect.gameObject.SetActive(true);

            yield return new WaitForSeconds(m_flTime);

            if (goEffect != null)
            {
                Destroy(goEffect);
            }
        }
        else
        {
            Debug.LogError("안됨병시나");
        }
    }

    //IEnumerator MoveByHitEffect(Transform trAttacker, Vector3 vtTargetPos, Quaternion qtnRotation, EnInGameEffect enEffectArrow, EnInGameEffect enEffectHit, float flHeightOffset, bool bSmoke)
    //{
    //    if (m_dicInGameEffect.ContainsKey(enEffectArrow))
    //    {
    //        float flDistance = 1f;
    //        float flTimer = Time.time;
    //        GameObject goHit = null;
    //        GameObject goSmoke = null;

    //        Vector3 vtCreatePos = new Vector3(trAttacker.position.x, trAttacker.position.y + flHeightOffset, trAttacker.position.z);
    //        GameObject goArrow = Instantiate(m_dicInGameEffect[enEffectArrow].gameObject, vtCreatePos, qtnRotation, transform);
    //        goArrow.transform.eulerAngles = new Vector3(0f, GetAngle(trAttacker.position, vtTargetPos), 0f);
    //        goArrow.SetActive(true);

    //        if (trAttacker == CsGameData.Instance.MyHeroTransform && Vector3.Distance(trAttacker.position, vtTargetPos) > 5)
    //        {
    //            flDistance = 1.5f;
    //        }

    //        if (m_dicInGameEffect.ContainsKey(enEffectHit)) // 피격 Effect 생성.
    //        {
    //            goHit = Instantiate(m_dicInGameEffect[enEffectHit].gameObject, vtTargetPos, qtnRotation, transform);
    //        }

    //        if (bSmoke && CsConfiguration.Instance.GetSettingKey(EnPlayerPrefsKey.Graphic) > 1) // 투사체 스모크 생성(그래픽설정이 중이상 일때만).
    //        {
    //            if (m_dicInGameEffect.ContainsKey(EnInGameEffect.Deva_Smoke))
    //            {
    //                goSmoke = Instantiate(m_dicInGameEffect[EnInGameEffect.Deva_Smoke].gameObject, vtCreatePos, qtnRotation, transform);
    //                goSmoke.transform.eulerAngles = new Vector3(0f, GetAngle(trAttacker.position, vtTargetPos), 0f);
    //                goSmoke.SetActive(true);
    //            }
    //        }


    //        while (goArrow.transform.position != vtTargetPos)
    //        {
    //            if (Vector3.Distance(goArrow.transform.position, vtTargetPos) < flDistance)
    //            {
    //                if (goHit != null)
    //                {
    //                    goHit.transform.position = goArrow.transform.position;
    //                }
    //                break;
    //            }
    //            else
    //            {
    //                if (Time.time - flTimer > 2)
    //                {
    //                    break;
    //                }
    //            }

    //            goArrow.transform.position = Vector3.MoveTowards(goArrow.transform.position, vtTargetPos, Time.deltaTime * 28); // 제일 긴 거리의 0.3초 동안 속도 = 28 .

    //            if (goSmoke != null)
    //            {
    //                goSmoke.transform.position = goArrow.transform.position;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }

    //        Destroy(goArrow);

    //        if (goHit != null)
    //        {
    //            goHit.SetActive(true);
    //        }

    //        yield return new WaitForSeconds(1f);

    //        if (goSmoke != null) // 투사체 스모크 지연 삭제.
    //        {
    //            Destroy(goSmoke);
    //        }

    //        if (goHit != null)
    //        {
    //            yield return new WaitForSeconds(0.5f);
    //            Destroy(goHit);
    //        }
    //    }
    //}

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
