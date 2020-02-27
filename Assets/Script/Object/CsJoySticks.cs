using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CsJoySticks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler , IEndDragHandler
{
    RectTransform m_rtfHandle;
    Vector2 m_vtCanvasResolution;
    float m_flRange;

    // Start is called before the first frame update
    void Start()
    {
        m_vtCanvasResolution = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution;
        m_rtfHandle = transform.Find("JoyStick").GetComponent<RectTransform>();
        m_flRange = Mathf.Abs(transform.GetComponent<RectTransform>().sizeDelta.x / 3);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CsGameData.Instance.JoysticDragging = true;
        CsGameEvent.Instance.OnEventChangeState(EnState.MoveByJoystic);
        SetHandlePosition(eventData.position);
        JoystickCosValue();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CsGameData.Instance.JoysticDragging = true;
        CsGameEvent.Instance.OnEventChangeState(EnState.MoveByJoystic);
        SetHandlePosition(eventData.position);
        JoystickCosValue();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
    }

    //---------------------------------------------------------------------------------------------------
    void SetHandlePosition(Vector2 vtPoint)
    {
        // 조이스틱의 앵글을 연산한다.
        Vector2 vt = new Vector2(vtPoint.x * m_vtCanvasResolution.x / Screen.width, vtPoint.y * m_vtCanvasResolution.y / Screen.height);
        Vector2 vt2 = vt - transform.GetComponent<RectTransform>().anchoredPosition;
        if (Vector2.Distance(Vector2.zero, vt2) < m_flRange)
        {
            m_rtfHandle.anchoredPosition = vt2;
        }
        else
        {
            m_rtfHandle.anchoredPosition = vt2.normalized * m_flRange;
        }
        CsGameData.Instance.JoysticAngle = Mathf.Atan2(m_rtfHandle.anchoredPosition.y, m_rtfHandle.anchoredPosition.x);
    }

    //---------------------------------------------------------------------------------------------------
    public void JoystickCosValue()
    {
        CsGameData.Instance.JoysticCosValue = Mathf.Abs(Mathf.Cos(CsGameData.Instance.JoysticAngle));
        CsGameData.Instance.JoysticCosValue = Mathf.Round(CsGameData.Instance.JoysticCosValue * 10f) / 10f;
    }

    //---------------------------------------------------------------------------------------------------
    public void Reset()
    {
        if (CsGameData.Instance.JoysticDragging)
        {
            CsGameData.Instance.JoysticDragging = false;
            CsGameEvent.Instance.OnEventChangeState(EnState.Idle);
            m_rtfHandle.anchoredPosition = Vector2.zero;
        }
    }
}
