using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnCameraState
{
    Auto, // 자동 추적 카메라
    Manual, // 기본 일반 터치카메라
    QuarterView, // 던전카메라
    Dungeon,
    Zoom // 줌모드
}

public enum EnActionCameraType { None, Enter1, Enter2, Boss, Clear, Ride, Taming } // 연출 카메라 타입.

public class CsIngameCamera : MonoBehaviour
{
    protected class CameraValue
    {

        protected float m_flLength;
        protected float m_flHeight;
        protected float m_flPivot2D_X;
        protected float m_flPivot2D_Y;
        protected float m_flUpAndDownValue;
        protected float m_flRightAndLeftValue;
        protected float m_flZoom;
        protected float m_flScreenYHeight = 0f;

        public float Length { get { return m_flLength; } set { m_flLength = value; } }
        public float Height { get { return m_flHeight; } set { m_flHeight = value; } }
        public float Pivot2D_X { get { return m_flPivot2D_X; } set { m_flPivot2D_X = value; } }
        public float Pivot2D_Y { get { return m_flPivot2D_Y; } set { m_flPivot2D_Y = value; } }
        public float UpAndDown { get { return m_flUpAndDownValue; } set { m_flUpAndDownValue = value; } }
        public float RightAndLeft { get { return m_flRightAndLeftValue; } set { m_flRightAndLeftValue = value; } }
        public float Zoom { get { return m_flZoom; } set { m_flZoom = value; } }
        public float ScreenYHeight { get { return m_flScreenYHeight; } set { m_flScreenYHeight = value; } }

        public CameraValue(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom)
        {
            m_flLength = flLength;
            m_flHeight = flHeight;
            m_flPivot2D_X = flPivot2D_X;
            m_flPivot2D_Y = flPivot2D_Y;
            m_flUpAndDownValue = flUpAndDownValue;
            m_flRightAndLeftValue = flRightAndLeftValue;
            m_flZoom = flZoom;
        }

        public CameraValue(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom, float flScreenYHeight)
        {
            m_flLength = flLength;
            m_flHeight = flHeight;
            m_flPivot2D_X = flPivot2D_X;
            m_flPivot2D_Y = flPivot2D_Y;
            m_flUpAndDownValue = flUpAndDownValue;
            m_flRightAndLeftValue = flRightAndLeftValue;
            m_flZoom = flZoom;
            m_flScreenYHeight = flScreenYHeight;
        }
    }

    [System.Serializable]
    public class DungeonEnterStep_1_Set
    {
        [Range(0f, 50.0f)]
        public float Delay;
        public float Duration;

        [Range(-15.0f, 15.0f)]
        public float Start_Length;
        [Range(-15.0f, 15.0f)]
        public float Start_Height;
        [Range(-15.0f, 15.0f)]
        public float Start_RightAndLeftValue;
        [Range(-15.0f, 15.0f)]
        public float Start_ScreenYHeight;
        [Range(-10.0f, 10.0f)]
        public float Start_Zoom;

        [Range(-15.0f, 15.0f)]
        public float End_Length;
        [Range(-15.0f, 15.0f)]
        public float End_Height;
        [Range(-15.0f, 15.0f)]
        public float End_RightAndLeftValue;
        [Range(-15.0f, 15.0f)]
        public float End_ScreenYHeight;
        [Range(-10.0f, 10.0f)]
        public float End_Zoom;
    }
    public DungeonEnterStep_1_Set m_dungeonEnterStep_1_Set = new DungeonEnterStep_1_Set();

    [System.Serializable]
    public class DungeonEnterStep_2_Set
    {
        [Range(0f, 50.0f)]
        public float Delay;

        public float Duration;

        [Range(-15.0f, 15.0f)]
        public float End_Length;
        [Range(-15.0f, 15.0f)]
        public float End_Height;
        [Range(-15.0f, 15.0f)]
        public float End_RightAndLeftValue;
        [Range(-15.0f, 15.0f)]
        public float End_ScreenYHeight;
        [Range(-5.0f, 5.0f)]
        public float End_Zoom;
    }
    public DungeonEnterStep_2_Set m_dungeonEnterStep_2_Set = new DungeonEnterStep_2_Set();

    [System.Serializable]
    public class DungeonTaming
    {
        [Range(0f, 50.0f)]
        public float Delay;

        public float Duration;

        [Range(-15.0f, 15.0f)]
        public float End_Length;
        [Range(-15.0f, 15.0f)]
        public float End_Height;
        [Range(-15.0f, 15.0f)]
        public float End_RightAndLeftValue;
        [Range(-15.0f, 15.0f)]
        public float End_UpAndDonwValue;
        [Range(-15.0f, 15.0f)]
        public float End_ScreenYHeight;
        [Range(-5.0f, 5.0f)]
        public float End_Zoom;
    }
    public DungeonTaming m_dungeonTaming = new DungeonTaming();

    EnActionCameraType m_enActionCamera = EnActionCameraType.None;

    public EnActionCameraType ActionCamera { get { return m_enActionCamera; } set { m_enActionCamera = value; } }

    float m_flDelayWatingTime = 0.0f;

    public float DelayWatingTime { get { return m_flDelayWatingTime; } set { m_flDelayWatingTime = value; } }


    [Header("Default Set")]
    protected const float c_flAdditionalHeight = 1.05f;
    protected Camera m_camera;

    protected Texture m_txBlackWhite;

    protected Vector3 m_vtCenter = Vector3.zero;
    protected Vector2 m_vtPrevPos = Vector2.zero;

    protected float m_flfarClipPlane = 0f;
    protected float m_flRadius = 10f;
    protected float m_flDialogTimer = 0f;
    public float m_flOrgUpAndDownValue;
    protected float m_flOrgLength;
    protected float m_flOrgHeight;
    protected float m_flOrgPivot2D_X;
    protected float m_flOrgPivot2D_Y;

    protected float m_flOrgRightAndLeftValue;
    protected float m_flOrgZoom;

    protected float c_flCameraDragSpeed = 0.001f;
    protected float m_flPrevNear;
    protected int m_nLayerMask;

    protected float m_flMinZoom = 1.28f;
    protected float m_flMaxZoom = 0.5f;
    protected float m_flMinCameraHeight = 5f;
    protected float m_flMaxCameraHeight = 4f;

    protected float m_flMinQurtarViewZoom = 0.7f;
    protected float m_flMaxQurtarViewZoom = 0.45f;

    protected bool m_bZoom = false;
    protected bool m_bDirecting = false;
    protected bool m_bAction = false;
    protected bool m_bActionStep2 = false;

    protected bool m_bBossAppearance = false;

    protected bool m_bShaking = false;

    protected float m_flShakeDecay;
    protected float m_flShakeIntensity;
    protected Quaternion m_qtnOriginalRot;

    public float m_flWidthOffset = 0.05f;

    protected float m_flUpAndDownOffest = 0;

    public Quaternion m_qGyro;


    [Range(0.1f, 10f)]
    protected float m_flFowardMax = 3.3f;
    protected float m_flZoomOffset = 0f;



    protected CameraValue m_AutoStateValue;
    protected CameraValue m_ManualStateValue;
    protected CameraValue m_QuarterViewStateValue;
    protected CameraValue m_GyroscopeStateValue;

    protected Transform m_trCameraTarget = CsGameData.Instance.MyHeroTransform;

    protected IEnumerator ieCoroutine;
    protected IEnumerator ieMenualCoroutine;
    protected IEnumerator ieAutoCoroutine;
    protected IEnumerator ieQuarterViewCoroutine;

    public EnCameraState m_enCameraState = EnCameraState.Auto;

    public bool m_bDungeonOutCamera = false;

    [SerializeField]
    protected float m_fl1D_GDistance = 25;
    [SerializeField]
    protected float m_fl2D_GDistance = 45;
    [SerializeField]
    protected float m_fl3D_GDistance = 100;

    [Range(-10f, 10f)]
    public float m_flForwardOffset = 0.15f; // 캐릭터 전방 카메라 이동

    [Range(0.1f, 20f)]
    public float m_flLength = 5.5f; // 카메라가 캐릭터로부터 떨어져있는 z축 길이

    [Range(0.1f, 20f)]
    public float m_flHeight = 4.5f; // 카메라가 캐릭터로부터 떨어져있는 Y축 길이(높이)

    [Range(-10f, 10f)]
    public float m_flScreenYHeight = 0.4f; // Screen화면을 직접 바꾸는 Y좌표(X Screen 생략)

    [Range(-10f, 10f)]
    protected float m_flHeightOffset = 1f; // 카메라의 높이 오프셋(플레이어 키)
    [Range(-10f, 10f)]
    protected float m_flHeightPlayerCenter = CsGameData.Instance.HeroMid;

    [Range(-10f, 10f)]
    public float m_flPivot2D_X = 0f; // 중심좌표을 기준으로 한 Y좌표 변동값

    [Range(-10f, 10f)]
    public float m_flPivot2D_Y = 0.5f; // 중심좌표을 기준으로 한 Y좌표 변동값

    [Range(-1f, 1f)]
    public float m_flUpAndDownValue = 0.2f; // 위 아래 회전반경(반구형)

    [Range(0f, 6.28f)]
    public float m_flRightAndLeftValue = 1.57f; // 좌우 회전 반경

    [Range(0.4f, 1.5f)]
    public float m_flZoom = 1.05f; // 카메라 줌. 캐릭터로부터 얼마나 떨어져있는지 세팅.

    float m_flDefaultFOV;

    public bool m_bCheckBoostMode = false;
    public bool m_bCheckDefaultMode = false;

    public Camera Camera { get { return m_camera; } }

    public float Length { get { return m_flLength; } set { m_flLength = value; } }
    public float Height { get { return m_flHeight; } set { m_flHeight = value; } }
    public float Pivot2D_X { get { return m_flPivot2D_X; } set { m_flPivot2D_X = value; } }
    public float Pivot2D_Y { get { return m_flPivot2D_Y; } set { m_flPivot2D_Y = value; } }
    public float UpAndDown { get { return m_flUpAndDownValue; } set { m_flUpAndDownValue = value; } }
    public float RightAndLeft { get { return m_flRightAndLeftValue; } set { m_flRightAndLeftValue = value; } }
    public float Zoom { get { return m_flZoom; } set { m_flZoom = value; } }
    public float HeightOffset { get { return m_flHeightOffset; } set { m_flHeightOffset = value; } }
    public float HeightPlayerCenter { get { return m_flHeightPlayerCenter; } set { m_flHeightPlayerCenter = value; } }

    public bool ZoomPlay { get { return m_bZoom; } set { m_bZoom = value; } }
    public bool Action { get { return m_bAction; } set { m_bAction = value; } }
    public bool ActionStep2 { get { return m_bActionStep2; } set { m_bActionStep2 = value; } }
    public bool BossAppearance { get { return m_bBossAppearance; } set { m_bBossAppearance = value; } }
    public float UpAndDownOffest { get { return m_flUpAndDownOffest; } set { m_flUpAndDownOffest = value; } }
    public float FarClipPlane { get { return m_flfarClipPlane; } }

    public Transform trCameraPos { get { return m_trCameraTarget; } set { m_trCameraTarget = value; } }

