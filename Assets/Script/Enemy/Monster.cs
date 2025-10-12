using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//몬스터 뿌리 클래스
//DamageSource를 상속해 플레이어가 닿을 때 데미지

public class Monster : DamageSource, ICheckpointSavable
{
    private Vector3 _initialPosition;
    private CharacterHealth _healthComp;
    
    
    public void SaveState()
    {
        
    }
    public void LoadState()
    {
        gameObject.SetActive(true); //활성화(부활)
        transform.position = _initialPosition; //위치 초기화
        _healthComp.HealFull(); //체력 회복
    }

    

    // Start is called before the first frame update
    void Start()
    {
        _healthComp = GetComponent<CharacterHealth>();
        if (_healthComp == null)
        {
            Debug.LogError("Health component not found");
        }
        _initialPosition = transform.position;
    }

}
