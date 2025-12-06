using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//스테이지 관리자
//클리어 판정, 씬 전환 및 연출, 죽음 처리..


public class StageManager : MonoBehaviour
{
    private bool _isPlayerDying = false;
    
    [Header("Death Settings")]
    public float deathPauseDuration = 4f; // 죽음 순간 조작 정지 시간
    public float gameOverImageDuration = 8f; // 게임 오버 이미지가 보여지는 시간
    
    [Header("UI & Sound")]
    public GameObject gameOverImageUI; // 인스펙터에 연결할 게임 오버 UI 이미지 오브젝트
    public AudioClip deathSound;       // 플레이어 피격/사망 순간 짧은 소리
    public AudioClip gameOverSound;    // 게임 오버 연출 중 재생할 소리
    
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
    public GameObject ClearDoor;
    private Animator _doorAnimator;
    
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

    
    //사운드
    private AudioSource _backGroundSound;
    private AudioSource _audioSource;
    
    
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
    
    
    //오브젝트 죽음 괸리(플레이어, 몬스터...)
    public void objectDeath(GameObject deadObject)
    {
        if (_isPlayerDying) return;
        
        // 플레이어 사망
        if (deadObject.CompareTag("Player"))
        {
            Debug.Log("player death");
        
            // **!!! 기존 ReloadCheckPoint() 대신 연출 코루틴 호출 !!!**
            StartCoroutine(PlayerDeathFlowCoroutine());
        }
        //else if (deadObject.CompareTag("Enemy"))
    }
    
    private IEnumerator PlayerDeathFlowCoroutine()
    {
        
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("Move");

        foreach (GameObject obj in objectsToDisable)
        {
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

            
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;

            }
        }

        _isPlayerDying = true;
        
        DisablePlayerControls(); // 조작 스크립트 비활성화
        StopPlayerPhysicsAndAnim();     // 속도 0, 중력 0으로 설정
        
        // 배경음악 정지
        _backGroundSound.Stop();
        
        // 사망 순간의 짧은 효과음 재생 (StageManager의 AudioSource 사용)
        if (_audioSource != null && deathSound != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }
        
        // 정지 상태 유지 (Unscaled Delta Time 사용)
        yield return new WaitForSeconds(deathPauseDuration);
        
        
        // 2. 페이드 인 (검은 화면) 및 Game Over 이미지/사운드 표시
        
        // 페이드 인 (화면을 검게 만듦)
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
        
        // Game Over 이미지 활성화 및 투명도
        if (gameOverImageUI != null)
        {

            // UI 요소를 활성화하고 투명도를 0으로 설정
            gameOverImageUI.SetActive(true);
            Image imageComp = gameOverImageUI.GetComponent<Image>();
            if (imageComp == null)
            {
                Debug.LogError("Game Over Image UI does not have an Image component.");
                yield break; // Image 컴포넌트가 없으면 코루틴 종료
            }
        
            // 초기 색상 가져오기 (알파를 0으로 초기화)
            Color startColor = imageComp.color;
            startColor.a = 0f;
            imageComp.color = startColor;
            
            // 게임 오버 사운드 재생
            if (_audioSource != null && gameOverSound != null)
            {
                _audioSource.PlayOneShot(gameOverSound);
            }
            
            // 게임 오버 이미지 점차 선명하게 (0 -> 1)
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            
                Color newColor = imageComp.color;
                newColor.a = alpha;
                imageComp.color = newColor;
            
                yield return null;
            }
            
            // 확실히 불투명하게 설정
            Color finalColor = imageComp.color;
            finalColor.a = 1f;
            imageComp.color = finalColor;

            // 게임 오버 이미지가 보여지는 시간 동안 대기
            yield return new WaitForSeconds(gameOverImageDuration);

            
            // 게임 오버 이미지 점차 사라짐
        
            // 게임 오버 이미지 점차 사라지게 (1 -> 0)
            timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            
                
                Color newColor = imageComp.color;
                newColor.a = alpha;
                imageComp.color = newColor;
            
