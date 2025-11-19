using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//상호작용 가능 오브젝트가 구현할 인터페이스
//상호작용 동작 관련 함수 및 본인 이름 구현 필요

public interface IInteractable
{
    //상호작용 수행 함수
    //isInteracting(상호작용 여부)에 따른 동작 구현
    void Interact(bool isInteracting, PlayerController player);
    
    
    //상호작용 대상 이름정보 -> 피드백, 디버깅, 시각 표현..
    string InteractionName { get; }
    
}