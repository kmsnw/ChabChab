using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

//배회 state.
//

namespace AI.FSM
{
    public class PatrolState : IState
    {
        private EnemyAI owner;
        private float stopChance = 0.3f;
        private float stopDuration = 2f;

        private float decisionTimer;
        private float decisionInterval = 2.0f;

        public PatrolState(EnemyAI owner)
        {
            this.owner = owner;
        }
        
        
        
        
        public void Enter()
        {
            owner.IsMoving = true;

        }
        public void Execute()
        {
            //추격 상태로 전환
            
            
            //이동 로직 수행 -> targetpos..
            if (owner.IsMoving)
            {
                if (owner.IsAtPatrolBoundary()) //경계 도달, 초과
                {
                    owner.currentMoveDirection *= -1f;
                }
            }
            
            decisionTimer -= Time.deltaTime;

            if (decisionTimer <= 0f)
            {
                StopOrMoveDecision();

                decisionTimer = decisionInterval;

            }
            
            
            //배회 반경 동작
            
        }
        
        public void Exit()
        {
            //코루틴 정지
            owner.StopAllCoroutines();
        }

        
        private void StopOrMoveDecision()
        {
            if (Random.value < stopChance)
            {
                // 정지 상태(코루틴 수행. duration 만큼 확정정지)
                owner.IsMoving = false;
                owner.StartCoroutine(StopAndWaitCoroutine(stopDuration));
            }
            else
            {
                //1. 확률 기반 새 방향 결정
                float newDirection = owner.DecideDirection();
                

                //2. 새로운 방향을 EnemyAI에 전달
                owner.currentMoveDirection = newDirection;
                owner.IsMoving = true;
                
            }
        }

        private IEnumerator StopAndWaitCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            owner.IsMoving = true; //duration 후 이동 플래그 재활성화
        }

    }
    
}

