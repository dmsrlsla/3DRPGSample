using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CsPanelFade : MonoBehaviour
{
    Transform m_trImageFade;
    Transform m_trImageDungeon;

    Text m_textDungeonName;
    Text m_textDungeonDescription;

    IEnumerator m_iEFadeIn;
    IEnumerator m_iEFadeOut;

    bool m_bStart = true;

    private void Awake()
    {
        CsGameEvent.Instance.EventFade += OnEventFade;
        m_trImageFade = transform.Find("ImageFade");
        m_trImageDungeon = transform.Find("ImageDungeon");

        m_textDungeonName = m_trImageDungeon.Find("TextName").GetComponent<Text>();
        m_textDungeonDescription = m_trImageDungeon.Find("TextDescription").GetComponent<Text>();

        m_trImageFade.gameObject.SetActive(false);
        m_trImageDungeon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CsGameEvent.Instance.EventFade -= OnEventFade;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEventFade(bool bFade, float flTime)
    {
        if(bFade)
        {
            StartFadeIn(m_trImageDungeon, 0.5f);
            StartFadeIn(m_trImageFade, flTime);
            m_textDungeonName.text = "피의 해안가";
            m_textDungeonDescription.text = "몬스터의 생성을 저지하라";
        }
        else
        {
            StartFadeOut(m_trImageDungeon, flTime);
            StartFadeOut(m_trImageFade, flTime);
        }


    }

    IEnumerator FadeIn(Transform trImageFade, float flTime)
    {
        //서서히 밝게 한다.
        CanvasGroup canvasGroup = trImageFade.GetComponent<CanvasGroup>();

        yield return new WaitUntil(() => canvasGroup.alpha == 1);

        yield return new WaitForSeconds(0.5f);

        for (float fl = 0; fl <= flTime; fl += Time.deltaTime)
        {
            canvasGroup.alpha = 1 - (fl / flTime);
            yield return null;
        }

        trImageFade.gameObject.SetActive(false);

        //if (trImageFade == m_trImageFade && m_bStart)
        //{
        //    StartFadeIn(trImageFade, 0.5f);
        //}
    }

    //---------------------------------------------------------------------------------------------------
    void StartFadeIn(Transform trImageFade, float flDuration)
    {
        if (m_iEFadeOut != null)
        {
            StopCoroutine(m_iEFadeOut);
            m_iEFadeOut = null;
        }

        if (m_iEFadeIn != null)
        {
            StopCoroutine(m_iEFadeIn);
            m_iEFadeIn = null;
        }

        trImageFade.gameObject.SetActive(true);
        trImageFade.GetComponent<CanvasGroup>().alpha = 1;

        m_iEFadeIn = FadeIn(trImageFade, flDuration);
        StartCoroutine(m_iEFadeIn);
    }

    //---------------------------------------------------------------------------------------------------
    void StartFadeOut(Transform trImageFade, float flTime)
    {
        if (m_iEFadeOut != null)
        {
            StopCoroutine(m_iEFadeOut);
            m_iEFadeOut = null;
        }

        if (m_iEFadeIn != null)
        {
            StopCoroutine(m_iEFadeIn);
            m_iEFadeIn = null;
        }

        trImageFade.gameObject.SetActive(false);
        trImageFade.GetComponent<CanvasGroup>().alpha = 0;


        m_iEFadeOut = FadeOut(trImageFade, flTime);
        StartCoroutine(m_iEFadeOut);
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator FadeOut(Transform trImageFade, float flTime)
    {
        //서서히 어둡게 한다
        CanvasGroup canvasGroup = trImageFade.GetComponent<CanvasGroup>();

        for (float fl = 0; fl <= flTime; fl += Time.deltaTime)
        {
            canvasGroup.alpha = fl / flTime;
            yield return null;
        }

        canvasGroup.alpha = 1;

        //if (trImageFade == m_trImageDungeon)
        //{
        //    StartFadeIn(trImageFade, 0.5f);
        //    //CsGameEventToIngame.Instance.OnEventStartDirection();
        //}
    }
}
