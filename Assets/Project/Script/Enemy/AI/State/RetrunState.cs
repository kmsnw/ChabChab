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
        
        public ReturnState(EnemyAI owner)
        {
            this.owner = owner;
            this.destination = owner.startPosition;
        }

        public void Enter()
        {
            owner.IsMoving = true;
            owner.animator.SetBool("isMove", true);
        }

        public void Execute()
        {
            // 목표 지점 방향으로 이동 결정
            float directionToStart = Mathf.Sign(destination.x - owner.transform.position.x);
            owner.currentMoveDirection = directionToStart;
            
            // 복귀 완료 조건 체크
            // X축 위치기반 복귀 여부 체크
            if (Mathf.Abs(owner.transform.position.x - destination.x) < 0.1f)
            {
                // 복귀 완료 -> patrol 전환
                owner.IsMoving = false;
                owner.ChangeState(new PatrolState(owner)); // <-- PatrolState로 전환
                
            }
        }

        public void Exit()
        {
            // 복귀 중 이동을 멈춤 -> 트리거 관계 없이 강제복귀
            owner.currentMoveDirection = 0f;
        }
    }
}