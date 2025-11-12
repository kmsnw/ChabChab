using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;

public class PlayerDetector : MonoBehaviour
{
    private EnemyAI ownerAI; // 부모의 AI 관리자
    
    // Start 시 부모에서 EnemyAI 컴포넌트 참조 (가장 중요한 연결)
    void Start()
    {
        ownerAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ownerAI.isPlayerInsideDetector = true; 
            Debug.Log("detected player enter");
            
            // 플레이어 감지 시, 부모 AI에게 알림
            ownerAI.OnPlayerDetected(other.transform); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("detected player exit");
            
            // 1. EnemyAI의 감지 플래그를 FALSE로 설정
            ownerAI.isPlayerInsideDetector = false; 
            
            // 2. (선택적) 이탈 시, AI에게 즉시 상태 전환을 명령할 수도 있으나,
            //    FSM 원칙상 ChaseState가 스스로 종료를 결정하는 것이 더 좋습니다.
            //    따라서 여기서는 플래그만 변경하고 ChaseState의 Execute()에 판단을 맡깁니다.
        }
    }
}
