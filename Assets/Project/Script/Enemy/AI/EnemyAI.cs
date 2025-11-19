using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI.FSM;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour, ICheckpointSavable
{
    [HideInInspector] public float baseMoveSpeed;
    
    public bool isPlayerInsideDetector = false;
        
    private Rigidbody2D rb;
    
    //체크포인트 로드
    private Vector3 _initialPosition;
    private CharacterHealth _healthComp;
    
    [Header("FSM")]
    private IState currentState;
    
    [Header("Patrol Data")]
    public float patrolRadius; //배회 반경
    public float moveSpeed; //배회 이동 속도
    
    public float centerBiasMax = 0.4f; //가중치. 반경 내 위치를 기반으로 확률을 상이하게 적용
    // -> 경계에 가까울 수록, 가중치 원본에 근사. -> 중앙 회귀 확률 상승 -> "경계쪽으로 갈 수록 중앙 회귀 확률이 높음"

    public bool IsMoving = true; //이동 상태 여부 플래그
    
    public Vector3 startPosition; //초기 시작 위치
    //private Vector3 targetPosition; //목표 위치
    public float currentMoveDirection = 1.0f;
    
    //
    public void ChangeState(IState newState)
    {
        Debug.Log("ChangeState");
        currentState.Exit();
        currentState = newState;
        currentState.Enter();

    }

    public bool IsAtPatrolBoundary()
    {
        float distance = transform.position.x - startPosition.x;

        if (Mathf.Abs(distance) >= patrolRadius)
        {
            //경계 도달 및 벗어남
            return true;
        }

        //이동 반경 내 위치
        return false;
    }

    public void MoveTarget()
    {
        if (rb == null) return;
        
        
        //목표 지점까지의 방향벡터
        Vector2 Velocity = new Vector2(currentMoveDirection * moveSpeed, rb.velocity.y);
        
        rb.velocity = Velocity;
    }

    public float DecideDirection()
    {
        //예:
        //AI가 중앙에서 50%거리(전체 반경 1/4지점), 
        
        //현 AI 위치 중앙 대비 떨어져 있는 비율 계산
        float relativeX = transform.position.x - startPosition.x;
        
        //중앙으로 돌아가려는 가중지 계산
        float currentDistanceRatio = Mathf.Abs(relativeX) / patrolRadius;
        float centerBias = currentDistanceRatio * centerBiasMax;
        
        //중앙으로 돌아갈 확률
        float probTowardCenter = 0.5f + centerBias; // 기본 확률 0.5 + 가중치 합산 -> 경계에 가까울수록 중앙방향 선정 확류 100%에 근사
        

        
        //중앙 방향과 반대 방향 결정
        // > 0 : 중앙기준 오른쪽 위치 -> 중앙방향은 왼쪽(-1)
        float directionTowardCenter = (relativeX > 0) ? -1f : 1f;

        //확률 기반 최종 방향 결정
        if (Random.value < probTowardCenter)
        {
            //중앙으로 돌아감
            return directionTowardCenter;
        }
        else
        {
            //중앙에서 멀어짐
            return -directionTowardCenter;
        }
    }
    
    // 플레이어가 감지
    public void OnPlayerDetected(Transform player)
    {
        Debug.Log("OnDetected");
        
        // 현 상태가 PatrolState일 때만 Chase 전환 -> 중복 전환 방지
        if (currentState is PatrolState) 
        {
            ChangeState(new ChaseState(this, player));
        }
    }
    
    
    public bool IsPlayerDetected()
    {
        return isPlayerInsideDetector; 
    }
    
    
    //몬스터의 체크포인트 기반 save,load 동작
    public void SaveState()
    {
        
    }
    public void LoadState()
    {
        gameObject.SetActive(true); //활성화(부활)
        transform.position = _initialPosition; //위치 초기화
        _healthComp.HealFull(); //체력 회복
    }

    
    
    void Start()
    {
        baseMoveSpeed = moveSpeed;
        
     
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D component not found");
    
        _healthComp = GetComponent<CharacterHealth>();
        if (_healthComp == null) Debug.LogError("Health component not found");

        currentState = new PatrolState(this);
        _initialPosition = transform.position; // 체크포인트 로드를 위한 초기 위치
        
        startPosition = _initialPosition;      // FSM 배회 중심 위치 설정
    
        // FSM (ChangeState를 통해 한 번만 호출)
        // PatrolState의 Enter() 호출(첫 시작 디폴트 배회)
        ChangeState(new PatrolState(this)); 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("excute currentState: " + currentState);
        //매 프레임 상태 로직 수행
        currentState.Execute();
    }

    private void FixedUpdate()
    {
        if (IsMoving) //이동
        {
            MoveTarget();
        }
        else // 정지
        {
            if (rb != null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }
}
