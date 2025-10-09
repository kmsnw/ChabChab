using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//클리어 버튼 컨트롤러
//상호작용 인터페이스 구현 -> on off 태그 변경

public class ClearButtonController : MonoBehaviour, IInteractable
{
    //상호작용 인터페이스 구현
    public string InteractionName => "Clear Button"; //본인 이름
   
    //상호작용 동작 구현
    public void Interact(bool isInteracting, PlayerController player)
    {
        //상호작용 상태에 따라 태그 변경
        if (isInteracting) gameObject.tag = "On";
        else gameObject.tag = "Off";
    }
    
    
    
    void Start()
    {
        //기본 태그값 설정(버튼 비활성화)
        gameObject.tag = "Off";
    }
    
}
