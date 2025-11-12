using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//스테이지 관리자
//클리어 판정, 씬 전환, 오브젝트 죽음 처리 관련 등..


public class StageManager : MonoBehaviour
{
    [Header("Fade Panel")]
    public ScreenFader screenFader;
    public float fadeDuration = 0.5f;
    
    public float initialWideViewDuration = 2.0f; // 넓게 보여줄 시간 (1~2초)
    
    public float cinematicCameraZDepth = -10f;
    
    [Header("Cameras")] 
    public GameObject cameraSplit1;
    public GameObject cameraSplit2;
    public GameObject cameraCinematic;
    
    [Header("Cutscene")]
    public float cutsceneDuration = 5f;
    public string nextStorySceneName;
    
    public CutsceneCameraMover cinematicCameraMover;
    public Transform doorExitTransform;
    
    
    
    //최신 체크포인트
    private CheckPoint _currentCheckPoint;
    //현재 배치된 벽
    private GameObject _currentBoundaryWall;
    
    private ICheckpointSavable[] _savableObjects;
    

    //벽 프리팹 연결(이동 제약용)
    [Header("Boundary State")]
    [SerializeField]
    private GameObject _boundaryPrefab;
    public float boundaryOffset = 1.0f;
    
    //레벨에 있는 클리어 버튼 할당(태그 확인용)
    [Header("ClearButton")]
    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;

    private bool _isClearStage = false;
    
    
    //플레이어
    private PlayerController[] _players;

    
    private void CheckStageClear()
    {
        if (_isClearStage) return;
        
        //태그 확인 방식
        bool isButton1_On = button1.CompareTag("On");
        bool isButton2_On = button2.CompareTag("On");

        if (isButton1_On && isButton2_On)
        {
            Debug.Log("Clear Stage");
            stageClear();
        }
    }
    
    public void stageClear()
    {
        //_isClearStage: 플래그 -> LoadScene 호출 단 한 번만 호출 유도
        if (_isClearStage) return;
        
        _isClearStage = true;

        StartCoroutine(StageClearFlowCoroutine());
        
    }

    private IEnumerator StageClearFlowCoroutine()
    {
        // === 1단계: 🖤 페이드 인 (화면 전환 은폐) ===
        // BlackFadePanel을 반드시 StageManager에 연결해야 합니다.
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
    
        // 1. 플레이어 입력 정지 (선택 사항)
        // PlayerInput.DisableAll(); 
    
        // 2. 중앙 위치 계산 (Z값 고정 포함)
        Vector3 centerPoint = Vector3.zero;
        centerPoint = (_players[0].transform.position + _players[1].transform.position) / 2f;
        
    
        // === 2단계: 🔄 카메라 전환 및 위치 설정 ===
        cameraSplit1.SetActive(false);
        cameraSplit2.SetActive(false);

        if (cameraCinematic != null)
        {
            // X, Y는 플레이어 중앙으로, Z는 미리 정의된 깊이로 설정
            cameraCinematic.transform.position = new Vector3(
                centerPoint.x, 
                centerPoint.y, 
                cinematicCameraZDepth 
            );
            cameraCinematic.SetActive(true); // 컷신 카메라 활성화
        }
    
        // === 3단계: 🎥 연출 시작 및 페이드 아웃 ===
        // 검은 화면에서 컷신 카메라 시점 공개
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 

        // 문 이동 연출 시작
        float cameraMoveTime = cutsceneDuration / 2f; // 총 컷신 시간의 절반을 카메라 이동에 사용
    
        if (cinematicCameraMover != null && doorExitTransform != null)
        {
            cinematicCameraMover.MoveToDoor(doorExitTransform, cameraMoveTime);
        }
    
        // 카메라 이동 시간 동안 대기
        yield return new WaitForSeconds(cameraMoveTime);
    
        // === 4단계: 🚪 문 애니메이션 대기 ===
    
        float doorOpenAnimationTime = 2f; // 문 열림 애니메이션 시간 가정
    
        // 문 열림 애니메이션 시작 (DoorController...)
        // exitDoor.OpenDoor(); 

        // 애니메이션 시간 동안 대기
        yield return new WaitForSeconds(doorOpenAnimationTime);
    
        // === 5단계: 💡 최종 종료 및 씬 전환 ===
        // 다시 검은 화면으로 전환 (씬 로드를 숨김)
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
    
        // 다음 씬으로 전환
        SceneManager.LoadScene(nextStorySceneName);
    }


    
    //오브젝트 죽음 괸리(캐릭터, 몬스터...)
    public void objectDeath(GameObject deadObject)
    {
        //플레이어 사망
        if (deadObject.CompareTag("Player"))
        {
            Debug.Log("player death");
            
            ReloadCheckPoint(); //최신 체크포인트 기반 Reload
        }
        //else if (deadObject.CompareTag("Enemy"))
    }

    public void SetCurrentCheckPoint(CheckPoint newCheckPoint)
    {
        
        Debug.Log("체크포인트 갱신");
        _currentCheckPoint = newCheckPoint;

        //갱신 시점 기준 오브젝트들 상태 저장
        foreach (var savableObject in _savableObjects)
        {
            savableObject.SaveState();
        }
        
        //콜라이더(벽) 생성..
        if (_currentBoundaryWall != null)
        {
            Destroy(_currentBoundaryWall);
        }

        Vector3 wallSpawnPos = new Vector3(
            _currentCheckPoint.transform.position.x - boundaryOffset,
            _currentCheckPoint.transform.position.y,
            _currentCheckPoint.transform.position.z);

        if (_boundaryPrefab != null)
        {
            _currentBoundaryWall = Instantiate(_boundaryPrefab, wallSpawnPos, Quaternion.identity);
            _currentBoundaryWall.name = "BoundaryWall";
        }
        else
        {
            Debug.LogError("BoundaryPrefab not found");
        }

    }

