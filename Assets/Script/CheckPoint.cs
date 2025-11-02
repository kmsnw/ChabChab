using System.Collections;
using System.Collections.Generic;using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

//체크포인트 스크립트
//자신에 트리거 되어 최신 체크포인트 갱신 -> StageManager에 자신의 객체를 전달(위치 정보 활용)
//2명 모두 체크포인트 트리거 시에만 체크포인트 갱신판정

public class CheckPoint : MonoBehaviour
{
    
    private StageManager _stageManager;

    [SerializeField]
    private int playersInZone = 0; //체크포인트에 트리거된 플레이어 수
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.GetComponent<PlayerController>() != null)
        {
            playersInZone++;
            
            if(playersInZone == 2)
                _stageManager.SetCurrentCheckPoint(this); //체크포인트 갱신
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (playersInZone > 0)
            {
                playersInZone--;
            }
        }
    }

    void Start()
    {
        _stageManager = FindObjectOfType<StageManager>();

        if (_stageManager == null)
        {
            Debug.LogError("StageManager not found");
            enabled = false;
        }
    }
    
}

