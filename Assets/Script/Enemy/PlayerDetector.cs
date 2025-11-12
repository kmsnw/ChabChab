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
            
            // 플레이어 감지 시, Enemy AI에 알림
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
                
        }
    }
}