    //체크 포인트 귀환으로 인한 스테이지 리로드
    public void ReloadCheckPoint()
    {
        Debug.Log("최신 체크포인트 기반 리로드");
        //인터페이스 구현 오브젝트들 로드

        foreach (var savableObject in _savableObjects)
        {
            savableObject.LoadState();
        }
        
        //currentCheckPoint 기반 플레이어 리스폰
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        Debug.Log("RespawnPlayer");
        
        if (_currentCheckPoint == null)
        {
            Debug.LogError("null currentCheckPoint");
            return;
        }
        
        //최신 체크포인트 위치
        Vector3 checkPointPos = _currentCheckPoint.transform.position;
        
        //체크포인트 기준 좌우 리스폰 오프셋
        const float playerOffset = 0.3f;
        
        //플레이어 리스폰
        //위치 조정 및 체력 회복
        for (int i = 0; i < _players.Length; i++)
        {
            GameObject playerObj = _players[i].gameObject;

            float spawnDirection = (i * 2) - 1;
            
            Vector3 spawnPos = new Vector3(
                checkPointPos.x + (playerOffset * spawnDirection),
                checkPointPos.y,
                checkPointPos.z);
            
            playerObj.transform.position = spawnPos;
            
            //currentHealth = maxHealth
            CharacterHealth healthComp = playerObj.GetComponent<CharacterHealth>();
            
            if (healthComp != null)
            {
                healthComp.HealFull();
            }
        }
    }

    // //플레이어 위치 기반 카메라가 이동할 위치 산출
    // //x축 기준 더 뒤에 있는 플레이어 위치를 기반 -> 대안: 두 플레이어 평균?
    // public Vector3 SetCameraPosition()
    // {
    //     Transform focusTarget = (_players[0].transform.position.x < _players[1].transform.position.x)
    //         ? _players[0].transform
    //         : _players[1].transform;
    //
    //     Vector3 offset = new Vector3(2f, 0f, -10f);
    //
    //     Vector3 targetPosition = focusTarget.position + offset;
    //     targetPosition.y = (_players[0].transform.position.y + _players[1].transform.position.y) / 2f;
    //
    //     return targetPosition;
    // }
    
    private IEnumerator StartSceneFlowCoroutine()
    {
        _players[0].enabled = false;
        _players[1].enabled = false;
        // 1. 🖤 페이드 인 (화면을 검게 만든 상태에서 시작)
        // 씬 로드 시 이미 검은 화면(Fade In)이 완료되어 있다고 가정하고 시작합니다.
        // 만약 씬 전환 시 페이드 아웃 로직을 넣지 않았다면, StartCoroutine(FadeScreen(1f, 0f));을 추가하세요.

        // 2. 🎥 시네마틱 카메라 초기 설정
        // 두 플레이어 중앙 위치 계산
        Vector3 centerPoint = (_players[0].transform.position + _players[1].transform.position) / 2f;
    
        // 컷신 카메라 위치 설정 (넓은 시야를 위해 ZDepth를 조정하거나, OrthoSize를 크게 설정)
        if (cameraCinematic != null)
        {
            cameraCinematic.transform.position = new Vector3(centerPoint.x, centerPoint.y, cinematicCameraZDepth);
            // 연출을 위해 Orthographic Size 일시적으로 넓게 설정 
            cameraCinematic.GetComponent<Camera>().orthographicSize = 5f; 
        
            cameraCinematic.SetActive(true);
        }
    
        // 3. 분할 카메라 비활성화
        cameraSplit1.SetActive(false);
        cameraSplit2.SetActive(false);
    
        // 4. 💡 넓은 화면 공개 (페이드 아웃)
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 
    
        // 5. 넓은 화면 보여주기 (1~2초간 멈춤 상태)
        yield return new WaitForSeconds(initialWideViewDuration); 
    
        // 6. 🖤 페이드 인 (다시 검은 화면으로 전환)
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 

        // 7. 🔄 분할 카메라로 복귀 및 컷신 카메라 정리
        cameraCinematic.SetActive(false);
    
        // 분할 화면 카메라의 Viewport Rect 설정을 Start()에서 이미 했으므로, 활성화만 합니다.
        cameraSplit1.SetActive(true);
        cameraSplit2.SetActive(true);
    
        // 8. 💡 화면 공개 및 게임 시작
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 
    
        // 9. 플레이어 입력 활성화 및 AI 시작
        // PlayerInput.EnableAll(); // (별도 구현 필요)
        
        _players[0].enabled = true;
        _players[1].enabled = true;
    
        Debug.Log("Game Start: Split-Screen Mode");
    }
    

    void Start()
    {
        _savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointSavable>().ToArray();
        
        _players = FindObjectsOfType<PlayerController>();
     
        
        if (button1 == null || button2 == null)
        {
            Debug.LogError("Stage Manager: 클리어 버튼 오브젝트 누락");
            enabled = false;
        }
        
        
        if (cameraSplit1.activeSelf || cameraSplit2.activeSelf)
        {
            // 씬 로드 시 활성화된 상태라면 비활성화 (StartSceneFlowCoroutine에서 켜짐)
            cameraSplit1.SetActive(false);
            cameraSplit2.SetActive(false);
        }
    
        // 2. 씬 시작 연출 플로우 시작
        StartCoroutine(StartSceneFlowCoroutine());
    }
    void Update()
    {
        //매 프레임 버튼 태그 확인
        CheckStageClear();
        
        
        
    }

    private void LateUpdate()
    {
   
          
    }

}
