using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//chase -> patrol 사이 중간 역할
//먼저 강제로 시작 위치 기반 복귀 이후 다시 patrol 진행

namespace AI.FSM
{
    public class ReturnState : IState
    {
        private EnemyAI owner;
        private Vector3 destination; // 복귀 목표 지점 (startPosition)
        
        // ReturnState는 항상 PatrolState로 복귀해야 하므로, PatrolState의 생성자를 호출합니다.
        public ReturnState(EnemyAI owner)
        {
            this.owner = owner;
            this.destination = owner.startPosition;
        }

        public void Enter()
        {
            // 1. 추격 속도 그대로 복귀 이동 시작
            owner.IsMoving = true;
        }

        public void Execute()
        {
            // 1. 목표 지점 방향으로 이동 결정
            float directionToStart = Mathf.Sign(destination.x - owner.transform.position.x);
            owner.currentMoveDirection = directionToStart;
            
            // 2. 복귀 완료 조건 체크
            // X축 위치만 비교하여 목표 지점에 도달했는지 확인합니다.
            if (Mathf.Abs(owner.transform.position.x - destination.x) < 0.1f)
            {
                // 복귀 완료!
                owner.IsMoving = false;
                owner.ChangeState(new PatrolState(owner)); // <-- PatrolState로 전환
            }
        }

        public void Exit()
        {
            // 복귀 중 이동을 멈추기 위해 (선택적)
            owner.currentMoveDirection = 0f;
        }
    }
}