                yield return null;
            }
        
            // 확실히 투명하게 설정
            finalColor = imageComp.color;
            finalColor.a = 0f;
            imageComp.color = finalColor;
            
            // 게임 오버 UI 비활성화
            gameOverImageUI.SetActive(false);
        }
    
        
        // 체크포인트 부활 및 조작 활성화 + 배경음악 다시 재생
        
        // 최신 체크포인트 기반 리로드 (플레이어 리스폰, 체력 회복, 저장된 오브젝트 상태 로드)
        ReloadCheckPoint(); 
        
        // 페이드 아웃 (화면을 다시 공개)
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 
        

        foreach (GameObject obj in objectsToDisable)
        {
            // 2. 오브젝트에 붙어있는 모든 MonoBehaviour 컴포넌트(스크립트)를 가져옵니다.
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

            
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;

            }
        }
        

        // 배경음악 다시 재생
        _backGroundSound.Play();
        
        _isPlayerDying = false;
        
        EnablePlayerControls();
        
        Debug.Log("Respawn Complete and Game Resumed.");
    }
    
    private void StopPlayerPhysicsAndAnim()
    {
        foreach (var player in _players)
        {
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            
                if (rb != null)
                {
                    // 잔여 속도/힘 강제 정지
                    rb.velocity = Vector2.zero; 
                    rb.angularVelocity = 0f;
                
                    // 중력제거
                    rb.gravityScale = 0f; 
                }
            }
        }
    }
    
    private void DisablePlayerControls()
    {
        // 모든 플레이어의 스크립트 비활성화 (피격, 입력, 움직임 로직 차단)
        foreach (var player in _players)
        {
            if (player != null)
            {
                //컨트롤러 스크립트 비활성(조작막기)
                player.enabled = false;
                
                
                //물리 효과 정지
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            
                if (rb != null)
                {
                    rb.velocity = Vector2.zero; 
                    rb.angularVelocity = 0f;
                
                    rb.gravityScale = 0f; 
                }
                
                //애니메이션 정지
                Animator anim = player.GetComponent<Animator>();
                if (anim != null)
                {
                    
                    anim.enabled = false; 
                }
            }
            

        }
    }

    private void EnablePlayerControls()
    {
        foreach (var player in _players)
        {
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = player._movement.gravityValue; 
                }
            
                //컨트롤러 스크립트 활성화
                player.enabled = true;
                
                //애니메이션 활성화
                Animator anim = player.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.enabled = true;
                }
            }
        }
    }

    public void SetCurrentCheckPoint(CheckPoint newCheckPoint)
    {
        if (_currentCheckPoint == newCheckPoint)
        {
            return;
        }
        
        
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

    
    // 씬 시작 연출
    private IEnumerator StartSceneFlowCoroutine()
    {
        _players[0].enabled = false;
        _players[1].enabled = false;
        
        //시네마틱 카메라 설정
        // 두 플레이어 중앙 위치 계산
        Vector3 centerPoint = (_players[0].transform.position + _players[1].transform.position) / 2f;
    
        // 컷신 카메라 위치 설정 (넓은 시야를 위해 ZDepth 조정 or OrthoSize 크게)
        if (cameraCinematic != null)
        {
            cameraCinematic.transform.position = new Vector3(centerPoint.x, centerPoint.y, cinematicCameraZDepth);
            // 연출을 위해 Orthographic Size 일시적으로 넓게 설정 
            cameraCinematic.GetComponent<Camera>().orthographicSize = 5f; 
        
            cameraCinematic.SetActive(true);
        }
    
        //분할 카메라 비활성
        cameraSplit1.SetActive(false);
        cameraSplit2.SetActive(false);
    
        //페이드 아웃
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 
    
        // 넓은 화면 보여주기 (duration만큼 대기)
        yield return new WaitForSeconds(initialWideViewDuration); 
    
        // 페이드 인 -> 다시 검은 화면으로 전환
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 

        // 분할 카메라로 복귀 및 컷신 카메라 정리
        cameraCinematic.SetActive(false);
    
        //분할 카메라 활성화
        cameraSplit1.SetActive(true);
        cameraSplit2.SetActive(true);
    
        // 화면 공개(게임시작)
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 
    
        // 플레이어 입력 활성화? 막아둘까
        // PlayerInput.EnableAll(); 
        
        _players[0].enabled = true;
        _players[1].enabled = true;
    
        _backGroundSound.Play();
        Debug.Log("Game Start: Split-Screen");
    }
    
    //스테이지 클리어 연출
    private IEnumerator StageClearFlowCoroutine()
    {
        _backGroundSound.Stop();
        //페이드 인
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
    
        // 컷신 중 플레이어 입력 정지. 필요하면..
        // PlayerInput.DisableAll(); 
    
        // 플레이어 중앙 위치
        Vector3 centerPoint = Vector3.zero;
        centerPoint = (_players[0].transform.position + _players[1].transform.position) / 2f;
        
        
        
        //분할 카메라 2개 비활성화
        cameraSplit1.SetActive(false);
        cameraSplit2.SetActive(false);

        //==연출용 카메라 설정==
        //플레이어 중앙 위치로부터 -> 문쪽으로 이동하며 확대 -> 문열리는 애니메이션..
        if (cameraCinematic != null)
        {
            cameraCinematic.transform.position = new Vector3(
                centerPoint.x, 
                centerPoint.y, 
                cinematicCameraZDepth 
            );
            cameraCinematic.SetActive(true); // 컷신 카메라 활성화
        }
    
        // ==연출 시작 및 페이드 아웃==
        // 검은 화면에서 컷신 카메라 시점 공개
        yield return StartCoroutine(screenFader.FadeScreen(0f, fadeDuration)); 

        // ==문 이동 연출==
        float cameraMoveTime = cutsceneDuration / 2f; // 총 컷신 시간의 절반을 카메라 이동에 사용
    
        if (cinematicCameraMover != null && ClearDoor != null)
        {
            cinematicCameraMover.MoveToDoor(ClearDoor.transform, cameraMoveTime); //target pos, 이동시간
        }
    
        // 카메라 이동 시간 동안 대기
        yield return new WaitForSeconds(cameraMoveTime);
    
        // ==문 애니메이션 대기== 
    
        float doorOpenAnimationTime = 5f; // 문 열림 애니메이션 시간 가정
    
        // 문 열림 애니메이션 시작 (DoorController.cs..)
        // exitDoor.OpenDoor(); 
        _doorAnimator.SetBool("openDoor", true);
        _doorAnimator.SetBool("startParticle", true);

        // 애니메이션 시간 동안 대기
        yield return new WaitForSeconds(doorOpenAnimationTime);
    
        //==페이드 인 및 다음 씬 전환
        yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
    
        SceneManager.LoadScene(nextStorySceneName);
    }
    
    

    void Start()
    {
        gameOverImageUI.gameObject.SetActive(false);
        
        _backGroundSound = GetComponent<AudioSource>();
        
        _doorAnimator = ClearDoor.GetComponent<Animator>();
        
        _savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointSavable>().ToArray();
        
        _players = FindObjectsOfType<PlayerController>();
     
        _audioSource = GetComponent<AudioSource>();
        
        
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
    
        // start 씬 시작 연출 
        StartCoroutine(StartSceneFlowCoroutine());
    }
    void Update()
    {
        //매 프레임 버튼 태그 확인
        CheckStageClear();
        
        
        
    }

}