    CameraValue m_defaultCameraSet;
    CameraValue m_LoginZoomValue;
    float m_flTempZoom;
    bool bSetRushCamera = false;
    float m_flResetCameraFromDush = 1.0f;
    float m_flResetCameraDefaultAtAuto = 1.0f;
    float m_flSetCameraAtStartDush = 1.0f;
    float m_flTargetZoom;
    float m_flTargetFOV;
    float m_flShareUpAndDown;
    float m_flShareRightAndLeft;
    float m_flShareHeight;
    float m_flShareZoom;
    bool m_bFirst = true;
    float m_flFirstTimer = 0.0f;
    bool m_bOneSet = true;
    float m_flCurrentValue;
    float m_flFirstCameraMoveDelta;
    //---------------------------------------------------------------------------------------------------
    protected void Awake()
    {
        m_camera = GetComponent<Camera>();
        CsGameData.Instance.InGameCamera = this;

        CsGameEvent.Instance.EventChangeCameraState += OnEventChangeCameraState;
        CsGameEvent.Instance.EventChangeCameraDistance += OnEventChangeCameraState;

        // Dead 연출 Setting
        m_txBlackWhite = Resources.Load<Texture>("BlackWhite");

        m_flPrevNear = m_camera.nearClipPlane;
        m_flfarClipPlane = m_camera.farClipPlane;
        m_nLayerMask = m_camera.cullingMask;
        m_camera.depthTextureMode = DepthTextureMode.Depth; //동작 안하면 물이 정상적으로 처리가 안됨. AQUAS
        m_flWidthOffset = 0.05f;
        m_flForwardOffset = 0.15f;
        m_flLength = m_flOrgLength = 0.9f;
        m_flHeight = m_flOrgHeight = 1f;
        m_flPivot2D_X = m_flOrgPivot2D_X = 0f;
        m_flPivot2D_Y = m_flOrgPivot2D_Y = 0.5f;
        m_flScreenYHeight = 0.2f;
        m_flUpAndDownValue = m_flOrgUpAndDownValue = 0.2f;
        m_flRightAndLeftValue = m_flOrgRightAndLeftValue = 0.7853f;   //  Mathf.PI / 4;
        m_flZoom = m_flOrgZoom = 1.05f;

        InitializeHeightInfo(); // Heightmap 값 세팅.

        float[] aflDistance = new float[32]; // 거리에 따른 원경 처리 레이어 설정.
        aflDistance = Camera.main.layerCullDistances;

        Camera.main.layerCullDistances = aflDistance;

        m_trCameraTarget = CsGameData.Instance.MyHeroTransform;
        m_flDefaultFOV = transform.GetComponent<Camera>().fieldOfView;
        #region 입장연출 관련
        // 입장 연출 스텝1
        m_dungeonEnterStep_1_Set.Delay = 1.3f;
        m_dungeonEnterStep_1_Set.Duration = 1.2f;
        m_dungeonEnterStep_1_Set.Start_Length = 2.5f;
        m_dungeonEnterStep_1_Set.Start_Height = 1f;
        m_dungeonEnterStep_1_Set.Start_RightAndLeftValue = 1.57f;
        m_dungeonEnterStep_1_Set.Start_ScreenYHeight = 0.6f;
        m_dungeonEnterStep_1_Set.Start_Zoom = 1.25f;

        m_dungeonEnterStep_1_Set.End_Length = 2.5f;
        m_dungeonEnterStep_1_Set.End_Height = 1.8f;
        m_dungeonEnterStep_1_Set.End_RightAndLeftValue = 0;
        m_dungeonEnterStep_1_Set.End_ScreenYHeight = 0.3f;
        m_dungeonEnterStep_1_Set.End_Zoom = 1.05f;

        // 입장 연출 스텝2
        m_dungeonEnterStep_2_Set.Delay = 0.2f;
        m_dungeonEnterStep_2_Set.Duration = 2f;

        m_dungeonEnterStep_2_Set.End_Length = 1f;
        m_dungeonEnterStep_2_Set.End_Height = 1f;
        m_dungeonEnterStep_2_Set.End_RightAndLeftValue = 0;
        m_dungeonEnterStep_2_Set.End_ScreenYHeight = 0.1f;
        m_dungeonEnterStep_2_Set.End_Zoom = 0.85f;

        // 테이밍 연출
        m_dungeonTaming.Delay = 2f;
        m_dungeonTaming.Duration = 4f;

        m_dungeonTaming.End_Length = 10f;
        m_dungeonTaming.End_Height = -3f;
        m_dungeonTaming.End_RightAndLeftValue = 1.57f;
        m_dungeonTaming.End_ScreenYHeight = 5f;
        m_dungeonTaming.End_UpAndDonwValue = -0.75f;
        m_dungeonTaming.End_Zoom = 1.8f;
        #endregion 입장연출 관련
    }

