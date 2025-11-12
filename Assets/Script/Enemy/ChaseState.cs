using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class ChaseState : IState
    {
        
        
        private EnemyAI owner;
        private Transform playerTarget; // 추적할 플레이어의 Transform

        // 추격 속도 (배회 속도보다 빠르게 설정될 수 있음)
        private float chaseSpeedMultiplier = 1.5f; 
        
        // AI가 너무 멀리 벗어나지 않도록 강제 복귀시킬 거리 (예: 반경의 1.5배)
        private float maxChaseDistanceMultiplier = 1.5f; 

        public ChaseState(EnemyAI owner, Transform target)
        {
            this.owner = owner;
            this.playerTarget = target;
        }

        public void Enter()
        {
            Debug.Log("Chase Endter");
            // 1. 추격 플래그 및 속도 설정
            owner.IsMoving = true;
            
            //추격 속도 설정
            owner.moveSpeed = owner.baseMoveSpeed * chaseSpeedMultiplier;
            
            //추격 애니메이션?
        }

        public void Execute()
        {
            if (playerTarget == null)
            {
                // 예외: 플레이어가 사라지면 즉시 복귀 상태로 전환
                owner.ChangeState(new ReturnState(owner)); 
                return;
            }

            // 1. 플레이어 방향으로 이동 결정
            float directionToPlayer = Mathf.Sign(playerTarget.position.x - owner.transform.position.x);
            owner.currentMoveDirection = directionToPlayer;

            // 2. 전환 조건 A: 플레이어가 감지 범위를 벗어났는지 확인
            if (!owner.IsPlayerDetected())
            {
                // 플레이어 이탈 시: ReturnState로 전환
                owner.ChangeState(new ReturnState(owner)); 
                return;
            }

            // 3. 전환 조건 B: AI가 자신의 배회 영역에서 너무 멀리 벗어났는지 확인
            float maxDistance = owner.patrolRadius * maxChaseDistanceMultiplier;
            float currentDistanceFromStart = Vector3.Distance(owner.transform.position, owner.startPosition);
    
            if (currentDistanceFromStart > maxDistance)
            {
                // ReturnState로 전환 (강제 복귀)
                owner.ChangeState(new ReturnState(owner)); 
                return;
            }
        }

        public void Exit()
        {
            // 속도 복원
            owner.moveSpeed = owner.baseMoveSpeed;
            
            // 2. 진행 중인 모든 코루틴 중지 (배회로 돌아갈 경우 필요)
            owner.StopAllCoroutines(); 
        }
    }
}
