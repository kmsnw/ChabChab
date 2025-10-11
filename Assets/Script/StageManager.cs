using System.Collections;
using System.Collections.Generic;
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

    
    private void CheckStageClear()
    {
        if (_isClearStage) return;
        
        //태그 확인 방식
        bool isButton1_On = button1.CompareTag("On");
        bool isButton2_On = button2.CompareTag("On");

        if (isButton1_On && isButton2_On)
        {
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
        
        
        //currentCheckPoint 기반 플레이어 리스폰
        RespawnPlayer();
        
    }

    public void RespawnPlayer()
    {
        Debug.Log("RespawnPlayer");
        //최신 체크포인트 위치
        Vector3 checkPointPos = _currentCheckPoint.transform.position;
        
        //체크포인트 기준 좌우 리스폰 오프셋
        const float playerOffset = 0.3f;

        
        //체크포인트 기반 플레이어 위치 재설정
        GameObject playerObj1 = _players[0].gameObject;
        playerObj1.transform.position = new Vector3(
            checkPointPos.x - playerOffset,
            checkPointPos.y,
            checkPointPos.z
            );
        
        GameObject playerObj2 = _players[1].gameObject;
        playerObj2.transform.position = new Vector3(
            checkPointPos.x + playerOffset,
            checkPointPos.y,
            checkPointPos.z
            );
        
    }
    
    void Start()
    {
        _players = FindObjectsOfType<PlayerController>();
        
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
    }
}
