using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//스테이지 관리자
//클리어 판정, 씬 전환, 오브젝트 죽음 처리 관련 등..


public class StageManager : MonoBehaviour
{
    //다음 씬 이름(씬 전환용)
    public string nextSceneName;

    //레벨에 있는 클리어 버튼 할당(태그 확인용)
    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;

    private bool _isClearStage = false;
    
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
        if (deadObject.CompareTag("Player"))
        {
            Debug.Log("player death");
            //플레이어 죽음
        }
        //else if (deadObject.CompareTag("Enemy"))
    }
    
    void Start()
    {
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
