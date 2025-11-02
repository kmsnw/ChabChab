using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//스테이지 관리자
//클리어 판정, 씬 전환, 오브젝트 죽음 처리 관련 등..


public class StageManager : MonoBehaviour
{
    //최신 체크포인트
    private CheckPoint _currentCheckPoint;
    //현재 배치된 벽
    private GameObject _currentBoundaryWall;
    
    private ICheckpointSavable[] _savableObjects;
    
    //다음 씬 이름(씬 전환용)
    [Header("NextScene")]
    public string nextSceneName;
    
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

    //카메라
    private CameraController _camera;
    
    
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
        SceneManager.LoadScene(nextSceneName);
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

    //플레이어 위치 기반 카메라가 이동할 위치 산출
    //x축 기준 더 뒤에 있는 플레이어 위치를 기반 -> 대안: 두 플레이어 평균?
    public Vector3 SetCameraPosition()
    {
        Transform focusTarget = (_players[0].transform.position.x < _players[1].transform.position.x) ? _players[0].transform : _players[1].transform;

        Vector3 offset = new Vector3(2f, 0f, -10f);
        
        Vector3 targetPosition = focusTarget.position + offset;
        targetPosition.y = (_players[0].transform.position.y + _players[1].transform.position.y) / 2f;
        
        return targetPosition;
    }
    
    void Start()
    {
        _savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointSavable>().ToArray();
        
        _players = FindObjectsOfType<PlayerController>();
     
        _camera = FindObjectOfType<CameraController>();

        if (_camera == null)
        {
            Debug.LogError("CameraController 누락");
        }
        
        
        if (button1 == null || button2 == null)
        {
            Debug.LogError("Stage Manager: 클리어 버튼 오브젝트 누락");
            enabled = false;
        }
    }
    void Update()
    {
        //매 프레임 버튼 태그 확인
        CheckStageClear();
        
        _camera.SetTargetPosition(SetCameraPosition());

        
    }

    private void LateUpdate()
    {
   
          
    }

}
