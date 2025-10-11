using System.Collections;
using System.Collections.Generic;using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

//체크포인트 스크립트
//자신에 트리거 되어 최신 체크포인트 갱신 -> StageManager에 자신의 객체를 전달(위치 정보 활용)

public class CheckPoint : MonoBehaviour
{
    private StageManager _stageManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            _stageManager.SetCurrentCheckPoint(this);
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