    //---------------------------------------------------------------------------------------------------
    protected void Start()
    {
        //m_flRightAndLeftValue = CsGameData.Instance.IngameManagement.MyPlayer().transform.eulerAngles.y * Mathf.Deg2Rad;
        m_flRightAndLeftValue = CsGameData.Instance.MyHeroTransform.eulerAngles.y * Mathf.Deg2Rad;
        Debug.Log("CsInGameCamera Start() ");
        Input.gyro.enabled = true;
        m_AutoStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
        m_ManualStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
        m_QuarterViewStateValue = new CameraValue(6.5f, 8f, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, 0.9f);
        m_GyroscopeStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
        m_defaultCameraSet = new CameraValue(m_AutoStateValue.Length, m_AutoStateValue.Height, m_AutoStateValue.Pivot2D_X, m_AutoStateValue.Pivot2D_Y, m_flUpAndDownValue, m_AutoStateValue.RightAndLeft, m_AutoStateValue.Zoom);
        Debug.Log("m_QuarterViewStateValue  === " + m_QuarterViewStateValue.Height);
        // hun dev
        // 최초 로그인시 줌인 값
        Debug.Log("Auto   ===  " + m_flRightAndLeftValue);
        Debug.Log(" CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y " + CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        float flSetRightAndLeft = 3.0f + CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        if (flSetRightAndLeft > 6.28f)
        {
            flSetRightAndLeft = flSetRightAndLeft - 6.28f;
        }
        m_LoginZoomValue = new CameraValue(2.3f, 3.2f, 0.3f, 0.43f, 0.25f, flSetRightAndLeft, 1.5f);
        if (m_enCameraState == EnCameraState.Zoom)
        {
            LoginZoomIn();
        }
        else if (m_enCameraState != EnCameraState.Zoom)
        {
            ChangeNewState(EnCameraState.Auto);//CsIngameData.Instance.InGameCameraState);
        }
        //ChangeNewState(CsIngameData.Instance.InGameCameraState);
        m_flHeightPlayerCenter = CsGameData.Instance.HeroMid;
        m_trCameraTarget = CsGameData.Instance.MyHeroTransform;
    }

    //---------------------------------------------------------------------------------------------------
    public void Update()
    {
        if (CsGameData.Instance.MyHeroTransform == null) return; // 메인 플레이어 정보가 없다면, 카메라 업데이트 안함.

        // 카메라의 타겟포지션 + Offset값을 기준으로한, 카메라가 바라볼 곳을 계산
        m_vtCenter = m_trCameraTarget.position + (new Vector3(0f, m_flHeightOffset, m_flWidthOffset) + (m_trCameraTarget.forward * m_flForwardOffset) + (m_trCameraTarget.up * m_flScreenYHeight));  m_flHeight = 1;
        // 카메라가 대상에서 얼마나 떨어져 있는지, 반경을 계산

        m_flRadius = Mathf.Sqrt(m_flLength * m_flLength * 2 + m_flHeight * m_flHeight);
        // 카메라가 어떤 각도에서 카메라를 바라볼지 계싼
        Quaternion qtn = Quaternion.Euler((Mathf.Asin(m_flHeight / m_flRadius) - (m_flUpAndDownValue + m_flUpAndDownOffest)) * Mathf.Rad2Deg, m_flRightAndLeftValue * Mathf.Rad2Deg, 0);
        Vector3 vtOffset = qtn * (Vector3.back * m_flRadius);
        Vector3 vtForward = (new Vector3(-vtOffset.x, 0, -vtOffset.z)).normalized;
        Vector3 vtRight = new Vector3(vtForward.z, 0, -vtForward.x);

        // 최종 카메라 위치를 설정
        Vector3 vtTarget = m_vtCenter + (vtForward * m_flPivot2D_Y) + (vtRight * m_flPivot2D_X);
        Vector3 vtCameraPos = vtTarget + vtOffset * (2f - (m_flZoom + m_flZoomOffset));
        float flCameraOffestY = Mathf.Clamp(GetMapHeight(vtCameraPos) * c_flAdditionalHeight, vtCameraPos.y, float.MaxValue);

        float flValue = flCameraOffestY - vtCameraPos.y;
        #region 돌진 추가보정
        bool bRush = (CsGameData.Instance.MyHeroTransform.GetComponent<CsMyPlayer>().Rush);
        bool bRushStart = (CsGameData.Instance.MyHeroTransform.GetComponent<CsMyPlayer>().RushStart);

        if (flValue <= 0 && CsGameData.Instance.BoostMode == false) // 경사에 의한 보정이 없으면 바로 위치 변경.
        {
            float flDelaySecondResetDush = 1.0f;
            if (m_flResetCameraFromDush < flDelaySecondResetDush)
            {
                m_flZoom = Mathf.Lerp(m_flZoom, m_flTempZoom, m_flResetCameraFromDush / flDelaySecondResetDush);
                if (m_enCameraState != EnCameraState.QuarterView)
                {
                    transform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(transform.GetComponent<Camera>().fieldOfView, m_flDefaultFOV, m_flResetCameraFromDush / flDelaySecondResetDush);
                }
                m_flResetCameraFromDush += Time.deltaTime;
            }
            transform.position = vtCameraPos;
            transform.LookAt(m_vtCenter);

            bSetRushCamera = false;

        }
        //else if (CsIngameData.Instance.BoostMode == true && m_enCameraState != EnCameraState.QuarterView) // 돌진이라면!
        else if (CsGameData.Instance.BoostMode == true) // 05_28 기획 요청사항 쿼터뷰인 경우 돌진시 줌 아웃되도록.
        {
            //			Debug.Log("DoDush");
            if (bSetRushCamera == false)
            {
                m_flTempZoom = m_flZoom;
                bSetRushCamera = true;
                if (m_enCameraState == EnCameraState.QuarterView)
                {
                    SetDushCamera(-0.25f, 20f);
                }
                else
                {
                    SetDushCamera(0.2f, 20f);
                }
                m_flResetCameraFromDush = 0f;
                m_flSetCameraAtStartDush = 0f;
            }

            float flDelaySecondStartDush = 10.0f;

            if (m_flSetCameraAtStartDush <= flDelaySecondStartDush)
            {
                m_flZoom = Mathf.Lerp(m_flZoom, m_flTargetZoom, m_flSetCameraAtStartDush / flDelaySecondStartDush);
                if (m_enCameraState != EnCameraState.QuarterView)
                {
                    transform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(transform.GetComponent<Camera>().fieldOfView, m_flTargetFOV, m_flSetCameraAtStartDush / flDelaySecondStartDush);
                }
                m_flSetCameraAtStartDush += Time.deltaTime;
                //if (m_flSetCameraAtStartDush > flDelaySecondStartDush && m_flSetCameraAtStartDush < flDelaySecondStartDush)
                //{
                //    m_flSetCameraAtStartDush = flDelaySecondStartDush;
                //}
            }
            if (m_enCameraState != EnCameraState.Auto)
            {
                transform.position = vtCameraPos;
                transform.LookAt(m_vtCenter);
                //transform.position = Vector3.Slerp(transform.position, vtCameraPos, Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, vtCameraPos, Time.deltaTime * 100.0f);
            }
            //if (bRushStart) // 몬스터에 도달하면, 그때 카메라가 따라가도록 설정.
            //{
            //    transform.position = Vector3.Slerp(transform.position, vtCameraPos, Time.deltaTime * 12f);
            //}
        }

        else
        #endregion
        {
            if (flValue < 2)
            {
                vtCameraPos = new Vector3(vtCameraPos.x, flCameraOffestY, vtCameraPos.z);
            }
            else // 기존 시야각과 차이가 2m 벌어지면 줌인.
            {
                vtCameraPos = vtTarget + (vtOffset * 0.6f); // 최대 줌 값(2f - 1.4f) 으로 적용.
                vtCameraPos = new Vector3(vtCameraPos.x, Mathf.Clamp(GetMapHeight(vtCameraPos) * c_flAdditionalHeight, vtCameraPos.y, float.MaxValue), vtCameraPos.z); // 줌된 값에 Height 적용.
            }
            // 카메라의 포지션을 부드릅게 이동
            transform.position = Vector3.Slerp(transform.position, vtCameraPos, Time.deltaTime * 7f); // 카메라 이동을 부드럽게 이동.
            // 카메라가 바라볼 장소를 설정함.
            transform.LookAt(m_vtCenter);
            if (bSetRushCamera == true)
            {
                m_flHeight = m_flHeight + 1.4f;
                m_flScreenYHeight = m_flScreenYHeight - 0.3f;
                bSetRushCamera = false;
            }
        }

        if (m_enCameraState == EnCameraState.Auto)
        {
            //if (CsIngameData.Instance.IngameManagement.StateIdlethreeSec() == true)
            //{
                if (CompareValues() == false)
                {
                    //Debug.Log("1. CsIngameData.Instance.IngameManagement.StateIdlethreeSec()  " + CsIngameData.Instance.IngameManagement.StateIdlethreeSec());
                    if (m_flResetCameraDefaultAtAuto <= 1.0f)
                    {
                        m_flUpAndDownValue = Mathf.Lerp(m_flUpAndDownValue, m_defaultCameraSet.UpAndDown, m_flResetCameraDefaultAtAuto);
                        m_flResetCameraDefaultAtAuto += 0.02f;
                        if (m_flResetCameraDefaultAtAuto > 1.0f && m_flResetCameraDefaultAtAuto < 1.02f)
                        {
                            m_flResetCameraDefaultAtAuto = 1.0f;
                        }
                    }
                }
            //}
            else
            {
                //CsIngameData.Instance.IngameManagement.ResetStateIdSecond();
                m_flResetCameraDefaultAtAuto = 0f;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (m_flZoom > m_flMaxZoom + 0.02f)
            {
                m_flZoom -= 0.03f;
            }
            else
            {
                m_flZoom = m_flMaxZoom;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (m_flZoom < m_flMinZoom - 0.02f)
            {
                m_flZoom += 0.03f;
            }
            else
            {
                m_flZoom = m_flMinZoom;
            }
        }
    }

    //---------------------------------------------------------------------------------------------------
    void LateUpdate()
    {
        if (CsGameData.Instance.MyHeroTransform == null) return;

        CameraShake();
        //AutoChaseCamera();
    }

    //---------------------------------------------------------------------------------------------------
    protected void OnDestroy()
    {
        CsGameEvent.Instance.EventChangeCameraState -= OnEventChangeCameraState;
        CsGameEvent.Instance.EventChangeCameraDistance -= OnEvnetChangeCameraDistance;
        //CsIngameData.Instance.InGameCameraState = m_enCameraState;
        m_camera = null;
        m_txBlackWhite = null;
    }

    //---------------------------------------------------------------------------------------------------
    public void ChangeState()
    {
        if (m_enCameraState == EnCameraState.Auto)
        {
            Debug.Log("메뉴얼 카메라로 이동");
            AutoCamera();
        }
        else if (m_enCameraState == EnCameraState.Manual)
        {
            ManualCamera();
        }
        else if (m_enCameraState == EnCameraState.QuarterView)
        {
            QuarterViewCamera();
        }
        //else if (m_enCameraState == EnCameraState.Gyroscope)
        //{
        //    GyroscopeCamera();
        //}
        else if (m_enCameraState == EnCameraState.Dungeon) // 임시.
        {
            DungeonCamera();
        }
        Debug.Log("CsInGameCamera.ChangeState" + m_enCameraState);
    }

    //---------------------------------------------------------------------------------------------------
    public void ChangeNewState(EnCameraState enNewCameraState)
    {
        //SaveStateValue(m_enCameraState);
        if (enNewCameraState == EnCameraState.Auto)
        {
            AutoCamera();
        }
        else if (enNewCameraState == EnCameraState.Manual)
        {
            ManualCamera();
        }
        else if (enNewCameraState == EnCameraState.QuarterView)
        {
            QuarterViewCamera();
        }
        //else if (enNewCameraState == EnCameraState.Gyroscope)
        //{
        //    GyroscopeCamera();
        //}
        else if (enNewCameraState == EnCameraState.Dungeon)
        {
            DungeonCamera();
        }

        Debug.Log("#####     CsInGameCamera.ChangeState = " + m_enCameraState);
    }

    //---------------------------------------------------------------------------------------------------
    void SaveStateValue(EnCameraState enCameraState)
    {
        if (enCameraState == EnCameraState.Auto)
        {
            m_AutoStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
            m_flShareRightAndLeft = m_flRightAndLeftValue;
            m_flShareUpAndDown = m_flUpAndDownValue;
            m_flShareHeight = m_flHeight;
            m_flShareZoom = m_flZoom;
        }
        else if (enCameraState == EnCameraState.Manual)
        {
            m_ManualStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
            m_flShareRightAndLeft = m_flRightAndLeftValue;
            m_flShareUpAndDown = m_flUpAndDownValue;
            m_flShareHeight = m_flHeight;
            m_flShareZoom = m_flZoom;
        }
        else if (enCameraState == EnCameraState.QuarterView)
        {
            m_QuarterViewStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
            m_flShareRightAndLeft = m_flRightAndLeftValue;
        }
        //else if (enCameraState == EnCameraState.Gyroscope)
        //{
        //    m_GyroscopeStateValue = new CameraValue(m_flLength, m_flHeight, m_flPivot2D_X, m_flPivot2D_Y, m_flUpAndDownValue, m_flRightAndLeftValue, m_flZoom);
        //    m_flShareRightAndLeft = m_flRightAndLeftValue;
        //}
    }

    //---------------------------------------------------------------------------------------------------
    void ManualCamera()
    {
        //Input.gyro.enabled = false;
        //m_flHeightPlayerCenter = 0.8f;

        m_flMinZoom = 1.28f;
        m_flMaxZoom = 0.5f;
        m_flScreenYHeight = 0.2f;

        if (ieQuarterViewCoroutine != null && ieAutoCoroutine != null)
        {
            StopCoroutine(ieQuarterViewCoroutine);
            StopCoroutine(ieAutoCoroutine);
        }
        //SettingCameraModeValue(m_ManualStateValue.Length, m_flShareHeight, m_ManualStateValue.Pivot2D_X, m_ManualStateValue.Pivot2D_Y, m_flShareUpAndDown, m_flShareRightAndLeft, m_flShareZoom);
        ieMenualCoroutine = iESettingCameraModeValue(m_ManualStateValue.Length, m_flShareHeight, m_ManualStateValue.Pivot2D_X, m_ManualStateValue.Pivot2D_Y, m_flShareUpAndDown, m_flShareRightAndLeft, m_flShareZoom);
        StartCoroutine(ieMenualCoroutine);

        //StartCoroutine(DirectingValue(m_ManualStateValue.Height, m_ManualStateValue.Zoom));
    }

    //---------------------------------------------------------------------------------------------------
    void AutoCamera()
    {
        //Input.gyro.enabled = false;
        //m_enCameraState = EnCameraState.Auto;
        m_flMinZoom = 1.28f;
        m_flMaxZoom = 0.5f;
        //m_flZoom = 1.05f;
        //m_flScreenYHeight = 0.2f;
        //SettingCameraModeValue(m_AutoStateValue.Length, m_flShareHeight, m_AutoStateValue.Pivot2D_X, m_AutoStateValue.Pivot2D_Y, m_flShareUpAndDown, m_flShareRightAndLeft, m_flShareZoom);

        if (ieMenualCoroutine != null && ieQuarterViewCoroutine != null)
        {
            StopCoroutine(ieMenualCoroutine);
            StopCoroutine(ieQuarterViewCoroutine);
        }
        ieAutoCoroutine = iESettingCameraModeValue(m_AutoStateValue.Length, m_flShareHeight, m_AutoStateValue.Pivot2D_X, m_AutoStateValue.Pivot2D_Y, m_flShareUpAndDown, m_flShareRightAndLeft, m_flShareZoom);
        StartCoroutine(ieAutoCoroutine);
        //StartCoroutine(DirectingValue(m_AutoStateValue.Height, m_AutoStateValue.Zoom));
    }

    //---------------------------------------------------------------------------------------------------
    void QuarterViewCamera()
    {
        //Input.gyro.enabled = false;
        //m_enCameraState = EnCameraState.QuarterView;
        m_flMinZoom = 0.8f;
        m_flMaxZoom = 0.6f;
        //m_flScreenYHeight = -0.35f;
        Debug.Log("m_enCameraState  " + m_enCameraState);
        if (ieAutoCoroutine != null && ieMenualCoroutine != null)
        {
            StopCoroutine(ieMenualCoroutine);
            StopCoroutine(ieAutoCoroutine);
        }
        ieQuarterViewCoroutine = iESettingCameraModeValue(m_QuarterViewStateValue.Length, m_QuarterViewStateValue.Height, m_QuarterViewStateValue.Pivot2D_X, m_QuarterViewStateValue.Pivot2D_Y, m_QuarterViewStateValue.UpAndDown, m_flShareRightAndLeft, m_QuarterViewStateValue.Zoom);
        StartCoroutine(ieQuarterViewCoroutine);
        //SettingCameraModeValue(m_QuarterViewStateValue.Length, m_QuarterViewStateValue.Height, m_QuarterViewStateValue.Pivot2D_X, m_QuarterViewStateValue.Pivot2D_Y, m_QuarterViewStateValue.UpAndDown, m_flShareRightAndLeft, m_QuarterViewStateValue.Zoom);
        //StartCoroutine(DirectingValue(m_QuarterViewStateValue.Height, m_QuarterViewStateValue.Zoom));
    }

    //---------------------------------------------------------------------------------------------------
    void GyroscopeCamera()
    {
        //m_enCameraState = EnCameraState.Gyroscope;
        m_flMinZoom = 1.28f;
        m_flMaxZoom = 0.5f;
        //SettingCameraModeValue(m_GyroscopeStateValue.Length, m_GyroscopeStateValue.Height, m_GyroscopeStateValue.Pivot2D_X, m_GyroscopeStateValue.Pivot2D_Y, m_GyroscopeStateValue.UpAndDown, m_flShareRightAndLeft, m_GyroscopeStateValue.Zoom);
        StartCoroutine(iESettingCameraModeValue(m_GyroscopeStateValue.Length, m_GyroscopeStateValue.Height, m_GyroscopeStateValue.Pivot2D_X, m_GyroscopeStateValue.Pivot2D_Y, m_GyroscopeStateValue.UpAndDown, m_flShareRightAndLeft, m_GyroscopeStateValue.Zoom));
        //m_flScreenYHeight = 0.2f;
        m_qStartRot = Quaternion.Euler(transform.eulerAngles);
        //StartCoroutine(DirectingValue(m_GyroscopeStateValue.Height, m_GyroscopeStateValue.Zoom));
    }

    //---------------------------------------------------------------------------------------------------
    void DungeonCamera()
    {
        m_enCameraState = EnCameraState.Dungeon;
        m_flMinZoom = 1.1f;
        m_flMaxZoom = 1.1f;
        m_flOrgZoom = 1.1f;
        m_flOrgPivot2D_Y = 0f;
        m_flHeightPlayerCenter = 0.85f;
        m_flOrgUpAndDownValue = 0.06f;
        m_flOrgLength = 7f;
    }

    //---------------------------------------------------------------------------------------------------


    int m_nDelayCount = 0;
    float m_flSpeed = 3f;
    float m_flStartRotationY;
    Quaternion m_qStartRot;
    //---------------------------------------------------------------------------------------------------
    void AutoChaseCamera()
    {
        if (m_enCameraState == EnCameraState.Auto)
        {
            if (CsGameData.Instance.BoostMode == true)
            {
                transform.LookAt(m_vtCenter);
                if (CsGameData.Instance.JoysticDragging)
                {
                    m_flSpeed = CsGameData.Instance.JoysticCosValue; // 조이스틱 각도에 따른 속도 변화
                    Debug.Log(m_flSpeed);
                    //if (CsGameData.Instance.JoysticAngle < -0.7f && CsGameData.Instance.JoysticAngle > -2.3f) return;
                }
                if (CsGameData.Instance.IsHeroStateIdle() == false)
                {
                    AutoChangeRightAndLeft(CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y * Mathf.Deg2Rad); // MyHero 로테이션 값 변환(0 ~ 6.28f)
                }
                return;
                //AutoChangeRightAndLeft(CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            }
            //if (CsTouchInfo.Instance.DragByTouch() || CsTouchInfo.Instance.ZoomByTouch())
            //{
            //    Debug.Log("4. AutoChase ");
            //    m_nDelayCount = 30;
            //}
            //else
            {
                if (m_bShaking || m_bZoom || m_bBossAppearance) return;

                if (m_nDelayCount > 0)
                {
                    m_nDelayCount--;
                    return;
                }

                if (CsGameData.Instance.JoysticDragging)
                {
                    //m_flSpeed = 1.0f;
                    m_flSpeed = CsGameData.Instance.JoysticCosValue; // 조이스틱 각도에 따른 속도 변화
                                                                     //if (CsGameData.Instance.JoysticAngle < -0.7f && CsGameData.Instance.JoysticAngle > -2.3f) return;
                }
                //else if (CsIngameData.Instance.IngameManagement.IsHeroStateAttack())
                else if (CsGameData.Instance.IsHeroStateAttack())
                {
                    m_flSpeed = 13; // 6 이전 스피드 점프스킬 테스트용으로 바꿈
                }
                else
                {
                    m_flSpeed = 2;
                }

                //AutoChangeUpAndDown();

                //if (CsIngameData.Instance.IngameManagement.IsHeroStateIdle() == false)
                if (CsGameData.Instance.IsHeroStateIdle() == false)
                {
                    AutoChangeRightAndLeft(CsGameData.Instance.MyHeroTransform.rotation.eulerAngles.y * Mathf.Deg2Rad); // MyHero 로테이션 값 변환(0 ~ 6.28f)
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------------
    void AutoChangeUpAndDown() // 0.28 값으로 자동으로 보정시킴.
    {
        if (m_bAction || m_bJumpAction) return;
        if (m_flUpAndDownValue < 0.21f)
        {
            float flUpAndDownDelta = 0.2f - m_flUpAndDownValue; // 변경할 값과 변경전 값의 차이.
            float flChangeUpAndDownValue = Mathf.Lerp(0, flUpAndDownDelta, m_flSpeed * Time.deltaTime);
            m_flUpAndDownValue += flChangeUpAndDownValue;
        }
        else if (m_flUpAndDownValue > 0.19f)
        {
            float flUpAndDownDelta = 0.2f - m_flUpAndDownValue; // 변경할 값과 변경전 값의 차이.
            float flChangeUpAndDownValue = Mathf.Lerp(0, flUpAndDownDelta, m_flSpeed * Time.deltaTime);
            m_flUpAndDownValue += flChangeUpAndDownValue;
        }
    }

    //---------------------------------------------------------------------------------------------------
    void AutoChangeRightAndLeft(float flRightAndLeftValue) // 플레이어 뒤를 쫓아가는 카메라.
    {
        float flRightAndLeftDelta = flRightAndLeftValue - m_flRightAndLeftValue; // 변경할 값과 변경전 값의 차이.
        float flChangeValue = 0;
        bool bInCrease = false;

        if (flRightAndLeftDelta > 0.05f) // 변경될 값이 현재 값보다 클때
        {
            if (flRightAndLeftDelta > 3.14) // 변경될 각도가 180도가 넘어갈때 반대쪽으로 값 변경.
            {
                flChangeValue = Mathf.Lerp(0, 6.28f - flRightAndLeftDelta, m_flSpeed * Time.deltaTime);
            }
            else // 변경될 각도가 양수로 증가하며 180도가 안될때.
            {
                flChangeValue = Mathf.Lerp(0, flRightAndLeftDelta, m_flSpeed * Time.deltaTime);
                bInCrease = true;
            }
        }
        else if (flRightAndLeftDelta < -0.05f) // 변경될 값이 현재 값보다 작을때.
        {
            if (flRightAndLeftDelta < -3.14)  // 변경될 각도가 180도가 넘어갈때 반대쪽으로 값 변경.
            {
                flChangeValue = Mathf.Lerp(0, 6.28f + flRightAndLeftDelta, m_flSpeed * Time.deltaTime);
                bInCrease = true;
            }
            else// 변경될 각도가 음수로 감소하며 180도가 안될때.
            {
                flChangeValue = Mathf.Lerp(0, flRightAndLeftDelta, m_flSpeed * Time.deltaTime);
            }
        }

        ChangeRightandLeftValue(flChangeValue, bInCrease);
    }

    //---------------------------------------------------------------------------------------------------
    void ChangeRightandLeftValue(float flChangeValue, bool bInCrease)
    {
        flChangeValue = Mathf.Abs(flChangeValue);
        if (bInCrease) // 값 증가.
        {
            if (m_flRightAndLeftValue + flChangeValue > 6.28f)
            {
                m_flRightAndLeftValue = 6.28f - (m_flRightAndLeftValue + flChangeValue);
            }
            else
            {
                m_flRightAndLeftValue += flChangeValue;
            }
        }
        else // 값 감소.
        {
            if (m_flRightAndLeftValue + flChangeValue < 0)
            {
                m_flRightAndLeftValue = 6.28f + (m_flRightAndLeftValue + flChangeValue);
            }
            else
            {
                m_flRightAndLeftValue -= flChangeValue;
            }
        }
    }

    //---------------------------------------------------------------------------------------------------

    float m_flOffset = CsGameData.Instance.HeroMid + 1f;
    float m_flJumpOffset = 2.5f;
    float m_flJumpSpeed = 0.3f;

    bool m_bSkillAction = false;
    bool m_bJumpAction = false;
    float m_flFowardUpSpeed = 0.5f;
    float m_flFowardDownSpeed = 2f;
    float m_flHeightValue = 1;

    //---------------------------------------------------------------------------------------------------
    protected void SetCenterPos(Transform tr)
    {
        if (CsGameData.Instance.IsHeroStateIdle()) // 스킬 사용중. 연출.
        {
            if (m_bSkillAction) // 스킬 관련 연출 시작.
            {
                if (m_bJumpAction)
                {
                    CameraActionJump();
                }

                if (m_flForwardOffset < m_flFowardMax)
                {
                    m_flForwardOffset += m_flFowardUpSpeed;
                }
                else
                {
                    m_flForwardOffset = m_flFowardMax;
                }
            }
            else // 스킬연출 초기화.
            {
                if (m_flForwardOffset > 0)
                {
                    m_flForwardOffset -= m_flFowardDownSpeed;
                }
                else
                {
                    m_flForwardOffset = 0;
                }

                if (m_flHeightOffset > CsGameData.Instance.HeroMid)
                {
                    m_flHeightOffset -= 0.5f;
                }
                else
                {
                    m_flHeightOffset = CsGameData.Instance.HeroMid;
                }

                if (m_flHeight + 0.5f < 5f)
                {
                    m_flHeight += 0.5f;
                }
                else
                {
                    m_flHeight = 5f;
                }

                if (m_flZoom > m_flOrgZoom)
                {
                    m_flZoom -= 0.03f;
                }
            }
        }
        else
        {
            if (m_bAction) // 입장 연출
            {
                m_flHeightOffset = m_flHeightPlayerCenter + 0.5f;
            }
            else
            {
                if (CsGameData.Instance.InDungeon && !CsGameData.Instance.DungeonClear) // 던전 내에서만 작동, 클리어시에는 카메라 위치를 리셋하지 않음.
                {
                    ResetUpdateCamera();
                }
            }
        }

        //m_vtCenter = tr.position + (new Vector3(0f, m_flHeightOffset, m_flWidthOffset) + (tr.forward * m_flForwardOffset) +
        //            (tr.up * m_flScreenYHeight));
    }

    //---------------------------------------------------------------------------------------------------
    void ResetUpdateCamera()
    {
        m_flHeight = Mathf.Lerp(m_flHeight, 5.0f, Time.deltaTime * 4);

        if (m_flForwardOffset > 0)
        {
            m_flForwardOffset -= 1f;
        }
        else
        {
            m_flForwardOffset = 0;
        }

        if (m_flHeightOffset > m_flHeightPlayerCenter)
        {
            m_flHeightOffset -= 0.06f;
        }
        else if (m_flHeightOffset <= m_flHeightPlayerCenter)
        {
            //m_flHeightOffset = m_flHeightPlayerCenter;
        }



        if (m_flForwardOffset > 0)
        {
            m_flForwardOffset -= 0.15f;
        }
        else
        {
            m_flForwardOffset = 0f;
        }

        m_flScreenYHeight = Mathf.Lerp(m_flScreenYHeight, 0.0f, Time.deltaTime * 4);

        if (m_flZoom > m_flOrgZoom)
        {
            m_flZoom -= 0.03f;
        }

        if (m_flWidthOffset < 0)
        {
            m_flWidthOffset += 0.03f;
        }
        else
        {
            m_flWidthOffset = 0f;
        }

        if (m_flUpAndDownValue > m_flOrgUpAndDownValue)
        {
            m_flUpAndDownValue -= 0.01f;
        }
        else
        {
            m_flUpAndDownValue = m_flOrgUpAndDownValue;
        }

        if (m_flLength >= m_flOrgLength)
        {
            m_flLength = m_flOrgLength;
        }
        else if (m_flLength < m_flOrgLength)
        {
            m_flLength += 0.07f;
        }
    }

    //---------------------------------------------------------------------------------------------------
    void CameraActionJump()
    {
        if (m_flHeightOffset < m_flOffset + m_flJumpOffset)
        {
            m_flHeightOffset += m_flJumpSpeed;
        }
        else
        {
            m_flHeightOffset = m_flOffset + m_flJumpOffset;
        }

        if (m_flHeight - 0.5f > m_flHeightValue)
        {
            m_flHeight -= 0.5f;
        }
        else
        {
            m_flHeight = m_flHeightValue;
        }

        if (m_flZoom < 1.4f)
        {
            m_flZoom += 0.05f;
        }
        else
        {
            m_flZoom = 1.4f;
        }
    }

    //---------------------------------------------------------------------------------------------------
    //protected bool ZoomByTouch()
    //{
    //    if (m_bZoom || m_bShaking || m_bSkillAction || m_bAction) return false;

    //    if (CsTouchInfo.Instance.ZoomByTouch()) // Zoom Touch 관련 처리
    //    {
    //        float fl = m_flZoom;
    //        fl += (CsTouchInfo.Instance.ZoomDelta / 1000);
    //        m_flZoom = Mathf.Max(Mathf.Min(fl, m_flMinZoom), m_flMaxZoom);
    //        return true;
    //    }
    //    return false;
    //}

    //---------------------------------------------------------------------------------------------------
    //protected void DragByTouch()
    //{
    //    if (m_bZoom || m_bShaking || m_bSkillAction || m_bAction) return;

    //    if (CsTouchInfo.Instance.DragByTouch())
    //    {
    //        Vector2 vtPos = CsTouchInfo.Instance.TouchedScreenPosition;

    //        if (m_vtPrevPos != Vector2.zero && vtPos != m_vtPrevPos)
    //        {
    //            float flChangeValueX = c_flCameraDragSpeed * 2 * Mathf.Abs((m_vtPrevPos - vtPos).x);
    //            float flChangeValueY = c_flCameraDragSpeed * Mathf.Abs((m_vtPrevPos - vtPos).y);

    //            if ((m_vtPrevPos - vtPos).x < 0)        // X값 변경에 대한 처리.
    //            {
    //                if (m_flRightAndLeftValue + flChangeValueX > 6.28f)
    //                {
    //                    m_flRightAndLeftValue = 0;
    //                }
    //                else
    //                {
    //                    m_flRightAndLeftValue += flChangeValueX;
    //                }
    //            }
    //            else // if ((m_vtPrevPos - vtPos).x > 0)
    //            {
    //                if (m_flRightAndLeftValue - flChangeValueX < 0)
    //                {
    //                    m_flRightAndLeftValue = 6.28f;
    //                }
    //                else
    //                {
    //                    m_flRightAndLeftValue -= flChangeValueX;
    //                }
    //            }

    //            if (m_enCameraState != EnCameraState.QuarterView) // 쿼터뷰 아닐때만 드래그로 변경.
    //            {
    //                if ((m_vtPrevPos - vtPos).y < 0)        //Y값 변경에 대한 처리.
    //                {
    //                    if (m_flUpAndDownValue + 0.01f < 0.3f) // Max 0.4f
    //                    {
    //                        if (m_flUpAndDownValue + flChangeValueY > 0.3f)
    //                        {
    //                            m_flUpAndDownValue = 0.3f;
    //                        }
    //                        else
    //                        {
    //                            m_flUpAndDownValue += flChangeValueY;
    //                        }
    //                    }
    //                }
    //                else // if ((m_vtPrevPos - vtPos).y > 0)
    //                {
    //                    if (m_flUpAndDownValue - 0.01f > -0.6f) // Min -0.8f
    //                    {
    //                        if (m_flUpAndDownValue - flChangeValueY < -0.6f)
    //                        {
    //                            m_flUpAndDownValue = -0.6f;
    //                        }
    //                        else
    //                        {
    //                            m_flUpAndDownValue -= flChangeValueY;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        if (CompareValues() == false)
    //        {
    //            Debug.Log("ssszzzz");
    //            CsIngameData.Instance.IngameManagement.ResetStateIdSecond();
    //        }

    //        m_vtPrevPos = vtPos;
    //        return;
    //    }

    //    m_vtPrevPos = Vector2.zero;
    //}

    #region CameraEvent

    //---------------------------------------------------------------------------------------------------
    void OnEventChangeCameraState()
    {
        SaveStateValue(m_enCameraState);
        // 순차적으로 이동.
        if (m_enCameraState == EnCameraState.Auto)
        {
            m_enCameraState = EnCameraState.Manual;
        }
        else if (m_enCameraState == EnCameraState.Manual)
        {
            m_enCameraState = EnCameraState.Zoom;
        }
        if (m_enCameraState == EnCameraState.Zoom)
        {
            m_enCameraState = EnCameraState.Auto;
        }
        ChangeState();
    }

    //---------------------------------------------------------------------------------------------------
    void OnEvnetChangeCameraDistance()
    {
        if (m_bDungeonOutCamera)
        {
            Debug.Log("모드 체인지 False");
            m_bDungeonOutCamera = false;
        }
        else
        {
            Debug.Log("모드 체인지 true");
            m_bDungeonOutCamera = true;
        }
    }

    //---------------------------------------------------------------------------------------------------
    //public void ChangeTexture(bool bDead)
    //{
    //    if (bDead)
    //    {
    //        m_amplifyColorEffect.LutTexture = m_txBlackWhite;
    //    }
    //    else
    //    {
    //        m_amplifyColorEffect.LutTexture = m_txOirgin;
    //    }
    //}

    //---------------------------------------------------------------------------------------------------
    public void SetFieldOfView(float flField)
    {
        m_camera.fieldOfView = flField;
    }

    //---------------------------------------------------------------------------------------------------
    public void ResetFieldOfView()
    {
        m_camera.fieldOfView = 45;
    }

    //---------------------------------------------------------------------------------------------------
    public void ResetCamera()
    {
        m_bZoom = false;
        m_camera.cullingMask = m_nLayerMask;

        this.m_flLength = m_flOrgLength;
        this.m_flHeight = m_flOrgHeight;
        this.m_flPivot2D_X = m_flOrgPivot2D_X;
        this.m_flPivot2D_Y = m_flOrgPivot2D_Y;
        this.m_flUpAndDownValue = m_flOrgUpAndDownValue;
        this.m_flRightAndLeftValue = CsGameData.Instance.MyHeroTransform.eulerAngles.y * Mathf.Deg2Rad;
        this.m_flZoom = m_flOrgZoom;

        m_camera.nearClipPlane = m_flPrevNear;
    }

    //---------------------------------------------------------------------------------------------------
    public void DirectingEnd()
    {
        StartCoroutine(ResetDirecting());
    }

    public void DirectingBossEnd()
    {
        StartCoroutine(ResetDungeonBoss());
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator ResetDungeonBoss()
    {
        m_bBossAppearance = false;
        ResetUpdateCamera();
        yield return new WaitForSeconds(1.0f);
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator ResetDirecting() // 1초동안 원래 위치로 복귀.
    {
        yield return new WaitUntil(() => m_bShaking == false);

        float flLengthDelta = m_flOrgLength - m_flLength;
        float flOrgLength = m_flLength;

        float flHeightDelta = m_flOrgHeight - m_flHeight;
        float flOgnHeight = m_flHeight;

        float flPivot2D_XDelta = m_flOrgPivot2D_X - m_flPivot2D_X;
        float flOrgPivot2D_X = m_flPivot2D_X;

        float flPivot2D_YDelta = m_flOrgPivot2D_Y - m_flPivot2D_Y;
        float flOrgPivot2D_Y = m_flPivot2D_Y;

        float flTimer = 0f;

        while (flTimer <= 1f)
        {
            if (m_bZoom || m_bShaking)
            {
                break;
            }
            else
            {
                m_flLength = flLengthDelta * Mathf.InverseLerp(0.1f, 1.0f, flTimer) + flOrgLength;
                m_flHeight = flHeightDelta * Mathf.InverseLerp(0.1f, 1.0f, flTimer) + flOgnHeight;
                m_flPivot2D_X = flPivot2D_XDelta * Mathf.InverseLerp(0.1f, 1.0f, flTimer) + flOrgPivot2D_X;
                m_flPivot2D_Y = flPivot2D_YDelta * Mathf.InverseLerp(0.1f, 1.0f, flTimer) + flOrgPivot2D_Y;
            }
            flTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    //---------------------------------------------------------------------------------------------------
    public void CameraMove(float flUpAndDownValue, float flRightAndLeftValue, float flZoom) // 카메라 이동만.
    {
        Debug.Log("CsingameCmera.CameraMove() flUpAndDownValue = " + flUpAndDownValue + " // flRightAndLeftValue = " + flRightAndLeftValue + " // flZoom = " + flZoom);
        m_flUpAndDownValue = flUpAndDownValue;
        m_flRightAndLeftValue = flRightAndLeftValue;
        m_flZoom = flZoom;
    }

    //---------------------------------------------------------------------------------------------------
    public void ResetZoomValue(float flZoom, float flTime)
    {
        StartCoroutine(ChangeZoomValue(flZoom, flTime));
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator ChangeZoomValue(float flZoom, float flTime)
    {
        Debug.Log("1 ChangeZoomValue flZoom = " + flZoom + " m_flZoom = " + m_flZoom);
        yield return new WaitUntil(() => m_bShaking == false);
        float flZoomDelta = flZoom - m_flZoom;
        float flTimer = 0f;

        while (flTimer <= flTime)
        {
            Debug.Log("2 ChangeZoomValue  flZoomDelta = " + flZoomDelta + " //   = " + flZoomDelta * Mathf.InverseLerp(0, flTime, flTimer));
            m_flZoom = flZoomDelta * Mathf.InverseLerp(0, flTime, flTimer) + flZoom;
            flTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    //---------------------------------------------------------------------------------------------------
    public void ChangeCamera(float flUpAndDown, float flZoom)
    {
        StartCoroutine(DirectingValue(flUpAndDown, flZoom));
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator DirectingValue(float flHeight, float flZoom)
    {
        yield return new WaitUntil(() => m_bShaking == false);

        float flHeightDelta = flHeight - m_flHeightValue;
        float flOgnHeight = m_flHeightValue;
        float flZoomDelta = flZoom - m_flZoom;
        float flOgnZoom = m_flZoom;
        float flTimer = 0f;

        while (flTimer <= 0.4f)
        {
            if (!m_bZoom && !m_bShaking)
            {
                m_flUpAndDownValue = flHeightDelta * Mathf.InverseLerp(0.1f, 0.4f, flTimer) + flOgnHeight;
                m_flZoom = flZoomDelta * Mathf.InverseLerp(0.1f, 0.4f, flTimer) + flOgnZoom;
            }
            flTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;

        m_flOrgHeight = m_flHeightValue = flHeight;
        m_flOrgZoom = m_flZoom = flZoom;
    }

    //---------------------------------------------------------------------------------------------------
    public void DungeonClearCameraMove(float flUpAndDown, float flRightAndLeft)
    {
        StartCoroutine(DirectionDungeonCameraMove(flUpAndDown, flRightAndLeft));
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator DirectionDungeonCameraMove(float flUpAndDown, float flRightAndLeft)
    {
        float flTimer = 0f;
        float flUpAndDownVal = m_flUpAndDownValue - flUpAndDown;
        float flRightAndLeftVal = m_flRightAndLeftValue - flRightAndLeft;
        while (flTimer <= 6.0f)
        {
            if (m_flUpAndDownValue > flUpAndDown)
            {
                //Debug.Log("Change UP And Down");
                m_flUpAndDownValue -= flUpAndDownVal * Mathf.InverseLerp(0.1f, 100.0f, flTimer) * 0.05f;
                //Debug.Log("m_flUpAndDownValue =  " + m_flUpAndDownValue);
            }

            if (m_flRightAndLeftValue > flRightAndLeft)
            {
                //Debug.Log("Change Right And Left");
                m_flRightAndLeftValue -= flRightAndLeftVal * Mathf.InverseLerp(0.1f, 100.0f, flTimer) * 0.05f;
                //Debug.Log("m_flRightAndLeftValue =   " + m_flRightAndLeftValue);
            }

            flTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    //---------------------------------------------------------------------------------------------------
    public void PVPResultCameraMove(float flUpAndDown, float flZoom)
    {
        StartCoroutine(DirectionDungeonCameraMove(flUpAndDown, flZoom));
    }
    //---------------------------------------------------------------------------------------------------
    IEnumerator DirectionPVPResultCameraMove(float flUpAndDown, float flZoom)
    {
        float flTimer = 0f;
        float flUpAndDownVal = flUpAndDown - m_flUpAndDownValue;
        float flZoomVal = m_flZoom - flZoom;
        while (flTimer <= 3.0f)
        {
            if (m_flZoom > flZoom)
            {
                m_flZoom -= flZoomVal * Mathf.InverseLerp(0.1f, 3.0f, flTimer) * 0.05f;
            }

            if (flUpAndDown > m_flUpAndDownValue)
            {
                m_flUpAndDownValue += flUpAndDownVal * Mathf.InverseLerp(0.1f, 3.0f, flTimer) * 0.05f;
            }
        }

        flTimer += Time.deltaTime;
        yield return new WaitForEndOfFrame();
    }

    #endregion CameraEvent

    #region CameraShake

    //---------------------------------------------------------------------------------------------------
    bool CameraShake()
    {
        if (m_flShakeIntensity > 0)
        {
            transform.position = transform.position + UnityEngine.Random.insideUnitSphere * m_flShakeIntensity;
            transform.rotation = new Quaternion(m_qtnOriginalRot.x + UnityEngine.Random.Range(-m_flShakeIntensity, m_flShakeIntensity) * .2f,
                                                 m_qtnOriginalRot.y + UnityEngine.Random.Range(-m_flShakeIntensity, m_flShakeIntensity) * .2f,
                                                 m_qtnOriginalRot.z + UnityEngine.Random.Range(-m_flShakeIntensity, m_flShakeIntensity) * .2f,
                                                 m_qtnOriginalRot.w + UnityEngine.Random.Range(-m_flShakeIntensity, m_flShakeIntensity) * .2f);

            m_flShakeIntensity -= m_flShakeDecay;
        }
        else if (m_bShaking)
        {
            m_bShaking = false;
        }

        return m_bShaking;
    }

    //---------------------------------------------------------------------------------------------------
    public void DoShake(int nShakeLevel, bool bBlur)
    {
        //m_csInGameCameraBlur.StopCount = 8;
        //m_csInGameCameraBlur.enabled = bBlur;

        //if (CsTouchInfo.Instance.DragByTouch()) return;

        m_bShaking = true;
        m_qtnOriginalRot = transform.rotation;

        switch (nShakeLevel)
        {
            case 0:
                m_flShakeIntensity = 0.02f;
                m_flShakeDecay = 0.002f;
                break;
            case 1:
                m_flShakeIntensity = 0.02f;
                m_flShakeDecay = 0.003f;
                break;
            case 2:
                m_flShakeIntensity = 0.015f;
                m_flShakeDecay = 0.0015f;
                break;
            case 3:
                m_flShakeIntensity = 0.015f;
                m_flShakeDecay = 0.002f;
                break;
            case 4:
                m_flShakeIntensity = 0.015f;
                m_flShakeDecay = 0.003f;
                break;
            case 5:
                m_flShakeIntensity = 0.03f;
                m_flShakeDecay = 0.0015f;
                break;
            case 10:
                m_flShakeIntensity = 0.1f;
                m_flShakeDecay = 0.0015f;
                break;
            case 11:
                m_flShakeIntensity = 0.05f;
                m_flShakeDecay = 0.0015f;
                break;
        }
    }

    //---------------------------------------------------------------------------------------------------
    // 테스트용 검증후 쓰임세 체크 필요 11/30 안광열
    //---------------------------------------------------------------------------------------------------
    //IEnumerator ShakeCamera(float amplitude, float duration, float dampStartPercentage)
    //{
    //    dampStartPercentage = Mathf.Clamp(dampStartPercentage, 0.0f, 1.0f);

    //    float elapsedTime = 0.0f;
    //    float damp = 1.0f;

    //    Vector3 cameraOrigin = transform.position;

    //    while (elapsedTime < duration)
    //    {

    //        elapsedTime += Time.deltaTime;

    //        float percentComplete = elapsedTime / duration;

    //        if (percentComplete >= dampStartPercentage && percentComplete <= 1.0f)
    //        {
    //            damp = 1.0f - percentComplete;
    //        }

    //        Vector2 offsetValues = UnityEngine.Random.insideUnitCircle;

    //        offsetValues *= amplitude * damp;

    //        transform.position = new Vector3(offsetValues.x, offsetValues.y, cameraOrigin.z);

    //        yield return null;
    //    }

    //    transform.position = cameraOrigin;
    //}

    #endregion CameraShake

    #region HeightMap

    int[] m_anHeightInfo = null;
    int m_nHeightMapWidth = 0;
    int m_nHeightMapHeight = 0;
    int m_nHeightMapZeroX = 0;
    int m_nHeightMapZeroY = 0;
    int m_nHeightMapZeroZ = 0;
    float m_flMaxHeight = 0;

    //---------------------------------------------------------------------------------------------------
    public float GetMapHeight(Vector3 vtPos)
    {
        if (m_anHeightInfo == null) return 0;

        int xPos = Mathf.RoundToInt(vtPos.x - m_nHeightMapZeroX);
        int zPos = Mathf.RoundToInt(vtPos.z - m_nHeightMapZeroZ);

        if (xPos < 0 || zPos < 0 || xPos >= m_nHeightMapWidth || zPos >= m_nHeightMapHeight)
        {
            return 0;
        }

        return m_anHeightInfo[zPos * m_nHeightMapWidth + xPos] + m_nHeightMapZeroY;
    }

    //---------------------------------------------------------------------------------------------------
    private void InitializeHeightInfo() //  해당 씬의 HeightInfo 세팅.
    {
        Texture2D texture = Resources.Load<Texture2D>("HeightMap/HeightMap_" + gameObject.scene.name);

        if (texture == null)
        {
            Debug.Log("InGameCamera.InitializeHeightInfo()       texture == null");
            return;
        }

        m_nHeightMapWidth = texture.width;
        m_nHeightMapHeight = texture.height;

        GameObject zeroObject = GameObject.Find("HeightMap_Zero");
        if (zeroObject != null)
        {
            m_nHeightMapZeroX = Mathf.FloorToInt(zeroObject.transform.position.x);
            m_nHeightMapZeroY = Mathf.FloorToInt(zeroObject.transform.position.y);
            m_nHeightMapZeroZ = Mathf.FloorToInt(zeroObject.transform.position.z);
        }

        Color32[] colors = texture.GetPixels32();
        m_anHeightInfo = new int[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            m_anHeightInfo[i] = colors[i].a;
            m_flMaxHeight = Mathf.Max(m_anHeightInfo[i], m_flMaxHeight);
        }
    }
    #endregion HeightMap

    //---------------------------------------------------------------------------------------------------
    IEnumerator ResetDirectingSecond(float ResetTime) // 1초동안 원래 위치로 복귀.
    {
        yield return new WaitUntil(() => m_bShaking == false);

        float flLengthDelta = m_flOrgLength - m_flLength;
        float flOrgLength = m_flLength;

        float flHeightDelta = m_flOrgHeight - m_flHeight;
        float flOgnHeight = m_flHeight;

        float flPivot2D_XDelta = m_flOrgPivot2D_X - m_flPivot2D_X;
        float flOrgPivot2D_X = m_flPivot2D_X;

        float flPivot2D_YDelta = m_flOrgPivot2D_Y - m_flPivot2D_Y;
        float flOrgPivot2D_Y = m_flPivot2D_Y;

        float flTimer = 0f;

        while (flTimer <= ResetTime)
        {
            if (m_bZoom || m_bShaking)
            {
                break;
            }
            else
            {
                m_flLength = flLengthDelta * Mathf.InverseLerp(0.1f, ResetTime, flTimer) + flOrgLength;
                m_flHeight = flHeightDelta * Mathf.InverseLerp(0.1f, ResetTime, flTimer) + flOgnHeight;
                m_flPivot2D_X = flPivot2D_XDelta * Mathf.InverseLerp(0.1f, ResetTime, flTimer) + flOrgPivot2D_X;
                m_flPivot2D_Y = flPivot2D_YDelta * Mathf.InverseLerp(0.1f, ResetTime, flTimer) + flOrgPivot2D_Y;

            }
            flTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            m_bShaking = false;
        }
        yield return null;
    }

    //---------------------------------------------------------------------------------------------------
    public void DoAiming(bool bAiming, float ForwardSpeedUp, float ForwardSpeedDown) // 조준, 당기기
    {
        Debug.Log("DoAiming       bAiming = " + bAiming + " // m_enCameraState = " + m_enCameraState);
        //if (m_enCameraState == EnCameraState.Auto)
        //{
        //    if (bAiming)
        //    {
        //        m_flFowardMax = 3.2f;
        //        m_flFowardUpSpeed = ForwardSpeedUp;
        //        m_flFowardDownSpeed = ForwardSpeedDown;
        //        m_bSkillAction = true;
        //    }
        //    else
        //    {
        //        m_bSkillAction = false;
        //    }
        //}
    }

    //---------------------------------------------------------------------------------------------------
    public void DoJump(bool bJump, float flJumpHeight, float flHeight)
    {
        if (m_enCameraState == EnCameraState.Auto || m_enCameraState == EnCameraState.Dungeon) // 던전에도 적용함.
        {
            if (bJump)
            {
                //Time.timeScale = 0.3f;
                //m_flUpAndDownValue = 0.4f;
                m_flJumpOffset = flJumpHeight;
                m_flHeightValue = flHeight;
                m_flFowardMax = 2.2f;
                m_flFowardUpSpeed = 0.3f;
                m_flFowardDownSpeed = 1f;
                m_bSkillAction = true;
                m_bJumpAction = true;
            }
            else
            {
                //Time.timeScale = 1f;
                //m_flUpAndDownValue = 0.6f;
                m_bJumpAction = false;
                m_bSkillAction = false;
            }
        }
    }

    public void DoFollow(Transform tr)
    {
        ieCoroutine = FollowCamera(tr);
        StartCoroutine(ieCoroutine);
    }

    //---------------------------------------------------------------------------------------------------
    public void DoNotFollow()
    {
        if (ieCoroutine != null)
        {
            StopCoroutine(ieCoroutine);
        }
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator FollowCamera(Transform tr)
    {
        float FollowTime = 0;
        Debug.Log("손을 따라감 " + tr.name);
        Debug.Log("FollowCamera m_flHeightPlayerCenter : " + m_flHeightPlayerCenter + "tr.position.y : " + tr.position.y + "CsGameData.Instance.MyHeroTransform.position.y : " + CsGameData.Instance.MyHeroTransform.position.y);
        m_flHeightPlayerCenter = m_flHeightPlayerCenter - (tr.position.y - CsGameData.Instance.MyHeroTransform.position.y);
        m_flHeightOffset = m_flHeightPlayerCenter;
        while (FollowTime < 1.1f)
        {
            FollowTime += Time.deltaTime;
            m_trCameraTarget = tr;

            yield return null;
        }
        Debug.Log("다시 센터를 잡음");
        m_flHeightPlayerCenter = 0.85f;
        m_trCameraTarget = CsGameData.Instance.MyHeroTransform;
    }

    //---------------------------------------------------------------------------------------------------
    //public void NpcDialogStart(float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom, float flTime) // 카메라 이동만.
    //{
    //    int nLayerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Npc")) | (1 << LayerMask.NameToLayer("Terrain"))
    //                    | (1 << LayerMask.NameToLayer("Light")) | (1 << LayerMask.NameToLayer("NonLight")));

    //    m_bZoom = true;
    //    m_camera.cullingMask = nLayerMask;

    //    m_flZoom = flZoom;
    //    m_flPivot2D_Y = flPivot2D_Y;
    //    m_flUpAndDownValue = flUpAndDownValue;
    //    m_flRightAndLeftValue = flRightAndLeftValue;

    //    m_flPrevNear = m_camera.nearClipPlane;
    //    m_camera.nearClipPlane = 0.1f;
    //}

    //---------------------------------------------------------------------------------------------------
    //void DialogStart(float flUpAndDown, float flZoomValue, float flTime, bool bEnd)
    //{
    //    float Value2 = Mathf.Abs(flUpAndDown - m_flUpAndDownValue);
    //    float Value4 = Mathf.Abs(flZoomValue - m_flZoom);

    //    if (flUpAndDown > m_flUpAndDownValue) // 값이 증가.
    //    {
    //        StartCoroutine(IncreaseValue(m_flUpAndDownValue, flUpAndDown, Value2, m_flOrgUpAndDownValue, flTime, 2, bEnd));
    //    }
    //    else if (flUpAndDown < m_flUpAndDownValue) // 값이 감소.
    //    {
    //        StartCoroutine(DecreaseValue(m_flUpAndDownValue, flUpAndDown, Value2, m_flOrgUpAndDownValue, flTime, 2, bEnd));
    //    }

    //    if (flZoomValue > m_flZoom) // 값이 증가.
    //    {
    //        StartCoroutine(IncreaseValue(m_flZoom, flZoomValue, Value4, m_flOrgZoom, flTime, 4, bEnd));
    //    }
    //    else if (flZoomValue < m_flZoom) // 값이 감소.
    //    {
    //        StartCoroutine(DecreaseValue(m_flZoom, flZoomValue, Value4, m_flOrgZoom, flTime, 4, bEnd));
    //    }
    //}

    //---------------------------------------------------------------------------------------------------
    //IEnumerator IncreaseValue(float flValue1, float flValue2, float flValue3, float Value4, float flTime, int nType, bool bEnd)
    //{
    //    float flMoveTimer = 0f;
    //    if (bEnd)
    //    {
    //        m_vtCenter = CsGameData.Instance.MyHeroTransform.position;
    //    }
    //    else
    //    {
    //        m_vtCenter = CsGameData.Instance.MyHeroTransform.position + CsGameData.Instance.MyHeroTransform.forward;
    //    }

    //    if (nType == 1)
    //    {
    //        while (flMoveTimer <= flTime)
    //        {
    //            flMoveTimer += Time.deltaTime;
    //            m_flPivot2D_Y = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flMoveTimer) + flValue1;
    //            if (m_flPivot2D_Y == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flPivot2D_Y = flValue2;
    //    }
    //    else if (nType == 2)
    //    {
    //        while (flMoveTimer <= flTime)
    //        {
    //            flMoveTimer += Time.deltaTime;
    //            m_flUpAndDownValue = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flMoveTimer) + flValue1;
    //            if (m_flUpAndDownValue == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flUpAndDownValue = flValue2;
    //    }
    //    else if (nType == 3)
    //    {
    //        while (flMoveTimer <= flTime)
    //        {
    //            flMoveTimer += Time.deltaTime;
    //            m_flRightAndLeftValue = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flMoveTimer) + flValue1;
    //            if (m_flRightAndLeftValue == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flRightAndLeftValue = flValue2;
    //    }
    //    else if (nType == 4)
    //    {
    //        while (flMoveTimer <= flTime)
    //        {
    //            flMoveTimer += Time.deltaTime;
    //            m_flZoom = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flMoveTimer) + flValue1;
    //            if (m_flZoom == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flZoom = flValue2;
    //    }
    //}

    //---------------------------------------------------------------------------------------------------
    //IEnumerator DecreaseValue(float flValue1, float flValue2, float flValue3, float Value4, float flTime, int nType, bool bEnd)
    //{
    //    float flTimer = flTime;
    //    if (bEnd)
    //    {
    //        m_vtCenter = CsGameData.Instance.MyHeroTransform.position;
    //    }
    //    else
    //    {
    //        m_vtCenter = CsGameData.Instance.MyHeroTransform.position + CsGameData.Instance.MyHeroTransform.forward;
    //    }

    //    if (nType == 1)
    //    {
    //        while (flTimer >= 0)
    //        {
    //            flTimer -= Time.deltaTime;
    //            m_flPivot2D_Y = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flTimer) + flValue2;
    //            if (m_flPivot2D_Y == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flPivot2D_Y = flValue2;
    //    }
    //    else if (nType == 2)
    //    {
    //        while (flTimer >= 0)
    //        {
    //            flTimer -= Time.deltaTime;
    //            m_flUpAndDownValue = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flTimer) + flValue2;
    //            if (m_flUpAndDownValue == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flUpAndDownValue = flValue2;
    //    }
    //    else if (nType == 3)
    //    {
    //        while (flTimer >= 0)
    //        {
    //            flTimer -= Time.deltaTime;
    //            m_flRightAndLeftValue = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flTimer) + flValue2;
    //            if (m_flRightAndLeftValue == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flRightAndLeftValue = flValue2;
    //    }
    //    else if (nType == 4)
    //    {
    //        while (flTimer >= 0)
    //        {
    //            flTimer -= Time.deltaTime;
    //            m_flZoom = flValue3 * Mathf.InverseLerp(0.1f, flTime - 0.1f, flTimer) + flValue2;
    //            if (m_flZoom == flValue2)
    //            {
    //                break;
    //            }
    //            yield return new WaitForEndOfFrame();
    //        }
    //        m_flZoom = flValue2;
    //    }
    //}

    #region Transparency / Object 투명화 처리하기.

    //List<Transform> m_listPrevHitObjects = new List<Transform>();
    //Vector3 vtOffsetPos = new Vector3(0f, -0.5f, 0f);

    ////---------------------------------------------------------------------------------------------------
    //void ObjectTransparency()
    //{
    //    int nLayerMask = 1 << LayerMask.NameToLayer("1D_G") | 1 << LayerMask.NameToLayer("2D_G") | 1 << LayerMask.NameToLayer("3D_G");
    //    Debug.DrawRay(Camera.main.transform.position, CsGameData.Instance.MyHeroTransform.position - Camera.main.transform.position, Color.green);
    //    RaycastHit[] araycastHit = Physics.RaycastAll(Camera.main.transform.position, CsGameData.Instance.MyHeroTransform.position - Camera.main.transform.position, 10, nLayerMask);

    //    if (araycastHit.Length > 0)
    //    {
    //        for (int i = 0; i < araycastHit.Length; i++)
    //        {
    //            if (araycastHit[i].transform.CompareTag("Transparency"))
    //            {
    //                if (!m_listPrevHitObjects.Contains(araycastHit[i].transform))
    //                {
    //                    araycastHit[i].transform.GetComponent<CsTransparentArea>().SetAlpha(true);
    //                    m_listPrevHitObjects.Add(araycastHit[i].transform);
    //                }
    //            }
    //        }
    //    }

    //    if (m_listPrevHitObjects.Count > 0) // 1. 초기화 리스트 확인.
    //    {
    //        for (int i = 0; i < m_listPrevHitObjects.Count; i++)
    //        {
    //            bool bExit = true;
    //            for (int j = 0; j < araycastHit.Length; j++) // 2. 초기화 대상 범위 내에 있는지 확인.
    //            {
    //                if (araycastHit[j].transform == m_listPrevHitObjects[i])
    //                {
    //                    bExit = false;
    //                    break;
    //                }
    //            }

    //            if (bExit) // 3. 초기화 진행.
    //            {
    //                m_listPrevHitObjects[i].GetComponent<CsTransparentArea>().ResetAlpha();
    //                m_listPrevHitObjects.Remove(m_listPrevHitObjects[i]);
    //            }
    //        }
    //    }
    //}

    #endregion Transparency

    //---------------------------------------------------------------------------------------------------
    void LoginZoomIn()
    {
        Debug.Log("ZoomIn #######################################");
        this.m_flLength = m_LoginZoomValue.Length;
        this.m_flHeight = m_LoginZoomValue.Height;
        this.m_flPivot2D_X = m_LoginZoomValue.Pivot2D_X;
        this.m_flPivot2D_Y = m_LoginZoomValue.Pivot2D_Y;
        this.m_flUpAndDownValue = m_LoginZoomValue.UpAndDown;
        this.m_flRightAndLeftValue = m_LoginZoomValue.RightAndLeft;
        this.m_flZoom = m_LoginZoomValue.Zoom;
        this.m_flScreenYHeight = 0.3f;
    }

    //---------------------------------------------------------------------------------------------------
    public void ResetToInit()
    {
        Debug.Log("ResetToInit#######!!##");
        StartCoroutine(ResetFromZoom(m_defaultCameraSet.Length, m_defaultCameraSet.Height, m_defaultCameraSet.Pivot2D_X, m_defaultCameraSet.Pivot2D_Y, m_defaultCameraSet.UpAndDown, m_AutoStateValue.RightAndLeft, m_defaultCameraSet.Zoom));
    }

    //---------------------------------------------------------------------------------------------------
    public void SettingZoomValue(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom)
    {
        m_LoginZoomValue = new CameraValue(flLength, flHeight, flPivot2D_X, flPivot2D_Y, flUpAndDownValue, flRightAndLeftValue, flZoom);
        LoginZoomIn();
    }

    //---------------------------------------------------------------------------------------------------
    void SettingCameraModeValue(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom)
    {
        //m_flLength = flLength;
        //Debug.Log(" flHeight  " + flHeight);
        //m_flHeight = flHeight;
        //m_flPivot2D_X = flPivot2D_X;
        //m_flPivot2D_Y = flPivot2D_Y;
        //m_flUpAndDownValue = flUpAndDownValue;
        //m_flRightAndLeftValue = flRightAndLeftValue;
        //m_flZoom = flZoom;
        //if (m_enCameraState == EnCameraState.QuarterView)
        //{
        //    m_flScreenYHeight = -0.35f;
        //}
        //else
        //{
        //    m_flScreenYHeight = 0.2f;
        //}
    }

    IEnumerator iESettingCameraModeValue(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom)
    {
        float flResetDelay = 0.5f;
        float flResetTime = 0;

        float flScreenYHeight = 0.2f;
        //CsGameEventToUI.Instance.OnEventCameraButtonHide(false);
        while (flResetTime < flResetDelay)
        {
            flResetTime += Time.deltaTime;

            m_flLength = Mathf.Lerp(m_flLength, flLength, flResetTime / flResetDelay);
            m_flHeight = Mathf.Lerp(m_flHeight, flHeight, flResetTime / flResetDelay);
            m_flPivot2D_X = Mathf.Lerp(m_flPivot2D_X, flPivot2D_X, flResetTime / flResetDelay);
            m_flPivot2D_Y = Mathf.Lerp(m_flPivot2D_Y, flPivot2D_Y, flResetTime / flResetDelay);
            m_flUpAndDownValue = Mathf.Lerp(m_flUpAndDownValue, flUpAndDownValue, flResetTime / flResetDelay);
            m_flRightAndLeftValue = Mathf.Lerp(m_flRightAndLeftValue, flRightAndLeftValue, flResetTime / flResetDelay);
            m_flZoom = Mathf.Lerp(m_flZoom, flZoom, flResetTime / flResetDelay);

            if (m_enCameraState == EnCameraState.QuarterView)
            {
                flScreenYHeight = -0.35f;
            }
            else
            {
                flScreenYHeight = 0.2f;
            }

            m_flScreenYHeight = Mathf.Lerp(m_flScreenYHeight, flScreenYHeight, flResetTime / flResetDelay);


            yield return null;
        }
        //CsGameEventToUI.Instance.OnEventCameraButtonHide(true);
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator ResetFromZoom(float flLength, float flHeight, float flPivot2D_X, float flPivot2D_Y, float flUpAndDownValue, float flRightAndLeftValue, float flZoom)
    {
        float flLengthDelta = flLength - m_flLength;
        float flOgnLength = m_flLength;
        float flHeightDelta = flHeight - m_flHeight;
        float flOgnHeight = m_flHeight;
        float flPivot2D_XDelta = flPivot2D_X - m_flPivot2D_X;
        float flOgnPivot2D_X = m_flPivot2D_X;
        float flPivot2D_YDelta = flPivot2D_Y - m_flPivot2D_Y;
        float flOgnPivot2D_Y = m_flPivot2D_Y;
        float flUpAndDownValueDelta = flUpAndDownValue - m_flUpAndDownValue;
        float flOgnUpAndDownValue = m_flUpAndDownValue;
        float flRightAndLeftValueDelta = flRightAndLeftValue - m_flRightAndLeftValue;
        //float flOgnRightAndLeftValue = CsIngameData.Instance.IngameManagement.MyPlayer().transform.eulerAngles.y * Mathf.Deg2Rad;
        //if (flOgnRightAndLeftValue > 6.28)
        //{
        //    flOgnRightAndLeftValue = flOgnRightAndLeftValue - 6.28f;
        //}
        float flZoomDelta = flZoom - m_flZoom;
        float flOgnZoom = m_flZoom;

        float flTimer = 0.0f;

        while (flTimer <= 1.0f)
        {
            m_flLength = Mathf.Lerp(flOgnLength, flLength, flTimer);
            m_flHeight = Mathf.Lerp(flOgnHeight, flHeight, flTimer);
            m_flPivot2D_X = Mathf.Lerp(flOgnPivot2D_X, flPivot2D_X, flTimer);
            m_flPivot2D_Y = Mathf.Lerp(flOgnPivot2D_Y, flPivot2D_Y, flTimer);
            m_flUpAndDownValue = Mathf.Lerp(flOgnUpAndDownValue, flUpAndDownValue, flTimer);
            //m_flRightAndLeftValue = Mathf.Lerp(flOgnRightAndLeftValue, flRightAndLeftValue, flTimer);
            m_flZoom = Mathf.Lerp(flOgnZoom, flZoom, flTimer);
            m_flScreenYHeight = Mathf.Lerp(m_flScreenYHeight, 0.2f, flTimer);
            flTimer += 0.05f;
            if (flTimer > 1.0f && flTimer < 1.05f)
            {
                flTimer = 1.0f;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    //---------------------------------------------------------------------------------------------------
    void SetDushCamera(float flZoomVal, float flFOVVal)
    {
        m_flTargetZoom = m_flZoom + flZoomVal;
        m_flTargetFOV = transform.GetComponent<Camera>().fieldOfView + flFOVVal;
    }

    //---------------------------------------------------------------------------------------------------
    bool CompareValues()
    {
        if (m_flUpAndDownValue == m_defaultCameraSet.UpAndDown)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator CameraActionEnter(float delay, float duration, float Start_Height, float Start_Length, float Start_ScreenYHeight,
         float Start_Zoom, float Start_RightAndLeftValue,
         float End_Height, float End_Length, float End_RightAndLeftValue, float End_ScreenYHeight, float End_Zoom)
    {
        yield return new WaitForSeconds(delay); // 딜레이만큼 대기

        while (DelayWatingTime <= duration) // duration만큼 실행됨.
        {
            m_flHeight = Mathf.Lerp(Start_Height, End_Height, DelayWatingTime / duration);

            m_flLength = Mathf.Lerp(Start_Length, End_Length, DelayWatingTime / duration);

            m_flRightAndLeftValue = Mathf.Lerp(Start_RightAndLeftValue, End_RightAndLeftValue, DelayWatingTime / duration);

            m_flScreenYHeight = Mathf.Lerp(Start_ScreenYHeight, End_ScreenYHeight, DelayWatingTime / duration);

            m_flZoom = Mathf.Lerp(Start_Zoom, End_Zoom, DelayWatingTime / duration);
            DelayWatingTime += Time.deltaTime;
            yield return null;
        }

        m_flHeight = End_Height;
        m_flLength = End_Length;
        m_flRightAndLeftValue = End_RightAndLeftValue;
        m_flScreenYHeight = End_ScreenYHeight;
        m_flZoom = End_Zoom;
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator CameraActionEnter2(float delay, float duration, float Start_Height, float Start_Length, float Start_ScreenYHeight,
         float Start_Zoom,
         float End_Height, float End_Length, float End_ScreenYHeight, float End_Zoom)
    {
        yield return new WaitForSeconds(delay); // 딜레이만큼 대기

        while (DelayWatingTime <= duration) // duration만큼 실행됨.
        {
            m_flHeight = Mathf.Lerp(Start_Height, End_Height, DelayWatingTime / duration);

            m_flLength = Mathf.Lerp(Start_Length, End_Length, DelayWatingTime / duration);

            m_flScreenYHeight = Mathf.Lerp(Start_ScreenYHeight, End_ScreenYHeight, DelayWatingTime / duration);

            m_flZoom = Mathf.Lerp(Start_Zoom, End_Zoom, DelayWatingTime / duration);
            DelayWatingTime += Time.deltaTime;
            yield return null;
        }

        m_flHeight = End_Height;
        m_flLength = End_Length;
        m_flScreenYHeight = End_ScreenYHeight;
        m_flZoom = End_Zoom;
    }

    //---------------------------------------------------------------------------------------------------
    IEnumerator CameraActionEnter3(float delay, float duration, float Start_Height, float Start_Length, float Start_ScreenYHeight,
         float Start_Zoom,
         float End_Height, float End_Length, float End_ScreenYHeight, float End_Zoom,
         float End_UpAndDown)
    {
        yield return new WaitForSeconds(delay); // 딜레이만큼 대기

        while (DelayWatingTime <= duration) // duration만큼 실행됨.
        {
            m_flHeight = Mathf.Lerp(Start_Height, End_Height, DelayWatingTime / duration);

            m_flLength = Mathf.Lerp(Start_Length, End_Length, DelayWatingTime / duration);

            m_flScreenYHeight = Mathf.Lerp(Start_ScreenYHeight, End_ScreenYHeight, DelayWatingTime / duration);

            m_flZoom = Mathf.Lerp(Start_Zoom, End_Zoom, DelayWatingTime / duration);

            m_flUpAndDownValue = Mathf.Lerp(m_flUpAndDownValue, End_UpAndDown, DelayWatingTime / duration);
            DelayWatingTime += Time.deltaTime;
            yield return null;
        }

        m_flHeight = End_Height;
        m_flLength = End_Length;
        m_flScreenYHeight = End_ScreenYHeight;
        m_flZoom = End_Zoom;
        m_flUpAndDownValue = End_UpAndDown;
    }

    public void StartEnterStpe1()
    {
        StartCoroutine(CameraActionEnter(m_dungeonEnterStep_1_Set.Delay
            , m_dungeonEnterStep_1_Set.Duration
            , m_dungeonEnterStep_1_Set.Start_Height
            , m_dungeonEnterStep_1_Set.Start_Length
            , m_dungeonEnterStep_1_Set.Start_RightAndLeftValue
            , m_dungeonEnterStep_1_Set.Start_ScreenYHeight
            , m_dungeonEnterStep_1_Set.Start_Zoom
            , m_dungeonEnterStep_1_Set.End_Height
            , m_dungeonEnterStep_1_Set.End_Length
            , m_dungeonEnterStep_1_Set.End_RightAndLeftValue
            , m_dungeonEnterStep_1_Set.End_ScreenYHeight
            , m_dungeonEnterStep_1_Set.End_Zoom));
    }

    public void StartEnterStpe2()
    {
        StartCoroutine(CameraActionEnter2(m_dungeonEnterStep_2_Set.Delay
            , m_dungeonEnterStep_2_Set.Duration
            , m_flHeight
            , m_flLength
            , m_flScreenYHeight
            , m_flZoom
            , m_dungeonEnterStep_2_Set.End_Height
            , m_dungeonEnterStep_2_Set.End_Length
            , m_dungeonEnterStep_2_Set.End_ScreenYHeight
            , m_dungeonEnterStep_2_Set.End_Zoom));
    }

    public void StartEnterTaming()
    {
        StartCoroutine(CameraActionEnter3(m_dungeonTaming.Delay
            , m_dungeonTaming.Duration
            , m_flHeight
            , m_flLength
            , m_flScreenYHeight
            , m_flZoom
            , m_dungeonTaming.End_Height
            , m_dungeonTaming.End_Length
            , m_dungeonTaming.End_ScreenYHeight
            , m_dungeonTaming.End_Zoom
            , m_dungeonTaming.End_UpAndDonwValue));
    }

    public void SettingTamingCamera(bool bActive)
    {
        if(bActive)
        {
            m_defaultCameraSet.ScreenYHeight= m_flScreenYHeight = 5;
            m_defaultCameraSet.UpAndDown = m_flUpAndDownValue = -0.75f;
            m_defaultCameraSet.Length = m_flLength = 1f;
            m_defaultCameraSet.Height = m_flHeight = -1f;
        }
        else
        {
            m_defaultCameraSet.ScreenYHeight = m_flScreenYHeight = 0.2f;
            m_defaultCameraSet.UpAndDown = m_flUpAndDownValue = 0.2f;
            m_defaultCameraSet.Length = m_flLength = 0.9f;
            m_defaultCameraSet.Height = m_flHeight = 1f;

            m_flWidthOffset = 0.05f;
            m_flForwardOffset = 0.15f;
            m_flLength = m_flOrgLength = 0.9f;
            m_flHeight = m_flOrgHeight = 1f;
            m_flPivot2D_X = m_flOrgPivot2D_X = 0f;
            m_flPivot2D_Y = m_flOrgPivot2D_Y = 0.5f;
            m_flScreenYHeight = 0.2f;
            m_flUpAndDownValue = m_flOrgUpAndDownValue = 0.2f;
            m_flRightAndLeftValue = m_flOrgRightAndLeftValue = 0.7853f;   //  Mathf.PI / 4;
            m_flZoom = m_flOrgZoom = 1.05f;
        }
    }
}
