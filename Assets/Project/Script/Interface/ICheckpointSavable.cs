using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퍼즐 오브젝트(정적 오브젝트, 몬스터......) 구현 인터페이스
//초기 상태정보(리로드 시 사용될 상태값(위치, 상태...) 및 로드 동작 구현

public interface ICheckpointSavable
{
    //리로드시 활용될 초기 정보 설정
    void SaveState();
    
    //저장된 상태 기반 복원
    void LoadState();
}
